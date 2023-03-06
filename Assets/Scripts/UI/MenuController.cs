using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;

    List<Text> menuItems;
    public event Action<int> onMenuSelected;
    public event Action onBack;

    int selectedItem = 0;
    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
    }
    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateSelection();
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }
    public void HandleUpdate()
    {
        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count() - 1);

        if(prevSelection != selectedItem)
        UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onMenuSelected?.Invoke(selectedItem);
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            CloseMenu();
        }
    }

    void UpdateSelection()
    {
        for(int i = 0; i < menuItems.Count(); i++)
        {
            if(i == selectedItem)
            {
                menuItems[i].color = Color.blue;
            }
            else
            {
                menuItems[i].color = Color.black;
            }
        }
    }
}
