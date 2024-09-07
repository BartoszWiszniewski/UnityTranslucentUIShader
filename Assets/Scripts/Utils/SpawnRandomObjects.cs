using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utils
{
    [ExecuteAlways]
    [RequireComponent(typeof(AssignRandomMaterials))]
    [RequireComponent(typeof(BoxCollider))]
    public class SpawnRandomObjects : MonoBehaviour
    {
        public List<GameObject> prefabs = new List<GameObject>();
        public int targetObjects = 30;
        public BoxCollider boxCollider;
        public AssignRandomMaterials assignRandomMaterials;

        private void Awake()
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
                boxCollider.size = Vector3.one * targetObjects/2;
            }

            if (assignRandomMaterials == null)
            {
                assignRandomMaterials = GetComponent<AssignRandomMaterials>();
            }
            
        }

        private void OnEnable()
        {
            if(prefabs == null || prefabs.Count == 0)
            {
                Debug.LogError("No prefabs assigned to " + gameObject.name);
                return;
            }
            
            var childCount = transform.childCount;
            if (childCount < targetObjects)
            {
                var toSpawn = targetObjects-childCount;
                for (int i = 0; i < toSpawn; i++)
                {
                    var randomIndex = UnityEngine.Random.Range(0, prefabs.Count);
                    var randomPrefab = prefabs[randomIndex];
                    var randomPosition = new Vector3(
                        UnityEngine.Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x),
                        UnityEngine.Random.Range(boxCollider.bounds.min.y, boxCollider.bounds.max.y),
                        UnityEngine.Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z)
                    );
                    var randomRotation = Quaternion.Euler(
                        UnityEngine.Random.Range(0, 360),
                        UnityEngine.Random.Range(0, 360),
                        UnityEngine.Random.Range(0, 360)
                    );
                    var newObject = Instantiate(randomPrefab, randomPosition, randomRotation, transform);
                    newObject.name = randomPrefab.name;

                    assignRandomMaterials.Randomize();
                }
            }
        }
    }
}
