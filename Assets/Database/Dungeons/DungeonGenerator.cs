using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    public class DungeonGenerator : MonoBehaviour
    {
        private static DungeonGenerator instance;
        public static DungeonGenerator Instance => instance;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void GenerateDungeon(Vector3 position, Quaternion rotation, Vector3 maxSize, EEBlockSet[] blockSets, EEBlock startingBlock)
        {
            List<EEBlock> allBlocks = new();
            if(blockSets.Length == 0)
            {
                Debug.LogError("Cannot create a dungeon without EEBlockSets.");
                return;
            }
            foreach(EEBlockSet blockSet in blockSets)
            {
                foreach(EEBlock block in blockSet.blocks)
                {
                    allBlocks.Add(block);
                }
            }

            HashSet<int> blockIndicesUsed = new();
            List<GeneratedBlock> blocksToPopulate = new();

             

            while(blocksToPopulate.Count != 0)
            {
                for(int i=0; i<blocksToPopulate[0].connections.Length; i++)
                {

                }
            }

            GeneratedBlock GenerateBlock()
            {
                return new GeneratedBlock();
            }
        }
    }

    public struct GeneratedBlock
    {
        public GameObject gameObject;
        //public GameObject doors;
        //public GameObject[] prefabs;
        public EEBlock block;
        public GeneratedBlock?[] connections;

        public GeneratedBlock(GameObject _gameObject, EEBlock _block)
        {
            gameObject = _gameObject;
            block = _block;
            connections = new GeneratedBlock?[_block.connections.Length];
        }
    }

    public struct DungeonRecipe
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 maxSize;
        public List<EEBlock> blockSet;
        public EEBlock startingBlock;

        public DungeonRecipe(Vector3 _position, Quaternion _rotation, Vector3 _maxSize, EEBlockSet[] _blockSet, EEBlock _startingBlock)
        {
            position = _position;
            rotation = _rotation;
            maxSize = _maxSize;
            blockSet = new();
            foreach(EEBlockSet set in _blockSet)
            {
                foreach(EEBlock block in set.blocks)
                {
                    blockSet.Add(block);
                }
            } 
            startingBlock = _startingBlock;
        }
    }
}
