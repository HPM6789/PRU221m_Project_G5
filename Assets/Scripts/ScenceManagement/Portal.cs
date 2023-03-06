using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Portal : MonoBehaviour, IPlayerTrigger
{
    [SerializeField] int senceToLoad = -1;
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
        StartCoroutine(SwitchSence());
    }

    IEnumerator SwitchSence()
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        yield return SceneManager.LoadSceneAsync(senceToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        playerController.Charactor.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatedly => false;
}
public enum DestinationIdentifier
{
    A,B,C,D,E
}