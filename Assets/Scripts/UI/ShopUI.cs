using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    List<ItemBase> avalableItems;
    List<ItemSlotUI> slotUIList;

    int selectedItem;
    RectTransform itemListRect;
    int itemsInViewPoint = 8; 

    Action<ItemBase> onItemSelectedAction;
    Action onBack;
    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected, Action onBack)
    {
        this.avalableItems = availableItems;
        gameObject.SetActive(true);
        UpdateItemList();

        this.onItemSelectedAction = onItemSelected;
        this.onBack = onBack;

    }
    public void HandleUpdate()
    {
        var prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, avalableItems.Count - 1);

        if(selectedItem != prevSelection)
        {
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onItemSelectedAction?.Invoke(avalableItems[selectedItem]);
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }

    }
    void UpdateItemList()
    {
        //Clear all item
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        slotUIList = new List<ItemSlotUI>();
        foreach (var item in avalableItems)
        {
            var slotObject = Instantiate(itemSlotUI, itemList.transform);
            slotObject.SetNameAndPrice(item);

            slotUIList.Add(slotObject);
        }

        UpdateSelection();
    }
    void UpdateSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, avalableItems.Count - 1);

        for (int i = 0; i < avalableItems.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].NameText.color = Color.blue;
                slotUIList[i].CountText.color = Color.blue;
            }
            else
            {
                slotUIList[i].NameText.color = Color.black;
                slotUIList[i].CountText.color = Color.black;
            }
        }


        if (avalableItems.Count > 0)
        {
            var item = avalableItems[selectedItem];
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }


        HandleScrolling();
    }
    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewPoint) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPoint / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
