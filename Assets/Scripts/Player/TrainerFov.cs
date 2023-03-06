using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTrigger
{
    public bool TriggerRepeatedly => false; 

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Charactor.Animator.IsMoving = false;
        GameController.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}
