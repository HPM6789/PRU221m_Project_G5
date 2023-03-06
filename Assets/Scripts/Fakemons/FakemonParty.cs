using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FakemonParty : MonoBehaviour
{
    [SerializeField] List<Fakemon> fakemons;

    public event Action OnUpdated;

    public List<Fakemon> Fakemons
    {
        get { return fakemons; }
        set { fakemons = value; OnUpdated?.Invoke(); }
    }
    private void Awake()
    {
        foreach (var fakemon in fakemons)
        {
            fakemon.Init();
        }
    }

    public Fakemon GetHealthyFakemon()
    {
        return fakemons.Where(e => e.HP > 0).FirstOrDefault();
    }

    public void AddFakemon(Fakemon fakemon)
    {
        if(fakemons.Count< 6)
        {
            fakemons.Add(fakemon);
            OnUpdated?.Invoke();
        }
        else
        {

        }
    }
    public bool CheckForEvolution()
    {
       return fakemons.Any(p => p.CheckForEvolution() != null);
    }
    public IEnumerator RunEvolution()
    {
        foreach(var fakemon in fakemons)
        {
            var evol = fakemon.CheckForEvolution();
            if(evol != null)
            {
               yield return EvolutionManager.Instance.Evolve(fakemon, evol);
            }
        }

    }
    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }
    public static FakemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<FakemonParty>();
    }
}
