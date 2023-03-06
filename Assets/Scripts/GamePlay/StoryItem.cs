using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTrigger
{
    [SerializeField] Dialog dialog;

    public bool TriggerRepeatedly => false;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Charactor.Animator.IsMoving = false;
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
