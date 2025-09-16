using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Tools;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Code.Scripts.Level
{
    public class ParticleDensityController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float density = 5;
        [SerializeField] private int perTileEmission = 1;
        [SerializeField] private Vector2 scalingOffset = Vector2.zero;
        
        [Header("References")]
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("DebugItems")] 
        [SerializeField] private int particleQty;
        [SerializeField] private float tileQty;

#if UNITY_EDITOR
        [InspectorButton("UpdateDensity")]
        private void UpdateSettings()
        {
            tileQty = spriteRenderer.size.x * spriteRenderer.size.y;
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            ParticleSystem.MainModule mainModule = particleSystem.main;
            ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
            
            mainModule.maxParticles = Mathf.RoundToInt(density * tileQty);
            emission.rateOverTime = perTileEmission * tileQty;
            shapeModule.scale = new Vector3(spriteRenderer.size.x + scalingOffset.x, spriteRenderer.size.y + scalingOffset.y, 1);
            EditorUtility.SetDirty(gameObject);
        }

        private void Update()
        {
            particleQty = particleSystem.particleCount;
        }

        [MenuItem("Tools/Custom/Generate All Particle Densities")]
        private static void UpdateAll()
        {
            List<ParticleDensityController> particleDensityControllers = new();
            
            if (PrefabStageUtility.GetCurrentPrefabStage())
            {
                particleDensityControllers = PrefabStageUtility.GetCurrentPrefabStage().FindComponentsOfType<ParticleDensityController>().ToList();
            }
            else if (Selection.gameObjects.Length > 0 && Selection.gameObjects.Any(item => item.GetComponentInChildren<ParticleDensityController>(true)))
            {
                foreach (GameObject go in Selection.gameObjects)
                    particleDensityControllers.AddRange(go.GetComponentsInChildren<ParticleDensityController>(true));
            }
            else
            {
                //In Scene Mode not selecting anything
                particleDensityControllers = FindObjectsOfType<ParticleDensityController>(true).ToList();
            }

            foreach (ParticleDensityController particleDensityController in particleDensityControllers)
                particleDensityController.UpdateSettings();
        }
#endif
    }
}
