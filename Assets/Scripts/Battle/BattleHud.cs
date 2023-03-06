using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;
    [SerializeField] Color slpColor;


    Fakemon fakemon;
    Dictionary<ConditionID, Color> statusColor;
    public void SetData(Fakemon _fakemon)
    {

        if (_fakemon != null)
        {
            _fakemon.OnHPChanging -= UpdateHP;
            _fakemon.OnStatusChanged -= SetStatusText;
        }

        fakemon = _fakemon;
        nameText.text = fakemon.Base.Name;
        levelText.text = "Lvl " + fakemon.Level;
        SetLevel();
        hpBar.SetHP((float)fakemon.HP / fakemon.MaxHp);
        SetExp();
        statusColor = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor },
            {ConditionID.slp, slpColor },

        };
        SetStatusText();
        fakemon.OnStatusChanged += SetStatusText;
        fakemon.OnHPChanging += UpdateHP;

    }
    public void SetStatusText()
    {
        if (fakemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = fakemon.Status.Id.ToString().ToUpper();
            var colorStatus = statusColor[fakemon.Status.Id];
            statusText.color = new Color(colorStatus.r, colorStatus.g, colorStatus.b);
        }
    }
    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }
    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)fakemon.HP / fakemon.MaxHp);
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float nomalize = GetNormalizeExp();

        expBar.transform.localScale = new Vector3(nomalize, 1, 1);
    }
    public void SetLevel()
    {
        levelText.text = "Lvl" + fakemon.Level;
    }
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float nomalize = GetNormalizeExp();

        yield return expBar.transform.DOScaleX(nomalize, 1.5f).WaitForCompletion();
    }
    float GetNormalizeExp()
    {
        int curLevelExp = fakemon.Base.GetExpForLevel(fakemon.Level);
        int nextLevelExp = fakemon.Base.GetExpForLevel(fakemon.Level + 1);

        float nomalize = (float)(fakemon.Exp - curLevelExp) / (nextLevelExp - curLevelExp);
        return Mathf.Clamp01(nomalize);
    }
    public IEnumerator WaitForHpUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void CLearData()
    {
        if(fakemon != null)
        {
            fakemon.OnHPChanging -= UpdateHP;
            fakemon.OnStatusChanged -= SetStatusText;
        }
    }
}
