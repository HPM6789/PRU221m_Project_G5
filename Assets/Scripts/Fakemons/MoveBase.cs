using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Move", menuName ="Fakemon/Create New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] FakemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffect effects;
    [SerializeField] MoveTarget target;
    [SerializeField] List<SecondaryEffect> secondaryEffects;
    [SerializeField] int piority;

    [SerializeField] AudioClip sound;

    public string Name { get { return name; } }
    public FakemonType Type { get { return type; } }
    public string Description { get { return description; } }
    public int Power { get { return power; } }
    public float Accuracy { get { return accuracy; } }
    public int Pp { get { return pp; } }
    public int Piority { get { return piority;} }

    public MoveCategory Category { get { return category; } } 
    public MoveEffect Effects { get { return effects; } }

    public MoveTarget Target { get { return target; } }
    public List<SecondaryEffect> SecondaryEffects { get { return secondaryEffects; } }
    public bool AlwaysHits { get { return alwaysHits; } }

    public AudioClip Sound => sound;
}

[System.Serializable]
public class MoveEffect
{
    [SerializeField] List<StatBoost> boost;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts { get { return boost; } }
    public ConditionID Status { get { return status; } }
    public ConditionID VolatileStatus { get { return volatileStatus; } }
}

[System.Serializable]
public class SecondaryEffect : MoveEffect
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get { return chance; } }
    public MoveTarget Target { get{ return target; } }
} 

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}
public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}