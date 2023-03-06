using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text countText;

    public Text NameText => nameText;
    public Text CountText => countText;

    public float Height => rectTransform.rect.height;

    RectTransform rectTransform;
    private void Awake()
    {
        
    }
    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.Item.name;
        countText.text = $"X {itemSlot.Count}";
    }

    public void SetNameAndPrice(ItemBase item)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = item.Name;
        countText.text = $"$ {item.Price}";
    }
}
