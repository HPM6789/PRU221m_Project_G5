using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTrigger
{

    bool isJumpingToWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("The Water is deep blue");
        var animator = initiator.GetComponent<CharactorAnimator>();

        if (animator.IsSurfing || isJumpingToWater)
        {
            yield break; 
        }
        var fakemonWithSurf = initiator.GetComponent<FakemonParty>().Fakemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Surf"));

        if (fakemonWithSurf != null)
        {
            int choiceSelected = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {fakemonWithSurf.Base.Name} use surd?",
                choices: new List<string>() { "Yes", "No", },
                onChoiceSelected: (selection) => choiceSelected = selection
                );

            if (choiceSelected == 0)
            {
                //Yes
                yield return DialogManager.Instance.ShowDialogText($"{ fakemonWithSurf.Base.Name} use surf!");
                
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();

                isJumpingToWater = false;
                animator.IsSurfing = true;
            }
        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) < 10)
        {
            GameController.Instance.StartBattle(BattleTrigger.Water);
        }
    }
}
