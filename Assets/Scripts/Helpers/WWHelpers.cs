using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Linq;

namespace WrongWarp
{
    public static class WWHelpers
    {
        /// <summary>
        /// The camera currently being used by the player.
        /// </summary>
        public static Camera playerCamera;

        /// <summary>
        /// Removes a specified amount of values from a dictionary. By default removes all occurences.
        /// </summary>
        /// <param name="dictionary"> The dictionary to remove from. </param>
        /// <param name="valueToRemove"> The value to remove from the dictionary. </param>
        /// <param name="amountToRemove"> The amount to remove. By default is -1, which removes all of the value. </param>
        /// <returns> Returns how many values were removed. </returns>
        public static int RemoveDictionaryValues<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue valueToRemove, int amountToRemove = -1)
        {
            int amountRemoved = 0;
            if(amountToRemove < -1)
            {
                Debug.LogError("Cannot remove a negative amount of values from a dictionary.");
            }
            foreach(KeyValuePair<TKey, TValue> keyValuePair in dictionary)
            {
                if(keyValuePair.Value.Equals(valueToRemove))
                {
                    dictionary.Remove(keyValuePair.Key);
                    amountRemoved++;
                    if (amountToRemove != -1 && amountRemoved == amountToRemove)
                    {
                        break;
                    }
                }
            }
            return amountRemoved;
        }

        /// <summary>
        /// Removes the first occurence of a value in a dictionary.
        /// </summary>
        /// <param name="dictionary"> The dictionary to remove from. </param>
        /// <param name="valueToRemove"> The value to remove from the dictionary. </param>
        /// <returns> Returns whether a value was removed. </returns>
        public static bool RemoveFirstDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue valueToRemove)
        {
            return RemoveDictionaryValues(dictionary, valueToRemove, 1) == 1;
        }

        /// <summary>
        /// Stores WaitForSeconds values in a dictionary so not to allocate garbage.
        /// <para> Use when using the same WaitForSeconds values many times. </para>
        /// </summary>
        public static WaitForSeconds GetWait(float time)
        {
            if(waitDictionary.TryGetValue(time, out var wait)) { return wait; }

            waitDictionary[time] = new WaitForSeconds(time);
            return waitDictionary[time];
        }
        private static readonly Dictionary<float, WaitForSeconds> waitDictionary = new();

        /// <summary>
        /// Deletes all children under a given transform.
        /// </summary>
        /// <param name="t"></param>
        public static void DeleteChildren(Transform t)
        {
            foreach(Transform child in t)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        public static void SetChildrenLayers(Transform t, int layer, bool setParentLayer = true)
        {
            Transform[] transforms = t.GetComponentsInChildren<Transform>();
            if(setParentLayer)
            {
                transforms.Concat(new Transform[] { t });
            }
            SetTransformLayers(transforms, layer);
        }

        public static void SetTransformLayers(Transform[] transforms, int layer)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].gameObject.layer = layer;
            }
        }

        public static int CreateLayerMaskFromInts(int[] layers)
        {
            int layerMask = 0;
            for (int i=0; i<layers.Length; i++)
            {
                layerMask |= 1 << layers[i];
            }
            return layerMask;
        }

        public static int BoolToBinary(bool value)
        {
            return value == true ? 1 : 0;
        }
    }
}
