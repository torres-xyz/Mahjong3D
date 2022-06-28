using CustomHelperFunctions;
using System;
using System.Collections;
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
    }
    private void OnDisable()
    {
        GameManager.LoadLevel -= OnLoadLevel;
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
        PlayerControlls.ClickedOnTile -= OnClickedOnTile;
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
    private void ReleaseAllTilesInPool()
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

        //if (totalCubes / level.GetTileTypesInLevel().Length % 2 != 0)
        //    Debug.LogError($"There's an uneven number of tile repetitions. Total cubes = {totalCubes}, Number of tile types = {level.GetTileTypesInLevel().Length}, Tile repetitions = {totalCubes / level.GetTileTypesInLevel().Length}");

        //int leftOverTiles = totalCubes % level.GetTileTypesInLevel().Length;
        //if (leftOverTiles % 2 != 0)
        //    Debug.LogError($"There's an uneven number of leftover tiles. Total cubes = {totalCubes}");
        //End of checks
#endif
        ReleaseAllTilesInPool();
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
        //tileRepetitions has to be pair
        int tileRepetitions = boardList.Count / numberOfTileTypes;
        bool subtractedOneFromTileRepetitionsToMakeItPair = false;

        if (tileRepetitions % 2 != 0) //means it's not pair
        {
            subtractedOneFromTileRepetitionsToMakeItPair = true;
            tileRepetitions--;
        }

        for (int i = 0; i < tileRepetitions; i++)
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

        if (subtractedOneFromTileRepetitionsToMakeItPair)
        {
            leftOverTiles += numberOfTileTypes;
        }

        //Debug.Log($"extraTileType = {extraTileType}");
        //Debug.Log($"leftOverTiles  = {leftOverTiles}");


        //Figuring out the best distribution for left over tiles

        for (int i = 0; i < leftOverTiles / 2; i++)
        {
            boardList[currentBoardIndex].Init(level.GetTileTypesInLevel()[i]);
            boardList[currentBoardIndex + 1].Init(level.GetTileTypesInLevel()[i]);
            currentBoardIndex += 2;
        }


        //for (int i = 0; i < leftOverTiles; i++)
        //{
        //    //In here we're only adding the same type of tile as extra tiles,
        //    //but in a higher difficulty mode, we could spread this more evenly among other types.
        //    boardList[currentBoardIndex].Init(extraTileType);
        //    currentBoardIndex++;
        //}


        //Debug
        //for (int i = 0; i < level.GetTileTypesInLevel().Length; i++)
        //{
        //    int count = 0;
        //    for (int ii = 0; ii < boardList.Count; ii++)
        //    {
        //        if (boardList[ii].tileType == level.GetTileTypesInLevel()[i])
        //        {
        //            count++;
        //        }
        //    }
        //    Debug.Log($"There are {count} tiles of type {level.GetTileTypesInLevel()[i]}");
        //    if (count % 2 != 0)
        //    {
        //        Debug.LogError($"There's an uneven number of tiles of type {level.GetTileTypesInLevel()[i]}");
        //    }
        //}
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
            if (currentLevel == availableLevels.Length)
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

    [ContextMenu("Test Level Cleared")]
    void TestLevelCleared()
    {
        var tileListCopy = tileList;
        foreach (var tile in tileListCopy)
        {
            tileSpawner.ReleaseTile(tile);
        }
        BoardCleared?.Invoke(this, EventArgs.Empty);
    }

    [ContextMenu("Solve Board")]
    void SolveBoard()
    {
        IEnumerator solveBoard = SolveBoardTileByTile(0.1f);
        StopCoroutine(solveBoard);
        StartCoroutine(solveBoard);
    }

    IEnumerator SolveBoardTileByTile(float waitTime)
    {
        //This will try to find all pairs of a select type,
        //then when it can't, it moves on to the next type, until it loops around
        //to the first type tried, until all tiles are done.

        //Create a list with all types present in the board
        List<TileType> typesInBoard = new();
        foreach (var tile in tileList)
        {
            if (!typesInBoard.Contains(tile.tileType))
                typesInBoard.Add(tile.tileType);
        }
        int typesInBoardCurrentIndex = 0;

        bool exitOfLoop = false;
        int count = 0;
        while (exitOfLoop == false)
        {
            count++;

            bool hasFound1stFreeTile = false;
            Tile currentTileSelected = null;

            for (int i = 0; i < tileList.Count; i++)
            {
                //Only checking for one type of tile at a time
                if (tileList[i].tileType != typesInBoard[typesInBoardCurrentIndex]) continue;

                //Picking our first free tile
                if (hasFound1stFreeTile == false && IsTileFree(tileList[i]))
                {
                    hasFound1stFreeTile = true;
                    currentTileSelected = tileList[i];
                    continue;
                }

                if (hasFound1stFreeTile == true && //If we have already found one free tile  
                    tileList[i] != currentTileSelected && //AND it's not this one
                    IsTileFree(tileList[i]) && //AND this tile is free
                    currentTileSelected.tileType == tileList[i].tileType) //AND the types are the same
                {
                    (Tile, Tile) tilePair = (currentTileSelected, tileList[i]);
                    OnPlayerFoundTilePair(null, tilePair);

                    hasFound1stFreeTile = false;
                    currentTileSelected = null;

                    yield return new WaitForSeconds(waitTime);
                    continue;
                }
            }

            //When we reach the end of list we move on to the next tile type on the list
            //(looping the index)
            typesInBoardCurrentIndex = typesInBoard.Count > typesInBoardCurrentIndex + 1 ? ++typesInBoardCurrentIndex : 0;

            if (tileList.Count == 0)
            {
                Debug.Log("No more tiles in list");
                exitOfLoop = true;
            }
            if (count > 100)
            {
                exitOfLoop = true;
                Debug.Log("Loop got stuck");
            }
        }
    }



}