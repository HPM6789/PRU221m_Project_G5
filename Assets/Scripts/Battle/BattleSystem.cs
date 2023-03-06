using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using Assets.Scripts.Fakemons.TakeDameStrategy;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurns, Busy, PartyScreen, BattleOver,AboutToUse, MoveToForget, Bag }
public enum BattleAction { Move, SwitchFakemon, UseItem, Run }
public enum BattleTrigger { LongGrass, Water}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox battleDialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip battleVictoryMusic;

    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite grassBackground;
    [SerializeField] Sprite waterBackground;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    bool aboutToUseChoice=true;

    FakemonParty fakemonParty;
    FakemonParty trainerParty;
    Fakemon wildFakemon;

    bool isTrainerBattle = false;

    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;
    MoveBase moveToLearn;

    BattleTrigger battleTrigger;
    public void StartBattle(FakemonParty fakemonParty, Fakemon wildDakemon, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        this.fakemonParty = fakemonParty;
        this.wildFakemon = wildDakemon;
        player = fakemonParty.GetComponent<PlayerController>();

        isTrainerBattle = false;

        battleTrigger = trigger;

        AudioManager.Instance.PlayerMusic(wildBattleMusic, true, false);

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(FakemonParty fakemonParty, FakemonParty trainerParty, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        this.fakemonParty = fakemonParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;

        battleTrigger=trigger;

        AudioManager.Instance.PlayerMusic(trainerBattleMusic, true, false);

        player = fakemonParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }else if(state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }else if(state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };
            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if(state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if(moveIndex == 4)
                {
                    StartCoroutine(battleDialogBox.TypeDialog($"{playerUnit.Fakemon.Base.Name} did not learn {moveToLearn.Name}"));
                }
                else
                {
                    var selectedMove = playerUnit.Fakemon.Moves[moveIndex].Base;
                    StartCoroutine(battleDialogBox.TypeDialog($"{playerUnit.Fakemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}"));
                    playerUnit.Fakemon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.RunningTurns;
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.CLear();
        enemyUnit.CLear();

        backgroundImage.sprite = (battleTrigger == BattleTrigger.LongGrass) ? grassBackground : waterBackground;
        if (!isTrainerBattle)
        {
            playerUnit.SetUp(fakemonParty.GetHealthyFakemon());
            enemyUnit.SetUp(wildFakemon);

            battleDialogBox.SetMoveNames(playerUnit.Fakemon.Moves);
            //battleDialogBox.SetDialog($"A Wild  {enemyUnit.Fakemon.Base.Name} appeared.");

            yield return battleDialogBox.TypeDialog($"A Wild  {enemyUnit.Fakemon.Base.Name} appeared.");
        }
        else
        {
            //Show character
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return battleDialogBox.TypeDialog($"{trainer.Name} wants to battle");

            //Send out fakemon of trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);

            var enemyFakemon = trainerParty.GetHealthyFakemon();
            enemyUnit.SetUp(enemyFakemon);

            yield return battleDialogBox.TypeDialog($"{trainer.Name} send out {enemyFakemon.Base.Name}");


            //Send out fakemon of player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);

            var playerFakemon = fakemonParty.GetHealthyFakemon();
            playerUnit.SetUp(playerFakemon);

            yield return battleDialogBox.TypeDialog($"Go {playerFakemon.Base.Name}");
            battleDialogBox.SetMoveNames(playerUnit.Fakemon.Moves);
        }
        escapeAttempts = 0;

        partyScreen.Init();
        

        ActionSelection();
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        battleDialogBox.SetDialog("Choose an Action");
        battleDialogBox.EnableActionSelector(true);
    }
    void OpenPartyScreen()
    {
        partyScreen.CallFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        battleDialogBox.EnableDialogText(false);
        battleDialogBox.EnableActionSelector(false);
        battleDialogBox.EnableMoveSelector(true);
    }
    IEnumerator AboutToUse(Fakemon newFakemon)
    {
        state = BattleState.Busy;
        yield return battleDialogBox.TypeDialog($"{trainer.name} is about to use {newFakemon.Base.Name}. Do you want to change Fakemon?");
        state = BattleState.AboutToUse;
        battleDialogBox.EnableChoiceBox(true);
    }

    void BattleOver(bool battleOver)
    {
        state = BattleState.BattleOver;
        fakemonParty.Fakemons.ForEach(f => f.OnBattleOver());
        playerUnit.Hud.CLearData();
        enemyUnit.Hud.CLearData();

        OnBattleOver(battleOver);
    }


    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurns;
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Fakemon.CurrentMove = playerUnit.Fakemon.Moves[currentMove];
            enemyUnit.Fakemon.CurrentMove = enemyUnit.Fakemon.GetRandomMove();

            var playerMovePiority = playerUnit.Fakemon.CurrentMove.Base.Piority;
            var enemyMovePiority = enemyUnit.Fakemon.CurrentMove.Base.Piority;


            //Check Which go first
            bool playerGoesFirst = true;

            if (enemyMovePiority > playerMovePiority)
                playerGoesFirst = false;
            else if (enemyMovePiority == playerMovePiority)
                playerGoesFirst = playerUnit.Fakemon.Speed >= enemyUnit.Fakemon.Speed;  

            var firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
            var secondaryUnit = playerGoesFirst ? enemyUnit : playerUnit;
            var secondaryFakemon = secondaryUnit.Fakemon;
            //First Turn
            yield return RunMove(firstUnit, secondaryUnit, firstUnit.Fakemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;
            

            Debug.Log("State: " + state);
            Debug.Log("HP: " + secondaryFakemon.HP);
            if(secondaryFakemon.HP > 0)
            {
                yield return RunMove(secondaryUnit, firstUnit, secondaryUnit.Fakemon.CurrentMove);
                yield return RunAfterTurn(secondaryUnit);
                if (state == BattleState.BattleOver) yield break;
            }
            //Secondary Turn
        }
        else
        {
            if (playerAction == BattleAction.SwitchFakemon)
            {
                var selectedFakemon = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchFakemon(selectedFakemon);
            }
            if (playerAction == BattleAction.UseItem)
            {
                //This is handle from item screen, so do nothing and skip to enemy move
                battleDialogBox.EnableActionSelector(false);
                //yield return ThrowPokeball();
            }
            if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            //Enemy Turn
            var enemyMove = enemyUnit.Fakemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;

            
        }

        if (state != BattleState.BattleOver) ActionSelection();
    }
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);
        battleDialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                OpenBag();
               //StartCoroutine( RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 2)
            {
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }


    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Fakemon.Moves.Count - 1)
            {
                ++currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                --currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Fakemon.Moves.Count - 2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Fakemon.Moves[currentMove];
            if (move.PP == 0) return;

            battleDialogBox.EnableMoveSelector(false);
            battleDialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            battleDialogBox.EnableMoveSelector(false);
            battleDialogBox.EnableDialogText(true);
            ActionSelection();
        }
        battleDialogBox.UpdateMoveSelection(currentMove, playerUnit.Fakemon.Moves[currentMove]);
    }

    void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            var selectMember = partyScreen.SelectedMember;
            if (selectMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted Fakemon");
                return;
            }
            if (selectMember == playerUnit.Fakemon)
            {
                partyScreen.SetMessageText("You can't switch with the same Fakemon");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CallFrom == BattleState.ActionSelection)
            {

                StartCoroutine(RunTurns(BattleAction.SwitchFakemon));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CallFrom == BattleState.AboutToUse;
                StartCoroutine(SwitchFakemon(selectMember, isTrainerAboutToUse));
            }
            partyScreen.CallFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.Fakemon.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a fakemon to continue");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CallFrom == BattleState.AboutToUse)
            {

                StartCoroutine(SendNextTraineerFakemon());
            }
            else
            {
                ActionSelection();
            }
            partyScreen.CallFrom = null;
        };
        partyScreen.HandleUpdate(onSelected, onBack);
    }
    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
    }
    IEnumerator ShowDamageDetail(Fakemon.DamageDetail damageDetail)
    {
        if (damageDetail.Critical > 1)
        {
            yield return battleDialogBox.TypeDialog("A Critical hit");
        }
        if (damageDetail.TypeEffectiveness > 1)
        {
            yield return battleDialogBox.TypeDialog("It's super effective");
        }
        else if(damageDetail.TypeEffectiveness < 1)
        {
            yield return battleDialogBox.TypeDialog("It's not super effective");
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Fakemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChange(sourceUnit.Fakemon);
            yield return sourceUnit.Hud.WaitForHpUpdate();
            yield break;
        }
        yield return ShowStatusChange(sourceUnit.Fakemon);
        move.PP--;
        yield return battleDialogBox.TypeDialog($"{sourceUnit.Fakemon.Base.Name} used {move.Base.Name}");
        if (CheckIfMoveHits(move, sourceUnit.Fakemon, targetUnit.Fakemon))
        {
            sourceUnit.PlayerAttackAnimation();
            AudioManager.Instance.PlaySfx(move.Base.Sound);

            yield return new WaitForSeconds(1f);

            targetUnit.PlayerHitAnimcation();
            AudioManager.Instance.PlaySfx(AudioID.Hit);
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunEffectMove(move.Base.Effects, sourceUnit.Fakemon, targetUnit.Fakemon, move.Base.Target);
            }
            else
            {
                //var damageDetail = targetUnit.Fakemon.TakeDamage(move, sourceUnit.Fakemon);
                Fakemon.DamageDetail damageDetail = new Fakemon.DamageDetail();
                if(move.Base.Category == MoveCategory.Physical)
                {   
                    targetUnit.Fakemon.SetTakeDamageStrategy(new PhysicalAttackStrategy());                    
                }else if(move.Base.Category == MoveCategory.Special)
                {
                    targetUnit.Fakemon.SetTakeDamageStrategy(new SpecialAttackStrategy());
                }
                damageDetail = targetUnit.Fakemon.TakeDamageCal(move, sourceUnit.Fakemon);

                yield return targetUnit.Hud.WaitForHpUpdate();
                yield return ShowDamageDetail(damageDetail);

            }
            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Fakemon.HP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                    {
                        yield return RunEffectMove(secondary, sourceUnit.Fakemon, targetUnit.Fakemon, secondary.Target);
                    }
                }
            }
            if (targetUnit.Fakemon.HP <= 0)
            {
                yield return HandleFakemonFainted(targetUnit);
            }
        }
        else
        {
            yield return battleDialogBox.TypeDialog($"{sourceUnit.Fakemon.Base.Name}'s attack missed");
        }


    }
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurns);
        sourceUnit.Fakemon.OnAfterTurn();
        yield return ShowStatusChange(sourceUnit.Fakemon);
        yield return sourceUnit.Hud.WaitForHpUpdate();

        if (sourceUnit.Fakemon.HP <= 0)
        {
            yield return HandleFakemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurns); 
        }
    }
    IEnumerator RunEffectMove(MoveEffect effects, Fakemon source, Fakemon target, MoveTarget moveTarget)
    {
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoost(effects.Boosts);
            }
            else
            {
                target.ApplyBoost(effects.Boosts);
            }
        }
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);

        }
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);

        }
        yield return ShowStatusChange(source);
        yield return ShowStatusChange(target);
    }

    IEnumerator ShowStatusChange(Fakemon fakemon)
    {
        while (fakemon.StatusChanges.Count > 0)
        {
            var message = fakemon.StatusChanges.Dequeue();
            yield return battleDialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandleFakemonFainted(BattleUnit faintedUnit)
    {
        yield return battleDialogBox.TypeDialog($"{faintedUnit.Fakemon.Base.Name} Fainted");
        faintedUnit.PlayerFaintAnimation();

        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            bool battleWon = true;
            if (isTrainerBattle)
                battleWon = trainerParty.GetHealthyFakemon() == null;

            if (battleWon)
                AudioManager.Instance.PlayerMusic(battleVictoryMusic, true, false);


            //EXP gain
            int expYield = faintedUnit.Fakemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Fakemon.Level;

            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Fakemon.Exp += expGain;
            yield return battleDialogBox.TypeDialog($"{playerUnit.Fakemon.Base.Name} has gained {expGain} Exp");

            yield return playerUnit.Hud.SetExpSmooth();

            //Check levelup
            while (playerUnit.Fakemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return battleDialogBox.TypeDialog($"{playerUnit.Fakemon.Base.Name} grew to level {playerUnit.Fakemon.Level}");

                //Try to learn new move
                var newMove = playerUnit.Fakemon.GetLearnMove();
                if(newMove != null)
                {
                    if(playerUnit.Fakemon.Moves.Count < 4)
                    {
                        playerUnit.Fakemon.LearnMove(newMove.Base);
                        yield return battleDialogBox.TypeDialog($"{playerUnit.Fakemon.Base.Name} learned {newMove.Base.Name}");
                        battleDialogBox.SetMoveNames(playerUnit.Fakemon.Moves);
                    }
                    else
                    {
                        //Forgot Move to learn new move
                        yield return battleDialogBox.TypeDialog($"{playerUnit.Fakemon.Base.Name} tryong to learn {newMove.Base.Name}");
                        yield return battleDialogBox.TypeDialog($"But it cannot lean more than 4 moves");
                        yield return ChooseMoveToForget(playerUnit.Fakemon, newMove.Base);

                        yield return new WaitUntil(() => state != BattleState.MoveToForget);

                        yield return new WaitForSeconds(2f); 

                        
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }


            yield return new WaitForSeconds(1f);
        }
        CheckForBattleOver(faintedUnit);
    }
    IEnumerator ChooseMoveToForget(Fakemon fakemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return battleDialogBox.TypeDialog($"Choose new Move you want to forget");

        moveSelectionUI.gameObject.SetActive(true);

        moveSelectionUI.SetMoveData(fakemon.Moves.Select(x=>x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextFakemon = fakemonParty.GetHealthyFakemon();
            if (nextFakemon != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(true);
            }
        }
        else
        {
            if (!isTrainerBattle) { 
                BattleOver(true);
            }
            else
            {
                var nextFakemon = trainerParty.GetHealthyFakemon();
                if(nextFakemon != null)
                {
                    StartCoroutine(AboutToUse(nextFakemon));
                }
                else
                {
                    BattleOver(true);
                }
            }
        }
    }
    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        battleDialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            battleDialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTraineerFakemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            battleDialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTraineerFakemon());
        }

    }
    IEnumerator SwitchFakemon(Fakemon newFakemon, bool isTrainerAboutToUse = false)
    {
        bool currentFakemonFainted = true;
        if (playerUnit.Fakemon.HP > 0)
        {
            currentFakemonFainted = false;
            yield return battleDialogBox.TypeDialog($"Come back {playerUnit.Fakemon.Base.Name}");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.SetUp(newFakemon);
        playerUnit.Hud.SetData(newFakemon);
        battleDialogBox.SetMoveNames(newFakemon.Moves);

        yield return battleDialogBox.TypeDialog($"Go {newFakemon.Base.Name}");

        if (isTrainerAboutToUse)
        {
            StartCoroutine(SendNextTraineerFakemon());
        }
        else
        {
            state = BattleState.RunningTurns;
        }
    }
    bool CheckIfMoveHits(Move move, Fakemon source, Fakemon target)
    {
        if (move.Base.AlwaysHits) return true;
        float moveAccuracy = move.Base.Accuracy;
        int accuracy = source.StatBoost[Stat.Accuracy];
        int evasion = target.StatBoost[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        if (evasion > 0)
        {
            moveAccuracy /= boostValues[-evasion];
        }
        else
        {
            moveAccuracy *= boostValues[evasion];
        }
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator SendNextTraineerFakemon()
    {
        state = BattleState.Busy;

        var nextFakemon = trainerParty.GetHealthyFakemon();
        enemyUnit.SetUp(nextFakemon);

        yield return battleDialogBox.TypeDialog($"{trainer.Name} send out {nextFakemon.Base.Name}");

        state = BattleState.RunningTurns;
    }
    IEnumerator OnItemUsed(ItemBase usedItem)
    {

        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is PokeBallItem)
        {
            yield return ThrowPokeball((PokeBallItem)usedItem);
        }

        StartCoroutine(RunTurns(BattleAction.UseItem));
    }
    IEnumerator ThrowPokeball(PokeBallItem pokeBallItem)
    {
        state = BattleState.Busy;
        if (isTrainerBattle)
        {
            yield return battleDialogBox.TypeDialog($"You can't steal the trainers Fakemon");
            state = BattleState.RunningTurns;
            yield break;
        }

        yield return battleDialogBox.TypeDialog($"{player.Name} used {pokeBallItem.Name.ToUpper()}!");

       var pokeballObject = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2,0), Quaternion.identity);

        var pokeball = pokeballObject.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeBallItem.Icon;

        //Animation
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();

        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchFakemon(enemyUnit.Fakemon, pokeBallItem);
        for (int i = 0; i < Mathf.Min(shakeCount,3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if(shakeCount == 4)
        {
            //Caught
            yield return battleDialogBox.TypeDialog($"{enemyUnit.Fakemon.Base.Name} was caught");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            fakemonParty.AddFakemon(enemyUnit.Fakemon);
            yield return battleDialogBox.TypeDialog($"{enemyUnit.Fakemon.Base.Name} has add to your Party");
            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);

            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return battleDialogBox.TypeDialog($"{enemyUnit.Fakemon.Base.Name} broke free");
            else
            {
                yield return battleDialogBox.TypeDialog($"Almost caught it");
            }

            Destroy(pokeball);
            state = BattleState.RunningTurns;
        }
    }

    int TryToCatchFakemon(Fakemon fakemon, PokeBallItem pokeBallItem)
    {
        if(pokeBallItem.Name == "PokeBall")
        {
            pokeBallItem.SetCatchStrategy(new PokeBallCatchStrategy());
        }
        else
        {
            pokeBallItem.SetCatchStrategy(new GreatBallCatchStrategy());
        }
        float a = (float)pokeBallItem.GetCatchBaseRate(fakemon);
        //float a = (3 * fakemon.MaxHp - 2 * fakemon.HP) * fakemon.Base.CatchRate * pokeBallItem.CatchRateModifier *ConditionDB.GetStatusBonus(fakemon.Status) / (3 * fakemon.MaxHp);
        if (a >= 255) return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while(shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;
            ++shakeCount;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        if (isTrainerBattle)
        {
            yield return battleDialogBox.TypeDialog("You can't run from a trainer battle");
            state = BattleState.RunningTurns;
            yield break;
        }
        ++escapeAttempts;

        int playerSpeed = playerUnit.Fakemon.Speed;
        int enemySpeed = enemyUnit.Fakemon.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return battleDialogBox.TypeDialog("Run succesfully");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            if(UnityEngine.Random.Range(0,256) < f)
            {
                yield return battleDialogBox.TypeDialog("Run away safely");
                BattleOver(true);
            }
            else
            {
                yield return battleDialogBox.TypeDialog("Can't escape");
                state=BattleState.RunningTurns;
            }
        }
    }
}
