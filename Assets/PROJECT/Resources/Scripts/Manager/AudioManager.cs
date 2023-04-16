using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    List<Sound> sfxSources = new List<Sound>();
    List<Sound> bgmSources = new List<Sound>();
    
    public Slider sfxSlider;
    public Slider bgmSlider;

    public string currentbgm;
    public Sound currentBgmClip;
    public Sound[] sounds;

    public bool shoudPlayBgm;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        shoudPlayBgm = true;
    }
    void Start()
    {
        SetSoundList();
        InitVolumeBar();
        
    }


    public Sound GetSoundAccordingToName(string name)
{

        if (name.Contains("bgm"))
        {
            Sound s = bgmSources.Find(sound => sound.name == name);
        return s;
        }
        else
        {
            Sound s = sfxSources.Find(sound => sound.name == name);
            return s;
        }
    }


    public void InitVolumeBar()
    {
        //sfxSlider.value = hubManager.player.GetStats().sfxVolume;
        //bgmSlider.value = hubManager.player.GetStats().bgmVolume;
    }

    void Update()
    {
        //UpdateSFXVolume();
        //UpdateBGMVolume();
    }



    public void Play(string name)
    {
        if (name.Contains("bgm"))
        {
            Sound s = bgmSources.Find(sound => sound.name == name);
            currentBgmClip = s;
            s.source.Play();
        }
        else
        {
            Sound s = sfxSources.Find(sound => sound.name == name);
            s.source.Play();
        }
    }
    public void Pause(string name)
{
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Pause();
    }

    public void Stop(string name)
{
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void UpdateSFXVolume()
{
        foreach (Sound s in sfxSources)
        {
            //s.source.volume = sfxSlider.value;

            //gm.player.GetStats().sfxVolume = sfxSlider.value;
            //gm.player.sfxVolume = gm.player.GetStats().sfxVolume;
        }
    }

    public void UpdateBGMVolume()
{
        foreach (Sound bgm in bgmSources)
        {

            //bgm.source.volume = bgmSlider.value;

           
            //gm.player.GetStats().bgmVolume = bgmSlider.value;
            //gm.player.bgmVolume = gm.player.GetStats().bgmVolume;
        }
    }

    public void SetSoundList()
    {
        foreach (Sound s in sounds)
        {

            if (s.name.Contains("bgm"))
            {
                bgmSources.Add(s);
            }
            else
            {
                sfxSources.Add(s);
            }
        }
    }

}
