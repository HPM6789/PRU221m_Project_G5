using Assets.Scripts.Fakemons.TakeDameStrategy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Fakemon
{
    [SerializeField] FakemonBase _base;
    [SerializeField] int level;

    public Fakemon(FakemonBase pBase, int plevel)
    {
        _base = pBase;
        level = plevel;

        Init();
    }
    public Fakemon(FakemonSaveData saveData)
    {
        _base = FakemonBaseDB.GetObjectByName(saveData.name);

        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        if(saveData.statusId != null)
        {
            Status = ConditionDB.Conditions[saveData.statusId.Value];
        }
        else
        {
            Status = null;
        }

        Moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }
    public FakemonBase Base { 
        get { return _base; }
    }
    public int Level {
        get { return level; }
    }
    public int HP { get; set; }
    public int Exp { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoost { get; private set; }
    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; set; }

    public Queue<string> StatusChanges { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }


    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanging;


    private ITakeDamageStrategy takeDamageStrategy;
    public void Init()
    {
        

        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if(Moves.Count > 4)
                break;
        }
        CalculateStats();
        HP = MaxHp;
        Exp = Base.GetExpForLevel(Level);
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        int oldMaxHP = MaxHp;
        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + level;

        if(oldMaxHP != 0)
            HP += MaxHp - oldMaxHP;
    }
    int GetStat(Stat stat)
    {
        int startVal = Stats[stat];
        int boost = StatBoost[stat];

        var boostValue = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if(boost >= 0)
        {
            startVal = Mathf.FloorToInt(startVal * boostValue[boost]);
        }
        else
        {
            startVal = Mathf.FloorToInt(startVal / boostValue[-boost]);
        }

        return startVal;
    }
    void ResetStatBoost()
    {
        StatBoost = new Dictionary<Stat, int>() {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0 },
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    public void ApplyBoost(List<StatBoost> boosts)
    {
        foreach(var statBoost in boosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoost[stat] = Mathf.Clamp(StatBoost[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
            Debug.Log(boost + " " + stat);
        }
    }
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if(Status?.OnBeforeMove != null)
        {
            if( !Status.OnBeforeMove(this))
                canPerformMove = false;
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }
        return canPerformMove;
    }
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp
    {
        get; private set; 
    }

    public DamageDetail TakeDamage(Move move, Fakemon Attacker)
    {
        float critical = 1f;
        if(Random.value * 100f <= 6.25f)
        {
            critical = 2;
        }

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetail = new DamageDetail()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? Attacker.SpAttack : Attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifier = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * Attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;


        int damage = Mathf.FloorToInt(d * modifier);

        HP -= damage;
        if(HP < 0)
        {
            HP = 0;
            damageDetail.Fainted = true;
        }

        DecreaseHP(damage);

        return damageDetail;
    }

    public void SetTakeDamageStrategy(ITakeDamageStrategy value)
    {
        takeDamageStrategy = value;
    }

    public int GetDame(Move move, Fakemon attacker, Fakemon defender)
    {
        return takeDamageStrategy.DamageCalculation(move, attacker, defender);
    }

    public DamageDetail TakeDamageCal(Move move, Fakemon Attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2;
        }

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetail = new DamageDetail()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        int baseDamage = GetDame(move, Attacker, this);


        int damage = Mathf.FloorToInt(baseDamage * type * critical);

        HP -= damage;
        if (HP < 0)
        {
            HP = 0;
            damageDetail.Fainted = true;
        }

        DecreaseHP(damage);

        return damageDetail;
    }
    public FakemonSaveData GetSaveData()
    {
        var saveData = new FakemonSaveData()
        {
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };
        return saveData;
    }
    public Move GetRandomMove()
    {
        var moveWithPP = Moves.Where(e=>e.PP>0).ToList();
        var r = Random.Range(0, moveWithPP.Count);
        return moveWithPP[r];
    }
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) return;
        Status = ConditionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMassage}");

        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;
        VolatileStatus = ConditionDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMassage}");

    }
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }


    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChanging?.Invoke();
    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHPChanging?.Invoke();
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(Level + 1))
        {
            ++level;
            CalculateStats();
            return true;
        }
        else
        {
            return false;
        }
    }

    public LearnableMove GetLearnMove()
    {
        return Base.LearnableMoves.FirstOrDefault(e => e.Level == level);
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count > 4)
            return;
        Moves.Add(new Move(moveToLearn));
    }
    public void Heal()
    {
        HP = MaxHp;
        OnHPChanging?.Invoke();

        CureStatus();
    }

    public bool HasMove(MoveBase moveBase)
    {
        return Moves.Count(m => m.Base == moveBase) > 0;
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level);
    }

    public Evolution CheckForEvolution(ItemBase item)
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequireItem == item);
    }

    public void Evolve(Evolution evol)
    {
        _base = evol.EvolvesInto;
        CalculateStats();


    }
    public class DamageDetail
    {
        public bool Fainted { get; set; }
        public float Critical { get; set; }
        public float TypeEffectiveness { get; set; }
    }
}
[System.Serializable]
public class FakemonSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}