using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> sfxList;

    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;

    AudioClip currentMusic;
    [SerializeField] float fadeDuration;
    float originalMusicVol;

    Dictionary<AudioID, AudioData> sfxLookUp;

    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        
    }
    private void Start()
    {
        originalMusicVol = musicPlayer.volume;
        sfxLookUp = sfxList.ToDictionary(x => x.id);
    }
    public void PlaySfx(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null) return;
        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }
        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySfx(AudioID audioID, bool pauseMusic = false)
    {
        if (!sfxLookUp.ContainsKey(audioID)) return;
            var audioData = sfxLookUp[audioID];

        PlaySfx(audioData.clip, pauseMusic);
    }
    public void PlayerMusic(AudioClip clip, bool loop, bool fade = false)
    {
        if (clip == null || clip == currentMusic) return;

        currentMusic = clip;

        StartCoroutine(PlayMusicAsync(clip, loop, fade));
    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    {
        if (fade)
           yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();

        musicPlayer.clip = clip;
        musicPlayer.loop = loop;

        musicPlayer.Play();

        if (fade)
            yield return musicPlayer.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();
    }

    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalMusicVol, fadeDuration);
    }
}
public enum AudioID
{
    UISelect, Hit, Faint, ExpGain, Pickup
}

[System.Serializable]
public class AudioData
{
    public AudioID id;
    public AudioClip clip;
}