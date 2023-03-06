using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
   public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;
       yield return DialogManager.Instance.ShowDialog(dialog, new List<string>() { "Yes", "No"},
           (choiceIndex)=> selectedChoice = choiceIndex);

        if(selectedChoice == 0)
        {
            yield return Fader.Instance.FadeIn(0.5f);
            var playerParty = player.GetComponent<FakemonParty>();
            playerParty.Fakemons.ForEach(e => e.Heal());

            playerParty.PartyUpdated();
            yield return Fader.Instance.FadeOut(0.5f);

            yield return DialogManager.Instance.ShowDialogText($"Your Fakemon will be fully healed now");
        }
        else if(selectedChoice ==1)
        {
            yield return DialogManager.Instance.ShowDialogText($"Okay! Come back if you change your mind");
        }
        
    }
}
