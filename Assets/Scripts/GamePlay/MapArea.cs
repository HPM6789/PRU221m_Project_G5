using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<FakemonEncounterRecord> wildFakemons;
    [SerializeField] List<FakemonEncounterRecord> wildFakemonsInWater;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    [HideInInspector]
    [SerializeField] int totalChanceWater = 0;

    private void OnValidate()
    {
        totalChance = 0;
        foreach (var record in wildFakemons)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }

        totalChanceWater = 0;
        foreach (var record in wildFakemonsInWater)
        {
            record.chanceLower = totalChanceWater;
            record.chanceUpper = totalChanceWater + record.chancePercentage;

            totalChanceWater = totalChanceWater + record.chancePercentage;
        }
    }
    private void Start()
    {
        
    }
    public Fakemon GetRandomWildFakemon(BattleTrigger trigger)
    {
        var fakemonLists = (trigger == BattleTrigger.LongGrass)? wildFakemons : wildFakemonsInWater;

        int randVal = Random.Range(1, 101);
        var fakemonRecord = fakemonLists.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        var levelRange = fakemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y);

        var wildFakemon = new Fakemon(fakemonRecord.fakemon, level);
        wildFakemon.Init();
        return wildFakemon;
    }
}
[System.Serializable]
public class FakemonEncounterRecord
{
    public FakemonBase fakemon;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
