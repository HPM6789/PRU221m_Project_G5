using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cutsence))]
public class CutsenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutsence = target as Cutsence;

        if (GUILayout.Button("Add Dialog Action"))
            cutsence.AddAction(new DialogAction());
        else if (GUILayout.Button("Add Move Actor Action"))
            cutsence.AddAction(new MoveActorAction());
        else if (GUILayout.Button("Add Turn Actor Action"))
            cutsence.AddAction(new TurnActorAction());
        else if (GUILayout.Button("Add Teleport Action"))
            cutsence.AddAction(new TeleportObjectAction());
        else if (GUILayout.Button("Add Enable Object Action"))
            cutsence.AddAction(new EnableObjectAction());
        else if (GUILayout.Button("Add Disable Object Action"))
            cutsence.AddAction(new DiableObjectAction());
        else if (GUILayout.Button("Add NPC Interact Action"))
            cutsence.AddAction(new NPCInteractAction());
        else if (GUILayout.Button("Add Fade In Action"))
            cutsence.AddAction(new FadeInAction());
        else if (GUILayout.Button("Add Fade Out Action"))
            cutsence.AddAction(new FadeOutAction());
        base.OnInspectorGUI();
    }
}
