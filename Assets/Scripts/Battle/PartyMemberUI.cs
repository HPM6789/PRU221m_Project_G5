using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Text messageText;

    Fakemon _fakemon;

    public void Init(Fakemon fakemon)
    {
        _fakemon = fakemon;

        UpdateData();
        SetMessage("");
        _fakemon.OnHPChanging += UpdateData;
    }

    void UpdateData()
    {
        nameText.text = _fakemon.Base.Name;
        levelText.text = "Lvl " + _fakemon.Level;
        hpBar.SetHP((float)_fakemon.HP / _fakemon.MaxHp);
    }
    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = Color.blue;
        }
        else
        {
            nameText.color = Color.black;
        }
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
