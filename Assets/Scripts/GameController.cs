using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscence, Paused, Menu, PartyScreen, Bag, Evolution, Shop }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    MenuController menuController;

    public SceneDetail CurrentScene { get; private set; }
    public SceneDetail PreviousScence { get; private set; }
    public static GameController Instance { get; private set; }

    GameState state;
    GameState prevState;
    GameState stateBeforEvolution;

    TrainerController trainer;

    public GameState State => state;
    private void Awake()
    {
        Instance = this;
        menuController = GetComponent<MenuController>();
        FakemonBaseDB.Init();
        MoveBaseDB.Init();
        ConditionDB.Init();
        ItemDB.Init();
        QuestDB.Init();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnDialogFinished += () =>
        {
            if (state == GameState.Dialog)
                state = prevState;
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;

        EvolutionManager.Instance.OnStartEvolution += () =>
        {
            stateBeforEvolution = state;
            state = GameState.Evolution;
        };
        EvolutionManager.Instance.OnCompleteEvolution += () => 
        {
            partyScreen.SetPartyData();
            state = stateBeforEvolution;

            AudioManager.Instance.PlayerMusic(CurrentScene.SenceMusic, true, true);

         };

        ShopController.instance.OnStart += () =>
        {
            state = GameState.Shop;
        };
        ShopController.instance.OnFinish += () =>
        {
            state = GameState.FreeRoam;
        };

    }
    public void StartBattle(BattleTrigger trigger)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var fakemonParty = playerController.GetComponent<FakemonParty>();
        var wildFakemon = CurrentScene.GetComponent<MapArea>().GetRandomWildFakemon(trigger);

        var refFakemon = new Fakemon(wildFakemon.Base, wildFakemon.Level);
        battleSystem.StartBattle(fakemonParty, refFakemon, trigger);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        this.trainer = trainer;
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var fakemonParty = playerController.GetComponent<FakemonParty>();
        var trainerParty = trainer.GetComponent<FakemonParty>();
        battleSystem.StartTrainerBattle(fakemonParty, trainerParty);
    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
            state = GameState.Cutscence;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }
    }

    public void StartCutsenceState()
    {
        state = GameState.Cutscence;
    }

    public void StartFreeRoamState()
    {
        state = GameState.FreeRoam;
    }
    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        partyScreen.SetPartyData();

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(!won);
        worldCamera.gameObject.SetActive(won);

        var playerParty  = playerController.GetComponent<FakemonParty>();
        
        bool hasEvolutions = playerParty.CheckForEvolution();
        if(hasEvolutions)
            StartCoroutine( playerParty.RunEvolution());
        else
        {
            AudioManager.Instance.PlayerMusic(CurrentScene.SenceMusic, true, true);
        }
    }
    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }else if(state == GameState.Cutscence)
        {
            playerController.Charactor.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }else if(state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }else if(state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {

            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.Menu;
                
            };
             
            partyScreen.HandleUpdate(onSelected, onBack);
        }else if(state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.Menu;
            };

            inventoryUI.HandleUpdate(onBack);
        }else if(state == GameState.Shop)
        {
            ShopController.instance.HandleUpdate();
        }

        
    }

    public void SetCurrentScence(SceneDetail newScene)
    {
        PreviousScence = CurrentScene;
        CurrentScene = newScene;
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;
        }else if(selectedItem == 1)
        {
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            SavingSystem.i.Save("SaveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            SavingSystem.i.Load("SaveSlot1");
            state = GameState.FreeRoam;
        }

        
    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut = false)
    {
        yield return Fader.Instance.FadeIn(0.5f);


        worldCamera.transform.position += new Vector3(moveOffset.x, moveOffset.y);

        if (waitForFadeOut)
            yield return Fader.Instance.FadeOut(0.5f);
        else
            StartCoroutine(Fader.Instance.FadeOut(0.5f));
    }
}
