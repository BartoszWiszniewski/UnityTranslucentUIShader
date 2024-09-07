using UnityEngine;

namespace Utils
{
    [ExecuteAlways]
    public class AssignRandomMaterials : MonoBehaviour
    {
        public Material[] materials;

        private void OnEnable()
        {
            Randomize();
        }

        public void Randomize()
        {
            if(materials == null || materials.Length == 0)
            {
                Debug.LogError("No materials assigned to " + gameObject.name);
                return;
            }
            
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var targetRenderer in renderers)
            {
                targetRenderer.sharedMaterial = materials[Random.Range(0, materials.Length)];
            }
        }
    }
}
