using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Events
    public static EventHandler<(Tile, Tile)> PlayerFoundTilePair;
    public static EventHandler<int> TimeLeftChanged;
    public static EventHandler<string> SwitchCanvas;
    public static EventHandler<int> ScoreChanged;
    public static EventHandler<int> MultiplierChanged;
    public static EventHandler TimeOutGameOver;
    public static EventHandler Victory;
    public static EventHandler<int> LoadLevel;

    [Header("Gameplay Variables")]
    [SerializeField] private int gameDurationInSeconds;
    [Tooltip("Number of seconds until combo multiplier is reset.")]
    [SerializeField] private int multiplierSecondsLimit;

    //Levels
    private int currentLevel;
    [SerializeField] private int startingLevel; //FOR TESTING ONLY

    private int timeLeft;
    private int score;
    private int multiplier;
    private readonly int scoreAwardedPerTilePair = 100;
    private float timeOfLastTileMatch;
    private Tile lastSelectedTile;
    private IEnumerator countdownTimer;

    private void OnEnable()
    {
        Board.ClickedOnAFreeTile += OnClickedOnFreeTile;
        Board.BoardInitialized += OnBoardInitialized;
        Board.BoardCleared += OnBoardCleared;
        Board.FinalBoardCleared += OnFinalBoardCleared;
        PauseButton.PauseButtonWasPressed += OnPause;
        InstructionsButton.InstructionsButtonWasPressed += OnInstructions;
        ResumeButton.ResumeButtonWasPressed += OnResume;
        UIRestartGameButton.RestartGameButtonPressed += RestartGame;
        UINextLevelButton.NextLevelButtonPressed += OnNextLevelButtonPressed;
        SceneLoader.SceneFinishedLoading += OnSceneFinishedLoading;
    }
    private void OnDisable()
    {
        Board.ClickedOnAFreeTile -= OnClickedOnFreeTile;
        Board.BoardInitialized -= OnBoardInitialized;
        Board.BoardCleared -= OnBoardCleared;
        Board.FinalBoardCleared -= OnFinalBoardCleared;
        PauseButton.PauseButtonWasPressed -= OnPause;
        InstructionsButton.InstructionsButtonWasPressed -= OnInstructions;
        ResumeButton.ResumeButtonWasPressed -= OnResume;
        UIRestartGameButton.RestartGameButtonPressed -= RestartGame;
        UINextLevelButton.NextLevelButtonPressed -= OnNextLevelButtonPressed;
        SceneLoader.SceneFinishedLoading -= OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(object sender, string sceneName)
    {
        if (sceneName == "MainScene")
        {
            SetupGame();
        }
    }

    //private void Start()
    //{
    //    SetupGame();
    //}

    private void RestartGame(object sender, EventArgs e) => SetupGame();
    private void OnBoardInitialized(object sender, GameLevel e) => StartGameTimer();

    private void SetupGame()
    {
        //Turning on Main Canvas
        SwitchCanvas?.Invoke(this, "Canvas - In Game");

        score = 0;
        //currentLevel = 1;
        currentLevel = startingLevel;
        multiplier = 1;
        ScoreChanged?.Invoke(this, score);
        MultiplierChanged?.Invoke(this, multiplier);

        lastSelectedTile = null;
        timeLeft = gameDurationInSeconds;
        TimeLeftChanged?.Invoke(this, timeLeft);

        LoadLevel?.Invoke(this, currentLevel);
    }

    private void StartGameTimer()
    {
        if (countdownTimer != null)
        {
            StopCoroutine(countdownTimer);
        }

        timeLeft = gameDurationInSeconds;
        TimeLeftChanged?.Invoke(this, timeLeft);
        countdownTimer = CountdownTimer();
        StartCoroutine(countdownTimer);
    }

    private void OnResume(object sender, EventArgs e)
    {
        //Resume Timer
        StopCoroutine(countdownTimer);
        countdownTimer = CountdownTimer();
        StartCoroutine(countdownTimer);

        SwitchCanvas?.Invoke(this, "Canvas - In Game");
    }
    private void OnPause(object sender, EventArgs e)
    {
        //Stop Timer
        StopCoroutine(countdownTimer);

        SwitchCanvas?.Invoke(this, "Canvas - Pause");
    }

    private void OnInstructions(object sender, EventArgs e)
    {
        //Stop Timer
        StopCoroutine(countdownTimer);

        SwitchCanvas?.Invoke(this, "Canvas - Instructions");
    }
    private void OnBoardCleared(object sender, EventArgs e)
    {        
        currentLevel++;
        SwitchCanvas?.Invoke(this, "Canvas - Level Cleared");        
    }

    private void OnNextLevelButtonPressed(object sender, EventArgs e)
    {
        //Turning on Main Canvas
        SwitchCanvas?.Invoke(this, "Canvas - In Game");
        //Load next Level
        LoadLevel?.Invoke(this, currentLevel);
    }

    private void OnFinalBoardCleared(object sender, EventArgs e)
    {
        GameOver(victory: true);
    }

    private void GameOver(bool victory)
    {
        SwitchCanvas?.Invoke(this, "none");
        if (victory)
        {
            SwitchCanvas?.Invoke(this, "Canvas - Victory");
            Victory?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //CountdownTimer() will invoke TimeOutGameOver.
            SwitchCanvas?.Invoke(this, "Canvas - GameOver");
        }
    }
    private void OnClickedOnFreeTile(object sender, Tile clickedTile)
    {
        //First tile you click gets stored
        //If you click on a stuck tile, nothing happens. The stored tile doesn't change.
        //If you click on a free tile but of a different type, this new tile gets stored and the last one is forgotten.

        if (lastSelectedTile == null)
        {
            lastSelectedTile = clickedTile;
            return;
        }
        if (clickedTile == lastSelectedTile) //We are clicking on the same tile as last time, so we ignore this
        {
            return;
        }
        if (clickedTile.tileType == lastSelectedTile.tileType) //We have a match
        {
            score += scoreAwardedPerTilePair * GetMultiplier();
            ScoreChanged?.Invoke(this, score);

            PlayerFoundTilePair?.Invoke(this, (lastSelectedTile, clickedTile));

            lastSelectedTile = null;
            return;
        }

        lastSelectedTile = clickedTile;
    }
    private IEnumerator CountdownTimer()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
            TimeLeftChanged?.Invoke(this, timeLeft);
        }
        TimeOutGameOver?.Invoke(this, EventArgs.Empty);
        GameOver(victory: false);
    }
    private int GetMultiplier()
    {
        if (Time.time < timeOfLastTileMatch + multiplierSecondsLimit)
        {
            multiplier++;
        }
        else
        {
            multiplier = 1;
        }
        timeOfLastTileMatch = Time.time;

        MultiplierChanged?.Invoke(this, multiplier);
        return multiplier;
    }

    //Methods of Testing
    [ContextMenu("Test Level Cleared")]
    void TestLevelCleared() => OnNextLevelButtonPressed(null, null);
    [ContextMenu("Test GameOver Win")]
    void TestGameOverWin() => GameOver(true);
    [ContextMenu("Test GameOver Lose")]
    void TestGameOverLose() => GameOver(false);
}
