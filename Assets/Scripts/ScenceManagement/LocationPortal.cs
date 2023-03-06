using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Teleport the player to a different position without switching scenes 
public class LocationPortal : MonoBehaviour, IPlayerTrigger
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destinationIdentifier;

    PlayerController playerController;
    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Charactor.Animator.IsMoving = false;
        this.playerController = player;
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        playerController.Charactor.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

    }

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatedly => false;
}
