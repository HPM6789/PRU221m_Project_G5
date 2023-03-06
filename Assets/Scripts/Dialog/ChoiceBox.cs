using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choiceTextPrefabs;
    bool choiceSelected = false;
    List<ChoiceText> choiceTexts;
    int currentChoice;
    public IEnumerator ShowChoice(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;
        gameObject.SetActive(true);

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        choiceTexts = new List<ChoiceText>();
        foreach(var choice in choices)
        {
            var choiceTextObject = Instantiate(choiceTextPrefabs, transform);
            choiceTextObject.TextField.text = choice;
            choiceTexts.Add(choiceTextObject);
        }

        yield return new WaitUntil(() => choiceSelected == true);
        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++currentChoice;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            --currentChoice;
        }

        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (Input.GetKeyDown(KeyCode.Z))
            choiceSelected = true;
    }
}
