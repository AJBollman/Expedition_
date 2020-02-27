using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Expedition))]
public class ExpeditionEditor : Editor
{
    private Dictionary<string, bool> list = new Dictionary<string, bool>() {
        {"Overlay", false},
        {"World", false},
        {"Plains", false},
        {"GlowCaves", false},
        {"FoggyForest", false},
        {"CrystalCaves", false},
        {"Shipwreck", false},
        {"prefabbin", false}
    };



    public override void OnInspectorGUI() {
        Expedition instance = (Expedition)target;

        foreach(KeyValuePair<string, bool>i in list) {

            if(GUILayout.Button( (i.Value ? "Unload " : "Load " ) + i.Key )) {
                instance.LoadBiomeInEditor(i.Key, i.Value);
                list[i.Key] = !list[i.Key];
            }

        }

        DrawDefaultInspector();
    }
}
