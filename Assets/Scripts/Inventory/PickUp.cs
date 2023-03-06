using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] ItemBase item;

    public bool Used { get; set; } = false;

    

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            string name = initiator.GetComponent<PlayerController>().Name;

            AudioManager.Instance.PlaySfx(AudioID.Pickup, pauseMusic: true);

            yield return DialogManager.Instance.ShowDialogText($"{name} found {item.Name}");
        }
    }
    public object CaptureState()
    {
        return Used;
    }
    public void RestoreState(object state)
    {
        Used = (bool)state;

        if (Used)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
