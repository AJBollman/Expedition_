using UnityEngine;
using UnityEditor;


/// FROM THIS GUY /////////////////////////////
//https://forum.unity.com/threads/simple-way-to-edit-multiple-terrain-at-once.635080/
public class MultiTerrainEdit
    {
        [MenuItem("Tool/Multi terrain edit")]
        private static void SetDrawInstanced()
        {
            UnityEngine.Terrain[] terrains = Selection.activeGameObject.GetComponentsInChildren<UnityEngine.Terrain>();
            foreach (var terrain in terrains)
            {
                //terrain.drawInstanced = true;
                terrain.basemapDistance = 200;
                terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                terrain.detailObjectDensity = 0.5f;

            }
 
            Debug.Log($"Done. {terrains.Length} items.");
        }
    }