using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour, ISavable
{

    public event Action onEncounter;
    public event Action<Collider2D> onEnterTrainerView;

    private Vector2 input;
    private Charactor charactor;

    [SerializeField] Sprite sprite;
    [SerializeField] string name;

    public Sprite Sprite { get { return sprite; } }
    public string Name { get { return name; } }

    public Charactor Charactor { get { return charactor; } }

    public static PlayerController Instance { get; private set; }



    private void Awake()
    {
        Instance = this;
        charactor = GetComponent<Charactor>();
    }
    public void HandleUpdate()
    {
        if (!charactor.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            if (input.x != 0) input.y = 0;
            if (input != Vector2.zero)
            {
                StartCoroutine(charactor.Move(input, OnMoveOver));
            }
        }

        charactor.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine (Interact());
    }
    IEnumerator Interact()
    {
        var facingDir = new Vector3(charactor.Animator.MoveX, charactor.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayer.i.InteractableLayersLayer | GameLayer.i.WaterLayer);
        if(collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    IPlayerTrigger currentlyInTrigger;
    void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, 0.3f), 0.2f, GameLayer.i.TriggerableLayer);

        IPlayerTrigger triggerable = null;
        foreach(var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTrigger>();
            if(triggerable != null)
            {

                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;
                
                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if(colliders.Count() == 0 || triggerable != currentlyInTrigger)
        {
            currentlyInTrigger = null;
        }
    }

    private void CheckIfInTrainerView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayer.i.FovLayer);
        if (collider != null)
        {
            charactor.Animator.IsMoving = false;
            onEnterTrainerView?.Invoke(collider);
        }
    }

    public object CaptureState()
    {
        var save = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },

            fakemons = GetComponent<FakemonParty>().Fakemons.Select(f=>f.GetSaveData()).ToList(),
        };

        return save;
    }

    public void RestoreState(object state)
    {
        var save = (PlayerSaveData)state;

        var position = (float[])save.position;
        transform.position = new Vector3(position[0], position[1]);

        GetComponent<FakemonParty>().Fakemons =  save.fakemons.Select(f => new Fakemon(f)).ToList();
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;

    public List<FakemonSaveData> fakemons;
}