using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveForget ,Busy}
public class InventoryUI : MonoBehaviour
{

    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;


    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    Inventory inventory;
    int selectedItem = 0;
    int selectedCategory = 0;
    MoveBase moveToLearn;   


    List<ItemSlotUI> slotUiList;
    RectTransform itemListRect;
    const int itemsInViewPoint = 8;

    InventoryUIState state;

    Action<ItemBase> onItemUsed;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        //Clear all item
        foreach(Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        slotUiList = new List<ItemSlotUI>();
        foreach (var itemSLot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotObject = Instantiate(itemSlotUI, itemList.transform);
            slotObject.SetData(itemSLot);

            slotUiList.Add(slotObject);
        }

        UpdateSelection();
    }
    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        if(state == InventoryUIState.ItemSelection)
        {
            this.onItemUsed = onItemUsed;

            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++selectedCategory;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --selectedCategory;
            }
            if (selectedCategory > Inventory.ItemCategies.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategies.Count - 1;


            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);
            if(prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategies[selectedCategory];

                UpdateItemList();
            }else if (prevSelection != selectedItem)
            {
                UpdateSelection();
            }
                

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine( ItemSelected());
            } 
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }else if(state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                //Use Item on Selected Fakemon
                StartCoroutine(UseItem());
            };

            Action onBacked = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBacked);
        }else if(state == InventoryUIState.MoveForget)
        {
            Action<int> onMovedSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
                ClosePartyScreen();
            };

            moveSelectionUI.HandleMoveSelection(onMovedSelected);
        }
        
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

        var item = inventory.GetItem(selectedItem, selectedCategory);
        var fakemon = partyScreen.SelectedMember;
        {
            var evolution = fakemon.CheckForEvolution(item);

            if(evolution != null)
            {
                yield return EvolutionManager.Instance.Evolve(fakemon, evolution);
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText($"It's won't have any effect");
                ClosePartyScreen();
                yield break;
            }
        }

        var useItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if(useItem != null)
        {
            if( useItem is RecoveryItem )
                yield return DialogManager.Instance.ShowDialogText($"The Player Used {useItem.Name}");
            onItemUsed?.Invoke(useItem);
            ClosePartyScreen();
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect");
            ClosePartyScreen();
        }
    }

    void UpdateSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

        for (int i = 0; i < slotUiList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUiList[i].NameText.color = Color.blue;
                slotUiList[i].CountText.color = Color.blue;
            }
            else
            {
                slotUiList[i].NameText.color = Color.black;
                slotUiList[i].CountText.color = Color.black;
            }
        }


        if(slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }


        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUiList.Count <= itemsInViewPoint) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPoint/2, 0, selectedItem) * slotUiList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem;

        if(tmItem == null) { yield break; }

        var fakemon = partyScreen.SelectedMember;

        if (fakemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{fakemon.Base.Name} already know {tmItem.Move.Name}", false);
            yield break;
        }

        if (!tmItem.CanBeTaught(fakemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{fakemon.Base.Name} can't learn {tmItem.Move.Name}", false);
            yield break;
        }

        if(fakemon.Moves.Count < 4)
        {
            fakemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{fakemon.Base.Name} learned {tmItem.Move.Name}", false);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{fakemon.Base.Name} is trying to learn {tmItem.Move.Name}", false);
            
            yield return ChooseMoveToForget(fakemon, tmItem.Move);

            yield return new WaitUntil(() => state != InventoryUIState.MoveForget);

            yield return new WaitForSeconds(2f);
        }
    }
    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;
        var item = inventory.GetItem(selectedItem, selectedCategory);


        if(GameController.Instance.State == GameState.Shop)
        {
            onItemUsed?.Invoke(item);
            state = InventoryUIState.ItemSelection;
            yield break;
        }
        if(GameController.Instance.State == GameState.Battle)
        {
            if (!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item cannot be use in batlle", false);
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            if (!item.CanUseOutSide)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item cannot be use outside batlle", false);
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        if(selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
            if(item is TmItem)
            {
                partyScreen.ShowIfTmIsUsable(item as TmItem);
            }
        }
    }
    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;

        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.ClearTMUsable();
        partyScreen.gameObject.SetActive(false);
    }

    void ResetSelection()
    {
        selectedItem = 0;

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    IEnumerator ChooseMoveToForget(Fakemon fakemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"Choose new Move you want to forget", true, false);

        moveSelectionUI.gameObject.SetActive(true);

        moveSelectionUI.SetMoveData(fakemon.Moves.Select(x => x.Base).ToList(), newMove);

        moveToLearn = newMove;

        state = InventoryUIState.MoveForget;
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var fakemon = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();

        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == 4)
        {
            yield return(DialogManager.Instance.ShowDialogText($"{fakemon.Base.Name} did not learn {moveToLearn.Name}", false));
        }
        else
        {
            var selectedMove = fakemon.Moves[moveIndex].Base;
            yield return(DialogManager.Instance.ShowDialogText($"{fakemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}", false));
            fakemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}
