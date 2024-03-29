using System;
using UnityEngine;
using Mirror;
using System.Linq;

namespace WrongWarp
{
    public static class WWRuneSpellConverter
    {
        [Server]
        public static byte[] CountRunes(ServerInventory inventory)
        {
            return CountRunes(inventory.items);
        }

        [Server]
        public static byte[] CountRunes(ServerSimpleItem[] items)
        {
            byte[] data = new byte[4];
            foreach(var item in items)
            {
                // Item IDs that aren't between 6 and 12 are not runes
                if(item.id < 5 || item.id > 12)
                {
                    continue;
                }

                // 5, 6 -> data[0]
                // 7, 8 -> data[1]
                // 9, 10 -> data[2]
                // 11, 12 -> data[3]

                // In 4-bit binary representation of ID counts,
                // byte[4] data = { (5, 6), (7, 8), (9, 10), (11, 12) };
                data[(item.id - 5) / 2] += (byte)(item.id % 2 == 1 ? 1 : 16);
            }
            return data;
        }

        [Server]
        public static byte[] EncryptCountedRunes(byte[] runeCounts)
        {
            // Hash runecounts into a 64 character array
            var hash = new Hash128();
            hash.Append(runeCounts);
            char[] hashChars = hash.ToString().ToCharArray();

            byte[] returnData = new byte[0];
            for (int i = 0; i < 4; i++)
            {
                returnData.Concat(BitConverter.GetBytes(hashChars[i]));
            }
            return returnData;
        }

        // Used on both client and server
        //public static ProjectileData ConvertBytesToSpell(byte[] encryptedRuneCounts)
        //{
        //    return new ProjectileData();
        //}
    }
}
