using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cutsence : MonoBehaviour, IPlayerTrigger
{
    [SerializeReference]
    [SerializeField] List<CutsenceAction> actions;

    public bool TriggerRepeatedly => false;

    public IEnumerator Play()
    {
        GameController.Instance.StartCutsenceState();

        foreach(var action in actions)
        {
            if(action.WaitForComplete)
                yield return action.Play();
            else
                StartCoroutine(action.Play());
        }

        GameController.Instance.StartFreeRoamState();
    }
    public void AddAction(CutsenceAction action)
    {
//#if UNITY_EDITOR
//        Undo.RegisterCompleteObjectUndo(this, "Add action to cutscene.");
//#endif

        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Charactor.Animator.IsMoving = false;
        StartCoroutine(Play());
    }
}
