using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

    public readonly string mgFX = "FX";
    public readonly string mgMusic = "Music";

    public static AudioController Instance { get; private set; }

    [SerializeField]
    private AudioMixerGroup audioMusic;
    private AudioMixer mixerMusic;
    [SerializeField]
    private AudioMixerGroup audioFX;
    private AudioMixer mixerFX;

    [SerializeField]
    private AudioClip[] musics;
    [SerializeField]
    private AudioSource asMusic;
    public GameObject explosionPrefab;

    public bool muteFX { get; private set; }
    public bool muteMusic { get; private set; }
    public float volumeFX { get; private set; }
    public float volumeMusic { get; private set; }

    void Start () {
        Instance = this;
	}

    private void Awake()
    {
        Instance = this;
        muteFX = false;
        muteMusic = false;
        volumeFX = 0f;
        volumeMusic = 0f;
        mixerFX = audioFX.audioMixer;
        mixerMusic = audioMusic.audioMixer;
        LoadSettings();

        SelectMusic();
    }

    public void LoadSettings()
    {
        muteFX = PlayerPrefs.GetInt(SaveConfig.FXMute, 0) == 1;
        muteMusic = PlayerPrefs.GetInt(SaveConfig.MusicMute, 0) == 1;
        volumeFX = PlayerPrefs.GetFloat(SaveConfig.FXVolume, 0f);
        volumeMusic = PlayerPrefs.GetFloat(SaveConfig.MusicVolume, 0f);
        mixerFX.SetFloat(mgFX, volumeFX);
        mixerMusic.SetFloat(mgMusic, volumeMusic);
   }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(SaveConfig.FXMute, (AudioController.Instance.muteFX ? 1 : 0));
        PlayerPrefs.SetInt(SaveConfig.MusicMute, (AudioController.Instance.muteMusic ? 1 : 0));
        PlayerPrefs.SetFloat(SaveConfig.FXVolume, AudioController.Instance.volumeFX);
        PlayerPrefs.SetFloat(SaveConfig.MusicVolume, AudioController.Instance.volumeMusic);
    }

    public void MuteFX(bool value)
    {
        muteFX = value;
        if (muteFX)
        {
            mixerFX.SetFloat(mgFX, -80f);
        }
        else
        {
            mixerFX.SetFloat(mgFX, volumeFX);
        }
        SaveSettings();
    }

    public void MuteMusic(bool value)
    {
        muteMusic = value;
        if (muteMusic)
        {
            mixerMusic.SetFloat(mgMusic, -80f);
        }
        else
        {
            mixerMusic.SetFloat(mgMusic, volumeMusic);
            SaveSettings();
        }
    }

    public void ToggleFX()
    {
        muteFX = !muteFX;
        MuteFX(muteFX);
    }

    public void ToggleMusic()
    {
        muteMusic = !muteMusic;
        MuteMusic(muteMusic);
    }

    public void SetFXVolume(float value)
    {
        volumeFX = value;
        SaveSettings();
        if (!muteFX)
            mixerFX.SetFloat(mgFX, value);
    }

    public void SetMusicVolume(float value)
    {
        volumeMusic = value;
        SaveSettings();
        if (!muteMusic)
            mixerMusic.SetFloat(mgMusic, value);
    }

    public void SelectMusic()
    {
        asMusic.Stop();
        int mc = Random.Range(0, musics.Length);
        asMusic.clip = musics[mc];
        asMusic.Play();
    }

    private void Update()
    {
        
        if (asMusic.time >= asMusic.clip.length)
            SelectMusic();
    }

    public void SetExplosionSound(Vector3 _pos, AudioClip _sound)
    {
        GameObject go = Instantiate(explosionPrefab, _pos, Quaternion.identity) as GameObject;
        AudioSource es = go.GetComponent<AudioSource>();
        es.clip = _sound;
        es.Play();
        Destroy(go, es.clip.length);
    }

}
