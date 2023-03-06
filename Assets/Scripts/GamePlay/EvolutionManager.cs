using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image fakemonImage;
    [SerializeField] AudioClip evolutionMusic;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;
    public static EvolutionManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator Evolve(Fakemon fakemon, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);

        AudioManager.Instance.PlayerMusic(evolutionMusic, true, false);

        fakemonImage.sprite = fakemon.Base.FrontSprite;

        yield return DialogManager.Instance.ShowDialogText($"{fakemon.Base.Name} is evolving!!!");

        string oldFakemon = fakemon.Base.Name;
        fakemon.Evolve(evolution);

        fakemonImage.sprite = fakemon.Base.FrontSprite;

        yield return DialogManager.Instance.ShowDialogText($"{oldFakemon} evolved into {fakemon.Base.Name}!");
        evolutionUI.SetActive(false);

        OnCompleteEvolution?.Invoke();
    }
}
