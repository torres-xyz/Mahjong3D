using CustomHelperFunctions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClipsList;
    List<AudioSource> audioSourcePool;
    readonly int audioSourcePoolPreloadAmout = 5;
    AudioSource audioSourcePlayingLoopingMusic;
    [Range(0, 1)]
    [SerializeField] float musicVolume;

    private void Start()
    {
        InitializeAudioSourcePool();
        PlayLoopingMusic("Intro - Run For It");
    }

    private void OnEnable()
    {
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
        GameManager.Victory += OnVictory;
        GameManager.TimeOutGameOver += OnTimeOutGameOver;
        Board.ClickedOnAStuckTile += OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile += OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed += OnButtonWasClicked;
        PauseButton.PauseButtonWasPressed += OnPauseButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed += OnButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed += OnRestartGame;
        ResumeButton.ResumeButtonWasPressed += OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed += OnResumeButtonWasClicked;
        PlayerControlls.SwippedRightToLeft += OnSpinCamera;
        PlayerControlls.SwippedLeftToRight += OnSpinCamera;
        StartGameButton.LoadMainScene += OnLoadMainScene;
    }
    private void OnDisable()
    {
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
        GameManager.Victory -= OnVictory;
        GameManager.TimeOutGameOver -= OnTimeOutGameOver;
        Board.ClickedOnAStuckTile -= OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile -= OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed -= OnButtonWasClicked;
        PauseButton.PauseButtonWasPressed -= OnPauseButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed -= OnButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed -= OnRestartGame;
        ResumeButton.ResumeButtonWasPressed -= OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed -= OnResumeButtonWasClicked;
        PlayerControlls.SwippedRightToLeft -= OnSpinCamera;
        PlayerControlls.SwippedLeftToRight -= OnSpinCamera;
        StartGameButton.LoadMainScene -= OnLoadMainScene;
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
    private void OnPlayerFoundTilePair(object sender, (Tile, Tile) e) => PlayAudio("FoundTilePair_V2");

    private void PlayAudio(string clipToPlay)
    {
        AudioSource sourceToUse = GetAudioSourceFromPool();
        sourceToUse.clip = audioClipsList.Find(_ => _.name == clipToPlay);
        sourceToUse.Play();
    }

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
            audioSourcePool.Add(gameObject.AddComponent<AudioSource>());
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
        return newAudioSource;
    }
}

