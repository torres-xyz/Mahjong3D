using CustomHelperFunctions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static EventHandler<Tile> ClickedOnAFreeTile;
    public static EventHandler<Tile> ClickedOnAStuckTile;
    public static EventHandler BoardInitialized;
    public static EventHandler BoardCleared;
    public static EventHandler FinalBoardCleared;

    //[SerializeField] private Vector3Int boardSize;
    private readonly float gapBetweenCubes = 0.1f;
    private TileSpawner tileSpawner;
    private List<Tile> tileList;

    //Levels
    private readonly GameLevel[] availableLevels = { new Level01(), new Level02(), new Level03() };
    private int currentLevel;

    private void OnEnable()
    {
        GameManager.LoadLevel += OnLoadLevel;
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
        PlayerControlls.ClickedOnTile += OnClickedOnTile;
        RestartGameButton.RestartGameButtonPressed += OnRestartGame;
    }
    private void OnDisable()
    {
        GameManager.LoadLevel -= OnLoadLevel;
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
        PlayerControlls.ClickedOnTile -= OnClickedOnTile;
        RestartGameButton.RestartGameButtonPressed -= OnRestartGame;
    }
    void Awake()
    {
        tileSpawner = FindObjectOfType<TileSpawner>();
        if (tileSpawner == null) Debug.LogError("TileSpawner not found.");
    }

    private void OnLoadLevel(object sender, int level)
    {
        currentLevel = level;
        InitializeBoard(availableLevels[level - 1]);
    }

    private void OnRestartGame(object sender, EventArgs e)
    {
        if (tileList != null)
        {
            foreach (var item in tileList)
            {
                tileSpawner.ReleaseTile(item);
            }
        }
    }
    private void InitializeBoard(GameLevel level)
    {
#if UNITY_EDITOR

        //Making sure there's no problems with the level
        int totalCubes = 0;
        Vector3 boardSize = new Vector3(
            level.GetLevel().GetLength(0),
            level.GetLevel().GetLength(1),
            level.GetLevel().GetLength(2));
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int ii = 0; ii < boardSize.y; ii++)
            {
                for (int iii = 0; iii < boardSize.z; iii++)
                {
                    if (level.GetLevel()[i, ii, iii] == 1)
                    {
                        totalCubes++;
                    }
                }
            }
        }

        if (totalCubes == 0)
            Debug.LogError("Board size has a 0 dimension");

        int leftOverTiles = totalCubes % level.GetTileTypesInLevel().Length;
        if (leftOverTiles % 2 != 0)
            Debug.LogError($"There's an uneven number of leftover tiles. Total cubes = {totalCubes}");
        //End of checks
#endif

        tileList = CreateNewBoardFromLevel(level);
        BoardInitialized?.Invoke(this, EventArgs.Empty); //Change this to say which level was initialized
    }
    private List<Tile> CreateNewBoardFromLevel(GameLevel level)
    {
        //Create the board
        List<Tile> boardList = new();

        Vector3 boardSize = new Vector3(
            level.GetLevel().GetLength(0),
            level.GetLevel().GetLength(1),
            level.GetLevel().GetLength(2));

        //This will center them perfectly at the origin
        Vector3 startingPos = new(
            -boardSize.x * 0.5f + 0.5f - gapBetweenCubes * 1.5f,
            -boardSize.y * 0.5f + 0.5f - gapBetweenCubes * 1.5f,
            -boardSize.z * 0.5f + 0.5f - gapBetweenCubes * 1.5f);


        //Creating each cube in the board, one forloop for each dimension
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int ii = 0; ii < boardSize.y; ii++)
            {
                for (int iii = 0; iii < boardSize.z; iii++)
                {
                    if (level.GetLevel()[i, ii, iii] == 0)
                    {
                        continue;
                    }

                    Tile newTile = tileSpawner.GetTile();
                    newTile.transform.position = startingPos + new Vector3(
                        i + gapBetweenCubes * i,
                        ii + gapBetweenCubes * ii,
                        iii + gapBetweenCubes * iii);

                    newTile.transform.parent = this.transform;
                    newTile.boardPosition = new Vector3Int(i, ii, iii);
                    newTile.name = $"Cube ({i}, {ii}, {iii})";

                    boardList.Add(newTile);
                }
            }
        }

        boardList = UnityHelperFunctions.Shuffle(boardList);
        int numberOfTileTypes = level.GetTileTypesInLevel().Length;

        int currentBoardIndex = 0;
        int tileRepetitionsHere = boardList.Count / numberOfTileTypes;
        for (int i = 0; i < tileRepetitionsHere; i++)
        {
            for (int ii = 0; ii < numberOfTileTypes; ii++)
            {
                boardList[currentBoardIndex].Init(level.GetTileTypesInLevel()[ii]);
                currentBoardIndex++;
            }
        }

        //Extra tiles
        TileType extraTileType = level.GetTileTypesInLevel()[UnityEngine.Random.Range(0, level.GetTileTypesInLevel().Length)];
        int leftOverTiles = boardList.Count % numberOfTileTypes;
        for (int i = 0; i < leftOverTiles; i++)
        {
            //In here we're only adding the same type of tile as extra tiles,
            //but in a higher difficulty mode, we could spread this more evenly among other types.
            boardList[currentBoardIndex].Init(extraTileType);
            currentBoardIndex++;
        }

        return boardList;
    }

    private void OnPlayerFoundTilePair(object sender, (Tile, Tile) tilePair)
    {
        tileList.Remove(tilePair.Item1);
        tileList.Remove(tilePair.Item2);

        tileSpawner.ReleaseTile(tilePair.Item1);
        tileSpawner.ReleaseTile(tilePair.Item2);

        if (tileList.Count == 0)
        {
            if (currentLevel == availableLevels.Length - 1)
            {
                FinalBoardCleared?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                BoardCleared?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void OnClickedOnTile(object sender, Transform clickedTrans)
    {
        if (GetTileFromTransform(clickedTrans, out var clickedTile))
        {
            if (IsTileFree(clickedTile))
            {
                ClickedOnAFreeTile?.Invoke(this, clickedTile);
            }
            else
            {
                ClickedOnAStuckTile?.Invoke(this, clickedTile);
            }
        }
    }

    private bool GetTileFromTransform(Transform clickedTileTrans, out Tile tile)
    {
        tile = null;
        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].transform == clickedTileTrans)
            {
                tile = tileList[i];
                return true;
            }
        }
        return false;
    }

    private bool IsTileFree(Tile clickedTile)
    {
        //Cube tiles can only be pressed if they are not blocked
        //by other tiles on their left and right sides.

        bool blockedOnceOnX = false;
        bool blockedOnceOnZ = false;

        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].boardPosition.y != clickedTile.boardPosition.y)
                continue; //we don't need to check tiles with different Y coords

            if (tileList[i].boardPosition.x == clickedTile.boardPosition.x &&
                (tileList[i].boardPosition.z == clickedTile.boardPosition.z - 1 ||
                tileList[i].boardPosition.z == clickedTile.boardPosition.z + 1))
            {
                if (blockedOnceOnZ)
                {
                    return false; //Tile is blocked on both sides
                }
                blockedOnceOnZ = true;
            }
            if (tileList[i].boardPosition.z == clickedTile.boardPosition.z &&
                (tileList[i].boardPosition.x == clickedTile.boardPosition.x - 1 ||
                tileList[i].boardPosition.x == clickedTile.boardPosition.x + 1))
            {
                if (blockedOnceOnX)
                {
                    return false; //Tile is blocked on both sides
                }
                blockedOnceOnX = true;
            }
        }
        return true;
    }
}