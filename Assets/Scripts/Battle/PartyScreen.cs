using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;
    List<Fakemon> fakemons;
    FakemonParty party;

    int selection = 0;
    public BattleState? CallFrom { get; set; }
    public Fakemon SelectedMember => fakemons[selection]; 
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        party = FakemonParty.GetPlayerParty();
        //foreach (Fakemon member in party.Fakemons)
        //{
        //    member.Init();
        //}

        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        fakemons = party.Fakemons;
        for(int i = 0; i < memberSlots.Length; i++)
        {
            if (i < fakemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(fakemons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateMemberSelection(selection);
        messageText.text = "Choose a Fakemon";
    }

    public void UpdateMemberSelection(int selectionMember)
    {
        for(int i = 0; i < fakemons.Count; i++)
        {
            if (i == selectionMember)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++selection;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --selection;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selection += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selection -= 2;
        }

        selection = Mathf.Clamp(selection, 0, fakemons.Count - 1);
        UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }
    public void ShowIfTmIsUsable(TmItem tmItem)
    {
        for(int i= 0; i<fakemons.Count; i++)
        {
            string message = tmItem.CanBeTaught(fakemons[i]) ? "ABLE!" : "Not ABLE!";
            memberSlots[i].SetMessage(message);
        }
    }

    public void ClearTMUsable()
    {
        for (int i = 0; i < fakemons.Count; i++)
        {
            memberSlots[i].SetMessage("");
        }
    }
}
