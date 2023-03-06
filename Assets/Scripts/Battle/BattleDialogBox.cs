using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Text> actionText;
    [SerializeField] List<Text> moveText;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
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

    public void EnableDialogText(bool enable)
    {
        dialogText.enabled = enable;
    }

    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    public void EnableChoiceBox(bool enable)
    {
        choiceBox.SetActive(enable);
    }

    public void EnableMoveSelector(bool enable)
    {
        moveSelector.SetActive(enable);
        moveDetails.SetActive(enable);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionText.Count; i++)
        {
            if (i == selectedAction)
            {
                actionText[i].color = Color.blue;
            }
            else
            {
                actionText[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveText.Count; i++)
        {
            if (i == selectedMove)
            {
                moveText[i].color = Color.blue;
            }
            else
            {
                moveText[i].color = Color.black;
            }
        }
        ppText.text = $"PP {move.PP}/{move.Base.Pp}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else
            ppText.color = Color.black;
    }
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveText.Count; i++)
        {
            if (i < moves.Count)
            {
                moveText[i].text = moves[i].Base.Name;
            }
            else
            {
                moveText[i].text = "-";
            }
        }
    }

    public void UpdateChoiceBox(bool selector)
    {
        if (selector)
        {
            yesText.color = Color.blue;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = Color.blue;
        }
    }
}
