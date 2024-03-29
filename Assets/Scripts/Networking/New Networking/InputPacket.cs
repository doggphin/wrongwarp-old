using UnityEngine;

namespace WrongWarp
{
    public class InputPacket
    {
        public Vector2 lookVector;
        /// <summary>
        /// 0. W        <para></para>
        /// 1. A        <para></para>
        /// 2. S        <para></para>
        /// 3. D        <para></para>
        /// 4. Run      <para></para>
        /// 5. Crouch   <para></para>
        /// 6. Prone    <para></para>
        /// 7. Jump
        /// </summary>
        public byte inputByte1;
        /// <summary>
        /// 0. Attack   <para></para>
        /// 1. Interact <para></para>
        /// 2.          <para></para>
        /// 3.          <para></para>
        /// 4.          <para></para>
        /// 5.          <para></para>
        /// 6.          <para></para>
        /// 7.
        /// </summary>
        public byte inputByte2;

        //// ID 0: byte, x/255 percentage between inputpackets attacked
        //// ID 1: Vector3, direction looking when attacked
        //// ID 2: byte, x/255 percentage between inputpackets interacted
        //// ID 3: Vector3, direction looking when interacted
        //// ID 4: byte, switching hotbar selection
        //public byte[] packedData = null;

        public ArbitraryData data = new();

        public void SetInputValue(InputButton input, bool valueToWrite)
        {
            int inputButtonValue = (int)input % 8;
            if ((int)input < 8)
            {
                inputByte1 = WWBinaryFunctions.WriteBit(inputByte1, inputButtonValue, valueToWrite);
            }
            else if ((int)input < 16)
            {
                inputByte2 = WWBinaryFunctions.WriteBit(inputByte2, inputButtonValue, valueToWrite);
            }
            else
            {
                throw new System.Exception("Invalid input value.");
            }
            
        }

        public bool ReadInputValue(InputButton input)
        {
            int inputButtonValue = (int)input % 8;
            if((int)input < 8)
            {
                return WWBinaryFunctions.ReadBit(inputByte1, inputButtonValue);
            }
            else if((int)input < 16)
            {
                return WWBinaryFunctions.ReadBit(inputByte2, inputButtonValue);
            }
            throw new System.Exception("Invalid input value.");
        }

        public enum InputButton : int
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3,
            Run = 4,
            Crouch = 5,
            Prone = 6,
            Jump = 7,
            Attack = 8,
            Interact = 9
        }

        public InputPacket(NetworkedInputPacket inputPacket)
        {
            lookVector = inputPacket.lookVector;
            inputByte1 = inputPacket.inputByte1;
            inputByte2 = inputPacket.inputByte2;
            data = new ArbitraryData(inputPacket.packedData);
        }

        public void Reset()
        {
            data = new();
        }

        // Empty constructor, maybe modify in the future
        public InputPacket() { }

        public bool HasInputs => inputByte1 != 0 || inputByte2 != 0 || data.slices.Count !=0;
    }

    public struct NetworkedInputPacket
    {
        public Vector2 lookVector;
        public byte inputByte1;
        public byte inputByte2;
        public byte[] packedData;

        public NetworkedInputPacket(InputPacket inputPacket)
        {
            lookVector = inputPacket.lookVector;
            inputByte1 = inputPacket.inputByte1;
            inputByte2 = inputPacket.inputByte2;
            packedData = inputPacket.data.Packed;
        }
    }
}
