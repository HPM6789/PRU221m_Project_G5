using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    Action onDialogFinished;
    public static DialogManager Instance { get; private set; }

    public bool IsShowing { get; private set; }

    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true
        , List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        AudioManager.Instance.PlaySfx(AudioID.UISelect);

        yield return TypeDialog(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() =>
                Input.GetKeyDown(KeyCode.Z)
            );
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoice(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
        
    }
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices=null, Action<int> onChoiceSelected = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);
        
        foreach (var line in dialog.Lines)
        {
            AudioManager.Instance.PlaySfx(AudioID.UISelect);
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if(choices!=null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoice(choices, onChoiceSelected);
        }
        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }
    public void HandleUpdate()
    {
        
    }
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }
}
