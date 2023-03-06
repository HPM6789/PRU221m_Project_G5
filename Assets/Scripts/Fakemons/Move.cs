using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBase fBase)
    {
        Base = fBase;
        PP = fBase.Pp;
    }

    public Move(MoveSaveData saveData)
    {
        PP = saveData.pp;
        Base = MoveBaseDB.GetObjectByName(saveData.name);
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = Base.name,
            pp = PP
        };
        return saveData;
    }

    public void IncreasePP(int amount)
    {
        PP = Mathf.Clamp(PP + amount, 0, Base.Pp);
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}
