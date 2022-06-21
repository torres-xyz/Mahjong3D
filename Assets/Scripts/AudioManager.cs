using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] float musicVolume;
    [SerializeField] List<AudioClip> audioClipsList;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup audioMixerGroup;

    List<AudioSource> audioSourcePool;
    readonly int audioSourcePoolPreloadAmout = 5;
    AudioSource audioSourcePlayingLoopingMusic;
    private int currentMultiplier;
    private readonly int maxMultiplerGuesstimate = 20;
    //private float currentAudioLevel = 0.5f;

    private void Start()
    {
        InitializeAudioSourcePool();
        PlayLoopingMusic("Intro - Run For It");
        currentMultiplier = 1;
    }

    private void OnEnable()
    {
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
        GameManager.Victory += OnVictory;
        GameManager.TimeOutGameOver += OnTimeOutGameOver;
        GameManager.MultiplierChanged += OnMultiplierChanged;
        Board.ClickedOnAStuckTile += OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile += OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed += OnButtonWasClicked;
        PauseButton.PauseButtonWasPressed += OnPauseButtonWasClicked;
        InstructionsButton.InstructionsButtonWasPressed += OnButtonWasClicked;
        InstructionsButton.InstructionsButtonWasPressed += OnPauseButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed += OnButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed += OnRestartGame;
        ResumeButton.ResumeButtonWasPressed += OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed += OnResumeButtonWasClicked;
        PlayerControlls.SwippedRightToLeft += OnSpinCamera;
        PlayerControlls.SwippedLeftToRight += OnSpinCamera;
        StartGameButton.LoadMainScene += OnLoadMainScene;

        UIVolumeSlider.VolumeChanged += OnVolumeChanged;
    }
    private void OnDisable()
    {
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
        GameManager.Victory -= OnVictory;
        GameManager.TimeOutGameOver -= OnTimeOutGameOver;
        GameManager.MultiplierChanged -= OnMultiplierChanged;
        Board.ClickedOnAStuckTile -= OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile -= OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed -= OnButtonWasClicked;
        PauseButton.PauseButtonWasPressed -= OnPauseButtonWasClicked;
        InstructionsButton.InstructionsButtonWasPressed -= OnButtonWasClicked;
        InstructionsButton.InstructionsButtonWasPressed -= OnPauseButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed -= OnButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed -= OnRestartGame;
        ResumeButton.ResumeButtonWasPressed -= OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed -= OnResumeButtonWasClicked;
        PlayerControlls.SwippedRightToLeft -= OnSpinCamera;
        PlayerControlls.SwippedLeftToRight -= OnSpinCamera;
        StartGameButton.LoadMainScene -= OnLoadMainScene;
        
        UIVolumeSlider.VolumeChanged += OnVolumeChanged;
    }

    //Music
    private void OnVictory(object sender, EventArgs e) => PlayLoopingMusic("Victory - New Age Ghost");
    private void OnTimeOutGameOver(object sender, EventArgs e) => PlayLoopingMusic("GameOver - Morning Grind");
    private void OnResumeButtonWasClicked(object sender, EventArgs e) => PlayLoopingMusic("Gameplay - Bubblegum Pop Hiphop");
    private void OnRestartGame(object sender, EventArgs e) => PlayLoopingMusic("Gameplay - Bubblegum Pop Hiphop");
    private void OnPauseButtonWasClicked(object sender, EventArgs e) => PlayLoopingMusic("Pause - Low Growl");

    //Sound Effects
    private void OnLoadMainScene(object sender, EventArgs e) => PlayLoopingMusic("Gameplay - Bubblegum Pop Hiphop");
    private void OnSpinCamera(object sender, EventArgs e) => PlayAudio("CameraSpin");
    private void OnButtonWasClicked(object sender, EventArgs e) => PlayAudio("ButtonClicked");
    private void OnClickedOnAFreeTile(object sender, Tile e) => PlayAudio("FreeTile");
    private void OnClickedOnAStuckTile(object sender, EventArgs e) => PlayAudio("StuckTile");
    private void OnPlayerFoundTilePair(object sender, (Tile, Tile) e) => PlayAudio("FoundTilePair_V2", MultiplierToPitch(currentMultiplier));
    //Logic
    private void OnMultiplierChanged(object sender, int multi) => currentMultiplier = multi;
    private void OnVolumeChanged(object sender, float vol) => audioMixer.SetFloat("MainVolume", vol);

    private void PlayAudio(string clipToPlay)
    {
        AudioSource sourceToUse = GetAudioSourceFromPool();
        sourceToUse.clip = audioClipsList.Find(_ => _.name == clipToPlay);
        sourceToUse.pitch = 1;
        sourceToUse.Play();
    }
    private void PlayAudio(string clipToPlay, float pitch)
    {
        AudioSource sourceToUse = GetAudioSourceFromPool();
        sourceToUse.clip = audioClipsList.Find(_ => _.name == clipToPlay);
        sourceToUse.pitch = pitch;
        sourceToUse.Play();
    }

    private float MultiplierToPitch(int multiplier) => Mathf.Lerp(1, 3, (multiplier - 1) / (float)maxMultiplerGuesstimate); // (multiplier - 1) because the base multiplier is already 1.

    private void PlayLoopingMusic(string clipToPlay)
    {
        if (audioSourcePlayingLoopingMusic == null)
        {
            audioSourcePlayingLoopingMusic = GetAudioSourceFromPool();
        }
        audioSourcePlayingLoopingMusic.clip = audioClipsList.Find(_ => _.name == clipToPlay);
        audioSourcePlayingLoopingMusic.loop = true;
        audioSourcePlayingLoopingMusic.volume = musicVolume;
        audioSourcePlayingLoopingMusic.Play();
    }

    private void InitializeAudioSourcePool()
    {
        audioSourcePool = new List<AudioSource>();
        for (int i = 0; i < audioSourcePoolPreloadAmout; i++)
        {
            audioSourcePool.Add(ExpandAudioPool());
        }
    }
    private AudioSource GetAudioSourceFromPool()
    {
        for (int i = 0; i < audioSourcePool.Count; i++)
        {
            if (audioSourcePool[i].isPlaying == false)
                return audioSourcePool[i];
        }

        //All audio sources are busy, we must create a new one
        return ExpandAudioPool();
    }

    private AudioSource ExpandAudioPool()
    {
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        audioSourcePool.Add(newAudioSource);
        newAudioSource.outputAudioMixerGroup = audioMixerGroup;
        return newAudioSource;
    }
}

