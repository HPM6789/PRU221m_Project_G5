using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    Charactor charactor;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] Sprite sprite;
    [SerializeField] string name;

    [SerializeField] AudioClip trainerAppearClip;

    public Sprite Sprite { get { return sprite; } }
    public string Name { get { return name; } }

    bool battleLost = false;
    private void Awake()
    {
        charactor = GetComponent<Charactor>();
    }
    private void Start()
    {
        SetFovRotation(charactor.Animator.DefaultDirection); 
    }
    private void Update()
    {
        charactor.HandleUpdate();
    }
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        AudioManager.Instance.PlayerMusic(trainerAppearClip, true, false);

        //Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //Walk toward the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return charactor.Move(moveVec);

        //Show dialog
        yield return DialogManager.Instance.ShowDialog(dialog);

        GameController.Instance.StartTrainerBattle(this);
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Up)
            angle = 90f;
        else if (dir == FacingDirection.Left)
            angle = 180f;
        else if (dir == FacingDirection.Down)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public IEnumerator Interact(Transform initiator)
    {
        charactor.LookToward(initiator.position);

        if (!battleLost)
        {
            AudioManager.Instance.PlayerMusic(trainerAppearClip, true, false);

            yield return DialogManager.Instance.ShowDialog(dialog);
            GameController.Instance.StartTrainerBattle(this);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
        
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
        {
            fov.gameObject.SetActive(false);
        }
    }
}
