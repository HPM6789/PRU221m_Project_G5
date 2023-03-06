using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTrigger
{
    public bool TriggerRepeatedly => true;

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) < 10)
        {
            player.Charactor.Animator.IsMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.LongGrass);
        }  
    }
}
