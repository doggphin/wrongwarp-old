using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WrongWarp
{
    public class TickManager : NetworkBehaviour
    {
        public static TickManager Instance => instance;
        private static TickManager instance;
        public const int framesPerTick = 5;
        public const int tickDelay = 5;
        public const int minimumDeathFrames = 30;
        private readonly InputPacket blankInputs = new();

        public static event Action<InputPacket> PollInput = delegate { };
        public static event Action<byte, int> PollTransforms = delegate { };
        public static event Action PollEntities = delegate { };
        public static event Action<ushort> TickStart = delegate { };

        private Dictionary<ushort, Dictionary<byte, Dictionary<NetworkEntity, ArbitraryData>>> serverQueuedActions = new();
        private Dictionary<NetworkIdentity, ArbitraryData> polledEntityADFrameCache = new();
        private Dictionary<ushort, Dictionary<NetworkIdentity, InputPacket[]>> serverReceivedInputPackets = new();
        private Dictionary<ushort, Tick> serverCachedTicks = new();
        private Dictionary<ushort, Tick> clientCachedTicks = new();

        public ushort globalTick = 0;
        public byte globalFrame = 0;
        public static InputPacket[] clientFrameInputs = new InputPacket[framesPerTick];
        public static bool pollInputs = false;



        private void Awake()
        {
            if (instance == null) { instance = this; } else { Destroy(gameObject); }
            DontDestroyOnLoad(gameObject);
            serverCachedTicks[0] = new();
            serverReceivedInputPackets[0] = new();
            for(int i=0; i < framesPerTick; i++)
            {
                clientFrameInputs[i] = new();
            }
        }



        #region Synchronization

        public void Start()
        {
            if (!isServer)
            {
                CmdRequestTickResync();
            }
        }



        /// <summary>
        /// Called by clients to ask for their tick/frame to be synced with the server again.
        /// </summary>
        /// <param name="sender"></param>
        [Command(requiresAuthority = false)]
        public void CmdRequestTickResync(NetworkConnectionToClient sender = null)
        {
            Debug.Log($"Instructing client to use {globalTick - tickDelay}.");
            TargetSetTimeRpc(sender, (ushort)(globalTick - tickDelay), globalFrame);
        }



        /// <summary>
        /// Sets the tick and frame of a target.
        /// </summary>
        [TargetRpc]
        public void TargetSetTimeRpc(NetworkConnection target, ushort tick, byte frame)
        {
            Debug.Log($"Setting this client's time to {tick}:{frame}");
            globalTick = tick;
            globalFrame = frame;
        }

        #endregion Synchronization



        private void FixedUpdate()
        {
            if (isClient)
            {
                PollInput(clientFrameInputs[globalFrame]);
                RunInputsOnPlayer(NetworkClient.localPlayer.GetComponent<PlayerMovement>(), clientFrameInputs[globalFrame]);
                if(!isServer)
                {
                    ClientRunFrame(globalTick, globalFrame);
                }
            }

            if (isServer)
            {
                RunAndRecordServerFrame(globalTick, globalFrame);
                PollEntities();
            }

            globalFrame++;
            if (globalFrame == framesPerTick)
            {
                globalFrame = 0;
                if (isClient)
                {
                    ClientUploadInput();
                    for(int i=0; i<framesPerTick; i++)
                    {
                        clientFrameInputs[i].Reset();
                    }
                }
                if (isServer)
                {
                    PublishServerTick(globalTick);
                }

                globalTick++;
                if (isServer)
                {
                    serverCachedTicks[globalTick] = new();
                }
            }
        }



        // ================================================================================================================================================================================================
        // Shared Functions                                                                                                                                                                               
        // ================================================================================================================================================================================================

        /// <summary>
        /// Runs inputs on a connection's player gameObject.
        /// </summary>
        private void RunInputsOnPlayer(PlayerMovement movement, InputPacket packet)
        {
            if (movement.gameObject != null)
            {
                movement.UpdatePosition(packet);
                if (packet.ReadInputValue(InputPacket.InputButton.Interact))
                {
                    movement.gameObject.GetComponentInChildren<InteractRaycast>().RaycastInteract();
                }
                foreach(ArbitraryDataSlice slice in packet.data.slices)
                {
                    PlayPropertySliceOnIdentity(movement.netIdentity, slice);
                }
            }
        }



        // ================================================================================================================================================================================================
        // Server Functions
        // ================================================================================================================================================================================================

        [Command(requiresAuthority = false)]
        private void CmdUploadInput(NetworkedInputPacket[] inputPacket, NetworkConnectionToClient sender = null)
        {
            ushort _tick = (ushort)(globalTick + 1);

            // If received an input packet of abnormal length, kick the player
            if (inputPacket.Length != framesPerTick) { sender.Disconnect(); return; }

            // If the tick doesn't exist yet, create it
            if (!serverReceivedInputPackets.ContainsKey(_tick))
            {
                serverReceivedInputPackets[_tick] = new();
            }
            // If the player already uploaded input for this frame (high rtt variance), upload it to the frame after
            else if (serverReceivedInputPackets[_tick].ContainsKey(sender.identity))
            {
                _tick++;
                // If the tick doesn't exist yet, create it
                if (!serverReceivedInputPackets.ContainsKey(_tick))
                {
                    serverReceivedInputPackets[_tick] = new();
                }
            }

            serverReceivedInputPackets[_tick][sender.identity] = new InputPacket[framesPerTick];

            // For every frame in the uploaded tick of inputs,
            for (int i = 0; i < framesPerTick; i++)
            {
                // Set the identity's input packets
                serverReceivedInputPackets[_tick][sender.identity][i] = new InputPacket(inputPacket[i]);
            }
        }



        /// <summary>
        /// Runs all client inputs on the server to complete a frame.
        /// </summary>
        [Server]
        private void RunAndRecordServerFrame(ushort tick, byte frame)
        {
            // If no inputs were received this frame, make serverReceivedInputPackets non-null
            if(!serverReceivedInputPackets.ContainsKey(tick))
            {
                serverReceivedInputPackets[tick] = new();
            }

            Dictionary<NetworkConnectionToClient, ArbitraryData> playerDataCache = new();

            // For every player in the game,
            foreach (var connection in NetworkServer.connections.Values)
            {
                PlayerMovement playerMovement = connection.identity.gameObject.GetComponent<PlayerMovement>();

                bool playerUsedInputs = serverReceivedInputPackets[tick].TryGetValue(connection.identity, out InputPacket[] inputsPacketArrayCache);

                if (connection.connectionId != NetworkClient.connection.connectionId)
                {
                    RunInputsOnPlayer(playerMovement, playerUsedInputs ?
                            inputsPacketArrayCache[frame] :
                            blankInputs
                    );
                }

                if (playerUsedInputs)
                {
                    playerDataCache[connection] = new();

                    // Only upload player look vector if it has changed
                    // With camera rotation, use a byte to represent its rotation to save data
                    bool vectorHasChanged;
                    if(frame == 0)
                    {
                        if (serverReceivedInputPackets.TryGetValue((ushort)(tick - 1), out var identityInputPacketArrayPair))
                        {
                            if(identityInputPacketArrayPair.TryGetValue(connection.identity, out var inputPacketArray))
                            {
                                vectorHasChanged = WWBinaryFunctions.CompressFloatDegreesToByte(inputsPacketArrayCache[frame].lookVector.x) != WWBinaryFunctions.CompressFloatDegreesToByte(inputPacketArray[framesPerTick - 1].lookVector.x);
                            }
                            else
                            {
                                vectorHasChanged = true;
                            }
                        }
                        else
                        {
                            vectorHasChanged = true;
                        }
                    }
                    else
                    {
                        vectorHasChanged = WWBinaryFunctions.CompressFloatDegreesToByte(inputsPacketArrayCache[frame].lookVector.x) != WWBinaryFunctions.CompressFloatDegreesToByte(inputsPacketArrayCache[frame - 1].lookVector.x);
                    }
                    if (vectorHasChanged)
                    {
                        playerDataCache[connection].AddDataSlice(ADDataType.Byte, WWBinaryFunctions.CompressFloatDegreesToByte(inputsPacketArrayCache[frame].lookVector.x), (int)FrameActionID.SetVerticalLookRotation);
                    }

                    // If player is jumping, play it
                    if (inputsPacketArrayCache[frame].ReadInputValue(InputPacket.InputButton.Jump))
                    {
                        ArbitraryDataSlice slice = playerDataCache[connection].AddEmptyDataSlice((int)FrameActionID.PlayJumpAnimation);
                        PlayPropertySliceOnIdentity(connection.identity, slice);
                    }

                    // Handle extra data stored in the data field of the input packet
                    for(int i=0; i<inputsPacketArrayCache[frame].data.slices.Count; i++)
                    {
                        switch (inputsPacketArrayCache[frame].data.slices[i].ID)
                        {
                            case (int)FrameActionID.PlayEmote:
                                switch (inputsPacketArrayCache[frame].data.slices[i].GetElementAsByte(i))
                                {
                                    case (int)EmotePlayer.EmoteID.Alert:
                                        ArbitraryDataSlice slice = playerDataCache[connection].AddDataSlice(ADDataType.Byte, (int)EmotePlayer.EmoteID.Alert, (int)FrameActionID.PlayEmote);
                                        if (connection != NetworkServer.connections[0]) { PlayPropertySliceOnIdentity(connection.identity, slice); }
                                        break;
                                }
                                break;
                        }
                    }
                }
            }

            NullableTransformState transformStateCache;

            // For every NetworkIdentity in the server, store its transform state within the frame
            foreach (NetworkIdentity identity in NetworkServer.spawned.Values)
            {
                // Do not try to set identity position/rotation if it's not active
                if (identity.gameObject.activeInHierarchy)
                {
                    transformStateCache = new NullableTransformState(identity.gameObject.transform);

                    if (!serverCachedTicks[tick].frameDataOfIdentities[frame].TryGetValue(identity, out FrameData frameDataCache))
                    {
                        serverCachedTicks[tick].frameDataOfIdentities[frame].Add(identity, new FrameData(transformStateCache));
                    }
                    else
                    {
                        frameDataCache.nTransformState = transformStateCache;
                    }
                }
            }

            foreach(var playerData in playerDataCache)
            {
                serverCachedTicks[tick].frameDataOfIdentities[frame][playerData.Key.identity].data.slices.AddRange(playerData.Value.slices);
            }

            if(serverQueuedActions.TryGetValue(tick, out var serverQueuedActionsTick))
            {
                if(serverQueuedActionsTick.TryGetValue(frame, out var serverQueuedActionsFrame))
                {
                    foreach (var entityDataPair in serverQueuedActionsFrame)
                    {
                        if (entityDataPair.Key != null)
                        {
                            foreach (ArbitraryDataSlice slice in entityDataPair.Value.slices)
                            {
                                switch (slice.ID)
                                {
                                    case (int)SpecialEntityActions.Kill:
                                        NetworkServer.Destroy(entityDataPair.Key.gameObject);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            byte DegreeConverter(float degrees)
            {
                return (byte)(degrees / 180 * byte.MaxValue);
            }
        }



        private void PlayPropertySliceOnIdentity(NetworkIdentity identity, ArbitraryDataSlice slice)
        {
            switch(slice.ID)
            {
                case (int)FrameActionID.PlayJumpAnimation:
                    BillboardManager.CreateBillboard(BillboardManager.TestSprite, identity.gameObject.transform.position, .25f, 1); 
                    break;
                case (int)FrameActionID.PlayEmote:
                    identity.GetComponent<EmotePlayer>().ClientPlayEmote(Enum.IsDefined(typeof(EmotePlayer.EmoteID), slice.GetElementAsByte()) ? (EmotePlayer.EmoteID)slice.GetElementAsByte() : EmotePlayer.EmoteID.Alert);
                    break;
                case (int)FrameActionID.SetVerticalLookRotation:
                    PlayerMovement playerMovement = identity.GetComponent<PlayerMovement>();
                    playerMovement.PlayerCamera.transform.rotation = Quaternion.Euler(WWBinaryFunctions.DecompressByteDegrees(slice.GetElementAsByte(0)), playerMovement.PlayerCamera.transform.rotation.eulerAngles.y, playerMovement.PlayerCamera.transform.rotation.eulerAngles.z);
                    break;
            }
        }



        /// <summary>
        /// Combines frames together within a tick to cache and later send to clients.
        /// </summary>  
        [Server]
        private void PublishServerTick(ushort tick)
        {
            // First create the tick and store the tick value in it
            ArbitraryData publishedTickData = new();
            publishedTickData.AddDataSlice(ADDataType.UShort, tick);

            // Create  caches to save performance (maybe) and lines
            Dictionary<NetworkIdentity, FrameData> previousFrameIdentitiesFrameData;
            NullableTransformState currentTransformStateCache;

            // For every frame in the tick,
            for (int i = 0; i < framesPerTick; i++)
            {
                // Add a marker for the beginning of new frames, except for the first one because that's already implied
                if (i != 0) { publishedTickData.AddFrameMarker(); }

                // If on the first frame of the first tick, skip it and continue since it causes problems
                if (tick == 0 && i == 0 && framesPerTick > 1) { continue; }

                // Cache the last frame.
                previousFrameIdentitiesFrameData = i == 0 ?
                    serverCachedTicks[(ushort)(tick - 1)].frameDataOfIdentities[framesPerTick - 1] :
                    serverCachedTicks[tick].frameDataOfIdentities[i - 1];

                // For every identity + data pair,
                foreach (var identityFrameData in serverCachedTicks[tick].frameDataOfIdentities[i])
                {
                    // Make sure the identity still exists;
                    if (identityFrameData.Key != null)               // VERY expensive I assume
                    {
                        // Add the identity's NetID as a data slice,
                        publishedTickData.AddDataSlice(ADDataType.NetID, identityFrameData.Key.netId);

                        // Cache the identity's transform values
                        currentTransformStateCache = new NullableTransformState(identityFrameData.Key.transform);
                        if (previousFrameIdentitiesFrameData.TryGetValue(identityFrameData.Key, out FrameData frameDataCache))
                        {
                            // If the networkidentity existed last frame, only include parts of its transform state that changed
                            if (previousFrameIdentitiesFrameData[identityFrameData.Key].nTransformState.position != currentTransformStateCache.position)
                            {
                                publishedTickData.AddDataSlice(ADDataType.Vector3, currentTransformStateCache.position, (int)FrameActionID.SetPosition);
                            }
                            if (previousFrameIdentitiesFrameData[identityFrameData.Key].nTransformState.rotation != currentTransformStateCache.rotation)
                            {
                                publishedTickData.AddDataSlice(ADDataType.Quaternion, currentTransformStateCache.rotation, (int)FrameActionID.SetRotation);
                            }
                            if (previousFrameIdentitiesFrameData[identityFrameData.Key].nTransformState.scale != currentTransformStateCache.scale)
                            {
                                publishedTickData.AddDataSlice(ADDataType.Vector3, currentTransformStateCache.scale, (int)FrameActionID.SetScale);
                            }
                        }
                        else
                        {
                            // If the networkidentity didn't exist last frame, include all its values
                            publishedTickData.AddDataSlice(ADDataType.Vector3, currentTransformStateCache.position, (int)FrameActionID.SetPosition);
                            publishedTickData.AddDataSlice(ADDataType.Quaternion, currentTransformStateCache.rotation, (int)FrameActionID.SetRotation);
                            publishedTickData.AddDataSlice(ADDataType.Vector3, currentTransformStateCache.scale, (int)FrameActionID.SetScale);
                        }

                        // If the NetworkIdentity is a NetworkEntity with attached data for this frame, attach it.
                        if(polledEntityADFrameCache.TryGetValue(identityFrameData.Key, out ArbitraryData data))
                        {
                            publishedTickData.slices.AddRange(data.slices);
                        }

                        if(identityFrameData.Value.data.slices.Count != 0)
                        {
                            publishedTickData.slices.AddRange(identityFrameData.Value.data.slices);
                        }
                    }
                }

                // Reset the polled entity information cache for the next frame to use.
                polledEntityADFrameCache = new();
            }

            // Send to players. Probably best not to do this here- Figure out the delays and stuff 11/4/2022, 11/10/2022
            foreach (var NetIDNetIdentityPair in NetworkServer.connections)
            {
                TargetSendTickRpc(NetIDNetIdentityPair.Value, publishedTickData.Packed);
            }
        }



        /// <summary>
        /// Used by NetworkEntities after being polled. Extracts information from the entity provided and adds it to the current frame's cache.
        /// </summary>
        [Server]
        public static void UploadEntityInformationToCache(NetworkEntity entity, ArbitraryData data)
        {
            Instance.polledEntityADFrameCache[entity.netIdentity] = data;
        }



        [Server]
        public static void QueueSpecialEntityAction(NetworkEntity entity, SpecialEntityActions action)
        {
            Instance.AddEntityAction(0, entity, action);
        }



        [Server]
        public static void QueueSpecialEntityAction(int framesInFuture, NetworkEntity entity, SpecialEntityActions action)
        {
            Instance.AddEntityAction(framesInFuture, entity, action);
        }



        /// <summary>
        /// Adds an action to be commited by a networkEntity in the future on the server.
        /// </summary>
        [Server]
        private void AddEntityAction(int framesInFuture, NetworkEntity entity, SpecialEntityActions action)
        {
            // FramesInFuture must be at least one frame in the future
            if (framesInFuture <= 0) { framesInFuture = 1; }
            TickTime time = new(globalTick, globalFrame + framesInFuture);

            // Make sure that the serverQueuedActions variable exists first
            if (!serverQueuedActions.TryGetValue(time.Ticks, out var tickEntityData))
            {
                tickEntityData = serverQueuedActions[time.Ticks] = new();
            }
            if (!tickEntityData.TryGetValue(time.Frames, out var frameEntityData))
            {
                frameEntityData = tickEntityData[time.Frames] = new();
            }
            if (!frameEntityData.TryGetValue(entity, out ArbitraryData entityData))
            {
                entityData = frameEntityData[entity] = new();
            }

            // Add ArbitraryData based on action parameter
            switch (action)
            {
                case SpecialEntityActions.Kill:
                    entityData.AddEmptyDataSlice((int)SpecialEntityActions.Kill);
                    break;
            }
        }



        // ================================================================================================================================================================================================
        // Client Functions
        // ================================================================================================================================================================================================

        /// <summary>
        /// Sends a tick, in arbitrary data form, to a client which then stores the tick in its cache.
        /// </summary>
        [TargetRpc]
        private void TargetSendTickRpc(NetworkConnection target, byte[] arbitraryData)
        {
            // Create a new arbitrary data from the received data
            ArbitraryData receivedData = new(arbitraryData);
            Tick tickToStore = new();

            // Get the tick of the arbitrary data, which should be the first value
            ushort _tick = receivedData.slices[0].DataTag == ADDataTag.Two ? receivedData.slices[0].GetElementAsUShort() : throw new Exception("Error reading received Tick. Could not read time value of tick.");

            // Unpack the tick
            byte frame = 0;
            NetworkIdentity selectedIdentity = null;
            for (int sliceIndex = 1; sliceIndex < receivedData.slices.Count; sliceIndex++)  // i = 0, the first slice, signifies the tick number- skip it
            {
                // If the slice's data type is a Net ID, set as the selected identity and continue
                if (receivedData.slices[sliceIndex].DataTag == ADDataTag.NetIDMarker)
                {
                    if (NetworkClient.spawned.TryGetValue(receivedData.slices[sliceIndex].GetElementAsUInt(), out selectedIdentity))
                    {
                        // If receiving packets of own player, skip for now; in the future, check if the position lines up with what the client predicted it was on this tick
                        if (selectedIdentity == NetworkClient.localPlayer)
                        {
                            sliceIndex = GetIndexOfNextIdentityOrFrame(sliceIndex) - 1;
                            selectedIdentity = null;
                            continue;
                        }
                        // Otherwise just store the identity in the cache and use it for future data
                        else
                        {
                            tickToStore.frameDataOfIdentities[frame][selectedIdentity] = new(new(null, null, null));
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Identity no longer exists. Skipping to next identity in packet...");

                        // Subtract one from this because it goes to next iteration no matter what
                        sliceIndex = GetIndexOfNextIdentityOrFrame(sliceIndex) - 1;
                    }
                }

                else if (receivedData.slices[sliceIndex].DataTag == ADDataTag.FrameMarker)
                {
                    frame++;
                    selectedIdentity = null;
                    if (frame == framesPerTick) { throw new Exception($"Error reading received tick, slice {sliceIndex}. Tried to advance to a frame that shouldn't exist (Frame {frame})."); }
                }

                // ==========================================
                // PAST THIS POINT, ASSUME IT'S IDENTITY DATA
                // ==========================================

                // If there isn't a selected identity to begin with, something is wrong.
                else if (selectedIdentity == null) { throw new Exception($"Error reading received tick, slice {sliceIndex}. Tried to attach an attribute without specifying an identity first."); }

                // If the network identity doesn't exist on the client, skip it.
                else if (!NetworkClient.spawned.ContainsValue(selectedIdentity))
                {
                    Debug.LogError("Couldn't find the cached network identity on the client. Skipping for now.");
                }

                // Otherwise, read the ID of the data and do stuff with it.
                else
                {
                    FrameData frameData = tickToStore.frameDataOfIdentities[frame][selectedIdentity];
                    ArbitraryDataSlice slice = receivedData.slices[sliceIndex];
                    switch (slice.ID)
                    {
                        case (int)FrameActionID.SetPosition:
                            frameData.nTransformState.position = receivedData.slices[sliceIndex].GetElementAsVector3();
                            break;
                        case (int)FrameActionID.SetRotation:
                            frameData.nTransformState.rotation = receivedData.slices[sliceIndex].GetElementAsQuat();
                            break;
                        case (int)FrameActionID.SetScale:
                            frameData.nTransformState.scale = receivedData.slices[sliceIndex].GetElementAsVector3();
                            break;
                        default:
                            Debug.Log($"Unpacking a slice representing {(FrameActionID)slice.ID}");
                            frameData.data.slices.Add(slice);
                            break;
                    }
                }
            }

            clientCachedTicks[_tick] = tickToStore;

            // Returns the index of the next occurence of a network identity or frame marker in the received data, starting at a given index.
            int GetIndexOfNextIdentityOrFrame(int currentSliceIndex)
            {
                // Need to use +1 so that it doesn't stop on the current slice, which is a nonexistent NetID
                int newSliceIndex = currentSliceIndex + 1;

                while (newSliceIndex < receivedData.slices.Count && receivedData.slices[newSliceIndex].DataTag != ADDataTag.NetIDMarker && receivedData.slices[newSliceIndex].DataTag != ADDataTag.FrameMarker)
                {
                    newSliceIndex++;
                }

                return newSliceIndex;
            }
        }



        /// <summary>
        /// Runs a frame, received from the server and having already been unpacked on the client, on the client.
        /// </summary>
        [Client]
        private void ClientRunFrame(ushort tick, byte frame)
        {
            GameObject gameObjectCache = null;
            NullableTransformState transformStateCache;

            if (!clientCachedTicks.ContainsKey(tick))
            {
                Debug.LogWarning($"Missing client tick (Tick {tick}).");
                return;
            }
            if (clientCachedTicks[tick].frameDataOfIdentities == null)
            {
                Debug.LogWarning($"Missing client frame {tick}:{frame}.");
                return;
            }

            foreach (var identityFramePair in clientCachedTicks[tick].frameDataOfIdentities[frame])
            {
                // Check if this is being run on the local player
                if (identityFramePair.Key == NetworkClient.localPlayer)
                {
                    // Don't set personal player stuff yet, implement this later
                    continue;
                }

                // Do not run position/rotation/scale setting data on the server
                if (!isServer)
                {
                    gameObjectCache = identityFramePair.Key.gameObject;
                    transformStateCache = identityFramePair.Value.nTransformState;

                    // Set position, rotation and scale if they were set
                    if (transformStateCache.position != null)
                    {
                        if (identityFramePair.Key.gameObject.TryGetComponent(out NetworkEntity entity))
                        {
                            // lerped position doesn't really do anything differently than setting position directly, needs to be fixed later 11/10/2022
                            entity.ClientSetNewPosition((Vector3)transformStateCache.position);
                        }
                        else
                        {
                            gameObjectCache.transform.position = (Vector3)transformStateCache.position;
                        }
                    }
                    if (transformStateCache.rotation != null)
                    {
                        gameObjectCache.transform.rotation = (Quaternion)transformStateCache.rotation;
                    }
                    if (transformStateCache.scale != null)
                    {
                        gameObjectCache.transform.localScale = (Vector3)transformStateCache.scale;
                    }
                }

                // Run any extra data attached to the identity/entity
                foreach(ArbitraryDataSlice slice in identityFramePair.Value.data.slices)
                {
                    NetworkIdentity identity = identityFramePair.Key;
                    PlayPropertySliceOnIdentity(identity, slice);
                }
            }
        }



        /// <summary>
        /// Uploads all inputs this tick to the server.
        /// </summary>
        [Client]
        private void ClientUploadInput()
        {
            NetworkedInputPacket[] packetsToSend = new NetworkedInputPacket[framesPerTick];
            for (int i = 0; i < framesPerTick; i++)
            {
                packetsToSend[i] = new NetworkedInputPacket(clientFrameInputs[i]);
            }
            CmdUploadInput(packetsToSend);
        }
    }



    #region Helper Enums

    /// <summary>
    /// A table for what an ID within arbitrary data means for a NetworkIdentity.
    /// </summary>
    // An "X" means that the ID hasn't actually been implemented yet.
    public enum FrameActionID : int
    {
        // 0 -> 255 = High Priority IDs, 1 byte

        SetPosition = 0,                // Position
        SetRotation = 1,                // Quaternion   
        SetScale = 2,                   // Vector3
        SetVerticalLookRotation = 3,    // Byte (360/256)
        PlayKillAnimation = 4,          // Empty
        LongChildMarker = 5,            // UShort                       X
        SetHealth = 7,                  // Float                        X
        StartStand = 8,                 // Empty                        X
        StartCrouch = 9,                // Empty                        X
        StartProne = 10,                // Empty                        X
        Hurt = 12,                      // Empty                        X
        Heal = 13,                      // Empty                        X
        PlaySFXOnIdentity = 15,         // Int (ID)                     X
        SpawnProjectileByID = 16,       // Int

        // 256 -> 65535 = Medium Priority IDs, 2 bytes

        SetDamageable = 256,            // Bool                         X
        SetKillable = 257,              // Bool                         X 
        SetHealable = 258,              // Bool                         X
        ChangeHeldItemID = 259,         // UShort (ID)                  X
        PlayJumpAnimation = 260,        // Empty                        X
        PlayEmote = 261,                // Byte ID                      X

        // -2147483648 -> -1, 65536 -> 2147483647 = Low Priority IDs, 4 bytes

        Error = -1                      // N/A
    }



    public enum SpecialEntityActions : int
    {
        Kill = 0
    }

    #endregion Helper Enums



    #region Helper Classes and Structs

    public class Tick
    {
        public List<Dictionary<NetworkIdentity, FrameData>> frameDataOfIdentities = new();

        public Tick()
        {
            for(int i=0; i<TickManager.framesPerTick; i++)
            {
                frameDataOfIdentities.Add(new());
            }
        }
    }



    public class FrameData
    {
        public NullableTransformState nTransformState = new(null, null, null);
        public ArbitraryData data = new();

        public FrameData(NullableTransformState _nTransformState)
        {
            nTransformState = _nTransformState;
        }
    }



    public struct NullableTransformState
    {
        public Vector3? position;
        public Quaternion? rotation;
        public Vector3? scale;

        public NullableTransformState(Vector3? _position, Quaternion? _rotation, Vector3? _scale)
        {
            position = _position;
            rotation = _rotation;
            scale = _scale;
        }

        public NullableTransformState(Transform t)
        {
            position = t.position;
            rotation = t.rotation;
            scale = t.localScale;
        }
    }



    public struct TickTime
    {
        private ushort ticks;
        public ushort Ticks
        {
            get
            {
                return ticks;
            }
            set
            {
                ticks = Ticks;
            }
        }

        private byte frames;
        public byte Frames
        {
            get
            {
                return frames;
            }
            set
            {               
                if (Frames < 0)
                {
                    frames = 0;
                }
                else
                {
                    if(frames >= TickManager.framesPerTick)
                    {
                        frames = (byte)(Frames % TickManager.framesPerTick);
                        if (frames < Frames)
                        {
                            ticks += (ushort)((Frames - frames) / TickManager.framesPerTick);
                        }
                    }
                    else
                    {
                        frames = Frames;
                    }
                }
            }
        }

        public int TotalFrames
        {
            get
            {
                return ticks * TickManager.framesPerTick + frames;
            }
            set
            {
                if(TotalFrames < 0)
                {
                    ticks = 0;
                    frames = 0;
                }
                else
                {
                    frames = (byte)(TotalFrames % TickManager.framesPerTick);
                    ticks = (ushort)((TotalFrames - frames) / TickManager.framesPerTick);
                }
            }
        }

        public TickTime(int _frames)
        {
            ticks = 0;
            frames = (byte)(_frames % TickManager.framesPerTick);
            ticks = (ushort)((_frames - frames) / TickManager.framesPerTick);
        }

        public TickTime(ushort _ticks)
        {
            ticks = _ticks;
            frames = 0;
        }

        public TickTime(ushort _ticks, int _frames)
        {
            ticks = _ticks;
            frames = (byte)(_frames % TickManager.framesPerTick);
            ticks += (ushort)((_frames - frames) / TickManager.framesPerTick);
        }

        public TickTime(float time, bool roundUp = true)
        {
            float ret = time / Time.fixedDeltaTime;
            this = new TickTime((int)(roundUp ? Math.Ceiling(ret) : Math.Floor(ret)));
        }
    }

    #endregion Helper Classes and Structs
}
