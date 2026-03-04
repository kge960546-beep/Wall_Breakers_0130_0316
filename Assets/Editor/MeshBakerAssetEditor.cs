using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshBakeAsset))]
public class MeshBakerAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MeshBakeAsset script = (MeshBakeAsset)target;

        GUILayout.Space(20);
        GUI.backgroundColor = Color.cyan;

        if(GUILayout.Button("Bake Level & Save Asset", GUILayout.Height(40)))
        {
            script.BakeLevel();
            EditorUtility.SetDirty(script.gameObject);
        }
    }
}
