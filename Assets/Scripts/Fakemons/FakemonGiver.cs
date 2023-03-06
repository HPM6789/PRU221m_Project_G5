using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] Fakemon fakemonToGive;
    [SerializeField] int count = 1;
    [SerializeField] Dialog dialog;


    bool used = false;
    public IEnumerator GiveFakemon(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
        fakemonToGive.Init();
        player.GetComponent<FakemonParty>().AddFakemon(fakemonToGive);

        used = true;

        AudioManager.Instance.PlaySfx(AudioID.Pickup, pauseMusic: true);

        string dialogText = $"{player.Name} received {fakemonToGive.Base.Name}";

        yield return DialogManager.Instance.ShowDialogText(dialogText);
    }

    public bool CanBeGiven()
    {
        return fakemonToGive != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
