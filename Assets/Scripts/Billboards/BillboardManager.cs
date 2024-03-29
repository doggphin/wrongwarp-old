using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace WrongWarp
{
    public class BillboardManager : MonoBehaviour
    {
        public static BillboardManager Instance { get; private set; }
        public static Sprite TestSprite => Instance.testSprite;
        [SerializeField] private Sprite testSprite;
        [SerializeField] private Billboard billboardPrefab;
        private ObjectPool<Billboard> billboardPool;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            billboardPool = new(
                () => { return OnCreatePooledItem(); },
                billboard => { OnAccessFromPool(billboard); },
                billboard => { OnReleasedToPool(billboard); },
                billboard => { OnDestroyPooledObject(billboard); },
                true, 10, 100);
        }

        private Billboard OnCreatePooledItem()
        {
            return Instantiate(billboardPrefab);
        }

        private void OnAccessFromPool(Billboard billboard)
        {
            billboard.gameObject.SetActive(true);
            billboard.gameObject.transform.localScale = Vector3.one;
        }

        private void OnReleasedToPool(Billboard billboard)
        { 
            billboard.transform.parent = transform;
            billboard.gameObject.SetActive(false);
            billboard.gameObject.name = "Released Billboard";
        }

        private void OnDestroyPooledObject(Billboard billboard)
        {
            Destroy(billboard.gameObject);
        }

        public static Billboard CreateBillboard(Sprite sprite, Vector3 position, Vector3 upVector, float size = 1, float lifetime = 0)
        {
            return Instance.SpawnBillboard(sprite, null, position, upVector, size, lifetime);
        }

        public static Billboard CreateBillboard(Sprite sprite, Vector3 position, float size = 1, float lifetime = 0)
        {
            return Instance.SpawnBillboard(sprite, null, position, null, size, lifetime);
        }

        public static Billboard CreateBillboard(Sprite sprite, Transform transform, Vector3 upVector, float size = 1, float lifetime = 0)
        {
            return Instance.SpawnBillboard(sprite, transform, null, upVector, size, lifetime);
        }

        public static Billboard CreateBillboard(Sprite sprite, Transform transform, float size = 1, float lifetime = 0)
        {
            return Instance.SpawnBillboard(sprite, transform, null, null, size, lifetime);
        }

        private Billboard SpawnBillboard(Sprite sprite, Transform t, Vector3? position, Vector3? upVector, float size, float lifetime)
        {
            Debug.Log("Making a billboard");
            Billboard billboard = billboardPool.Get();
            if(t != null)
            {
                billboard.Init(t, upVector);
            }
            else
            {
                billboard.Init((Vector3)position, upVector);
            }
            billboard.SpriteRenderer.sprite = sprite;
            billboard.transform.localScale = new Vector3(size, size, size);
            Debug.Log(size);
            if (lifetime != 0) {
                StartCoroutine(DestroyBillboardAfterDelay(billboard, lifetime));
            }
            return billboard;
        }

        private static IEnumerator DestroyBillboardAfterDelay(Billboard billboard, float delay)
        {
            yield return WWHelpers.GetWait(delay);
            Debug.Log("Destroying billboard");
            Instance.billboardPool.Release(billboard);
        }
    }
}
