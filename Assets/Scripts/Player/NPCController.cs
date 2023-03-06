using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Dialog dialog;
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;
    [SerializeField] List<Vector2> movementPatter;
    [SerializeField] float timeBetweenPattern;

    Charactor charactor;
    ItemGiver itemGiver;
    FakemonGiver fakemonGiver;
    Healer healer;
    Merchant merchant;

    float idleTimer = 0f;
    NPCState state;
    int currentPattern;
    Quest activeQuest;

    private void Awake()
    {
        charactor = GetComponent<Charactor>();
        itemGiver = GetComponent<ItemGiver>();
        fakemonGiver = GetComponent<FakemonGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
    }
    public IEnumerator Interact(Transform initiator)
    {
        if(state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            if(charactor != null)
            {
                charactor.LookToward(initiator.position);
            }

            if(questToComplete != null)
            {
              var quest =  new Quest(questToComplete);
                yield return quest.CompleteQuest(initiator);
                questToComplete = null;
            }

            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            } else if (fakemonGiver != null && fakemonGiver.CanBeGiven())
            {
                yield return fakemonGiver.GiveFakemon(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;

                if (activeQuest.CanBeComplete())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeComplete())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialog);
                }
            } else if (healer != null)
            {
                yield return healer.Heal(initiator, dialog);
            }else if(merchant != null)
            {
                yield return merchant.Trade();
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }
            idleTimer = 0f; 
            state =NPCState.Idle;

        }
    }
    private void Update()
    {
        if (charactor != null)
        {
            if (state == NPCState.Idle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > timeBetweenPattern)
                {
                    idleTimer = 0f;
                    if (movementPatter.Count > 0)
                        StartCoroutine(Walk());
                }
            }

            charactor.HandleUpdate();
        }
            
    }
    IEnumerator Walk()
    {
        state = NPCState.Walking;
        var oldPos = transform.position;


        yield return charactor.Move(movementPatter[currentPattern]);
        if(transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPatter.Count;

        state = NPCState.Idle;
    }

    public object CaptureState()
    {
        var saveData = new NPCQuestSAveData();
        saveData.activeQuest = activeQuest?.GetSaveData();
        if (questToStart != null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();

        if(questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
       var saveData = state as NPCQuestSAveData;
        if(saveData != null)
        {
            activeQuest = (saveData.activeQuest != null) ? new Quest(saveData.activeQuest) : null;
            questToStart = (saveData.questToStart != null) ? new Quest(saveData.questToStart).Base : null;
            questToComplete = (saveData.questToComplete != null) ? new Quest(saveData.questToComplete).Base : null;


        }
    }
}

[System.Serializable]
public class NPCQuestSAveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}

public enum NPCState { Idle, Walking, Dialog}
