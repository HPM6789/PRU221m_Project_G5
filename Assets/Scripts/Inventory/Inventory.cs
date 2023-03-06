using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum ItemCategory { Items, Pokeballs, Tms}
public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> pokeballSlots;
    [SerializeField] List<ItemSlot> tmSlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;


    public static List<String> ItemCategies { get; set; } = new List<string>()
    {
        "ITEMS", "POKEBALLS", "TMs & HMs"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots };
    }
    public ItemBase UseItem(int indexItem, Fakemon selectedFakemon, int selectedCategory)
    {
        var item = GetItem(indexItem, selectedCategory);
       bool itemUsed = item.Use(selectedFakemon);
        if (itemUsed)
        {
            if(!item.IsReusable)
                RemoveItem(item);

            return item;
        }

        return null;
    }
    public void RemoveItem(ItemBase item, int count=1)
    {
        int category = (int) GetCategoryFromItem(item);
        var currentSlot = GetSlotsByCategory(category);
        var itemSlot = currentSlot.First(e => e.Item == item);

        itemSlot.Count -= count;
        if(itemSlot.Count == 0)
            currentSlot.Remove(itemSlot);

        OnUpdated?.Invoke();
    }
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var curentSlot = GetSlotsByCategory(categoryIndex);
        return curentSlot[itemIndex].Item;
    }

    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlot = GetSlotsByCategory(category);

       return currentSlot.Exists(slot => slot.Item == item);
    }

    public void AddItem(ItemBase item, int count=1)
    {
        int category = (int)GetCategoryFromItem(item);
        var curentSlot = GetSlotsByCategory(category);

        var itemSlot = curentSlot.FirstOrDefault(e => e.Item == item);
        if(itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            curentSlot.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();

    }

    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var curentSlot = GetSlotsByCategory(category);

        var itemSlot = curentSlot.FirstOrDefault(e => e.Item == item);
        if(itemSlot != null)
        {
            return itemSlot.Count;
        }else
        {
            return 0;
        }
    }
    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if(item is RecoveryItem || item is EvolutionItem)
        {
            return ItemCategory.Items;
        }else if(item is PokeBallItem)
        {
            return ItemCategory.Pokeballs;
        }
        else
        {
            return ItemCategory.Tms;
        }
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(e => e.GetSaveData()).ToList(),
            pokeBalls = pokeballSlots.Select(e => e.GetSaveData()).ToList(),
            tms = tmSlots.Select(e=>e.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        slots = saveData.items.Select(e => new ItemSlot(e)).ToList();
        pokeballSlots = saveData.pokeBalls.Select(e => new ItemSlot(e)).ToList();
        tmSlots = saveData.tms.Select(e => new ItemSlot(e)).ToList();

        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots };

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot 
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
            count = count
        };

        return saveData;
    }
    public ItemSlot() { }
    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }
    public ItemBase Item
    {
        get { return item; }
        set { item = value; }
    }
    public int Count
    {
        get { return count; }
        set { count = value; }
    }
}
[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData{
    public List<ItemSaveData> items;
    public List<ItemSaveData> pokeBalls;
    public List<ItemSaveData> tms;

}