using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy}
public class ShopController : MonoBehaviour
{
    [SerializeField] Vector2 shopCameraOffset;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] CountSelectorUI countSelectorUI;


    public event Action OnStart;
    public event Action OnFinish;
    public static ShopController instance;
    Inventory inventory;
    ShopState state;

    Merchant merchant;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }
    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;

        OnStart?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("How may I serve you ",
            waitForInput: false,
            choices: new List<string>() { "Buy", "Sell", "Quit" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //Buy
            
            yield return GameController.Instance.MoveCamera(shopCameraOffset);
            walletUI.Show();
            shopUI.Show(merchant.AvailableItems, (item) =>
            {
                StartCoroutine(BuyItem(item));
            }, () => StartCoroutine(OnBackFromBuying()));
            state = ShopState.Buying;
        }
        else if (selectedChoice == 1)
        {
            //Sell
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
        }
        else if (selectedChoice == 2)
        {
            //Quit
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if(state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling, (selectedItem) =>
            {
                StartCoroutine(SellItem(selectedItem));
            });
        }else if(state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }

    IEnumerator SellItem(ItemBase itemSell)
    {
        state = ShopState.Busy;

        if (!itemSell.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("You can't sell that!");
            state = ShopState.Selling;
            yield break;
        }
        walletUI.Show();
        float sellingPrice = Mathf.Round(itemSell.Price/2);
        int countToSell = 1;

        int itemCount = inventory.GetItemCount(itemSell);
        if(itemCount > 1)
        {
            yield return DialogManager.Instance.ShowDialogText($"How many would you like to sell?", waitForInput: false, autoClose: false);

            yield return countSelectorUI.ShowSelector(itemCount, sellingPrice,
                (selectedCount) => countToSell = selectedCount
                );

            DialogManager.Instance.CloseDialog();
                
        }

        sellingPrice = sellingPrice * countToSell;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"I can give {sellingPrice} for that! Would you like to sell",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if(selectedChoice == 0)
        {
            //Yes
            inventory.RemoveItem(itemSell, countToSell);

            Wallet.Instance.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"Turned over {itemSell.Name} and receive {sellingPrice}");
        }

        walletUI.Close();
        state = ShopState.Selling;

    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;

        yield return DialogManager.Instance.ShowDialogText($"How Many would you want to buy", waitForInput:false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price, 
            (selectedCount) => countToBuy = selectedCount);

        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;
        if (Wallet.Instance.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"That wiil be {totalPrice}",
                waitForInput: false,
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
            if(selectedChoice == 0)
            {
                inventory.AddItem(item, countToBuy);
                Wallet.Instance.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText($"Thank For Shopping!");
            }
            else if(selectedChoice == 1)
            {

            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"You Don't have enough money!");
        }

        state = ShopState.Buying;
    }

    IEnumerator OnBackFromBuying()
    {
        yield return GameController.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}
