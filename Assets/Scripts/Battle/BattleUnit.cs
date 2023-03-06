using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] FakemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    Image image;
    Vector3 originalPos;
    Color originalColor;

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public BattleHud Hud
    {
        get { return hud; }
    }
    private void Awake()
    {
       image = GetComponent<Image>();
       originalPos = image.transform.localPosition;
       originalColor = image.color;
    }
    public Fakemon Fakemon { get; set; }
    public void SetUp(Fakemon fakemon)
    {
        Fakemon = fakemon;
        if (isPlayerUnit)
            image.sprite = Fakemon.Base.BackSprite;
        else
            image.sprite = Fakemon.Base.FrontSprite;
        hud.gameObject.SetActive(true);
        hud.SetData(fakemon);

        transform.localScale = new Vector3(1, 1, 1);
        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void CLear()
    {
        hud.gameObject.SetActive(false);
    }
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayerAttackAnimation()
    {
        var seq = DOTween.Sequence();
        if (isPlayerUnit)
        {
            seq.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            seq.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }

        seq.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayerHitAnimcation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.DOColor(Color.gray, 0.1f));
        seq.Append(image.DOColor(originalColor, 0.1f));
    }
    
    public void PlayerFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        seq.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.DOFade(0, 0.5f));
        seq.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        seq.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return seq.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.DOFade(1, 0.5f));
        seq.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        seq.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return seq.WaitForCompletion();
    }
}
