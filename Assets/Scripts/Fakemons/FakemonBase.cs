using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fakemon", menuName = "Fakemon/Create New Fakemon")]
public class FakemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] FakemonType type1;
    [SerializeField] FakemonType type2;
    [SerializeField] GrowthRate growthRate;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int expYield;

    [SerializeField] int catchRate = 225;


    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;

    [SerializeField] List<Evolution> evolutions;

    public GrowthRate GrowthRate
    {
        get { return growthRate; }
    }
    public int ExpYield
    {
        get { return expYield; }
    }
    public int CatchRate
    {
        get { return catchRate; }
    }
    public string Name
    {
       get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public FakemonType Type1
    {
        get { return type1; }
    }
    public FakemonType Type2
    {
        get { return type2; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public float Attack
    {
        get { return attack; }
    }
    public float Defense
    {
        get { return defense; }
    }
    public float SpAttack
    {
        get { return spAttack; }
    }
    public float SpDefense
    {
        get { return spDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }


    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
    public List<MoveBase> LearnableByItems => learnableByItems;

    public List<Evolution> Evolutions => evolutions;


    public int GetExpForLevel(int level)
    {
        if(growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }else if(growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }

        return -1;
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
[System.Serializable]
public class Evolution
{
    [SerializeField] FakemonBase evolvesInto;
    [SerializeField] int requiredLevel;
    [SerializeField] EvolutionItem requireItem;

    public int RequiredLevel => requiredLevel;
    public FakemonBase EvolvesInto => evolvesInto;
    public ItemBase RequireItem => requireItem;
}
public enum FakemonType
{
    None,
    Normal,
    Fire,
    Water, 
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum GrowthRate
{
    Fast, MediumFast
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    //Not Actually Stat
    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[][] chart =
    {
        /*Nor*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f},
        /*Fir*/ new float[] {1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f},
        /*Wat*/ new float[] {1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f},
        /*Ele*/ new float[] {1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f},
        /*Gra*/ new float[] {1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f},
        /*Ice*/ new float[] {1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f },
        /*Fig*/ new float[] {2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f },
        /*Poi*/ new float[] {1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f },
        /*Gro*/ new float[] {1f, 2f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f },
        /*Fly*/ new float[] {1f, 1f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f },
        /*Psy*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f },
        /*Bug*/ new float[] {1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 0.5f },
        /*Roc*/ new float[] {1f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f },
        /*Gho*/ new float[] {0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0,5f, 1f, 1f },
        /*Dra*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f },
        /*Dar*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f },
        /*Ste*/ new float[] {1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f },
        /*Fai*/ new float[] {1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 0.5f, 1f },

    };

    public static float GetEffectiveness(FakemonType attackType, FakemonType defenseType)
    {
        if(attackType == FakemonType.None || defenseType == FakemonType.None)
        {
            return 1;
        }

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];
    }

}