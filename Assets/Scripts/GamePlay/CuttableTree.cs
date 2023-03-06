using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("This Tree looks like can be cut");

        var fakemonWithCut = initiator.GetComponent<FakemonParty>().Fakemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Cut"));

        if (fakemonWithCut != null)
        {
            int choiceSelected = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {fakemonWithCut.Base.Name} use cut?",
                choices: new List<string>() { "Yes", "No", },
                onChoiceSelected: (selection) => choiceSelected = selection
                );

            if(choiceSelected == 0)
            {
                //Yes
                yield return DialogManager.Instance.ShowDialogText($"{ fakemonWithCut.Base.Name} use cut!");
                gameObject.SetActive(false);
            }
        }
    }
}
