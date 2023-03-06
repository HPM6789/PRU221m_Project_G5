using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int totalChance = serializedObject.FindProperty("totalChance").intValue;

        GUILayout.Label($"Total Chance = {totalChance}");
    }
}
