using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(CutsenceActor))]
public class CutSenceActorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
       //position = EditorGUI.PrefixLabel(position, label);

       // var togglePos = new Rect(position.x, position.y, 70, position.height);
       // var fieldPos = new Rect(position.x + 100, position.y, position.width - 100, position.height);

       // var isPlayer = property.FindPropertyRelative("isPlayer");

       // isPlayer.boolValue = GUI.Toggle(togglePos, isPlayer.boolValue, "Is Player");
       // isPlayer.serializedObject.ApplyModifiedProperties();

       // if(!isPlayer.boolValue)
       //     EditorGUI.PropertyField(fieldPos, property.FindPropertyRelative("charactor"), GUIContent.none);

       // EditorGUI.EndProperty();
        base.OnGUI(position, property, label);
    }
}
