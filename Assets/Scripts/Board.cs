using CustomHelperFunctions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static EventHandler<Tile> ClickedOnAFreeTile;
    public static EventHandler ClickedOnAStuckTile;
    public static EventHandler BoardCleared;
    public static EventHandler BoardInitialized;

    [SerializeField] private Vector3Int boardSize;
    [SerializeField] private float gapBetweenCubes;
    private TileSpawner tileSpawner;
    private List<Tile> tileList;

    private void OnEnable()
    {
        PlayerControlls.ClickedOnTile += OnClickedOnTile;
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
        RestartGameButton.RestartGameButtonPressed += RestartBoard;
    }
    private void OnDisable()
    {
        PlayerControlls.ClickedOnTile -= OnClickedOnTile;
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
        RestartGameButton.RestartGameButtonPressed -= RestartBoard;
    }
    void Start()
    {
        tileSpawner = FindObjectOfType<TileSpawner>();
        if (tileSpawner == null) Debug.LogError("TileSpawner not found.");

        InitializeBoard();
    }
    private void RestartBoard(object sender, EventArgs e)
    {
        //Debug.Log("Restarting board");
        if (tileList != null)
        {
            foreach (var item in tileList)
            {
                tileSpawner.ReleaseTile(item);
            }
        }

        InitializeBoard();
    }

    private void InitializeBoard()
    {
        //Determine how many of each cube will go into the board.
        //4 * 4 * 4 = 64. 64 / 6 = 10.666...
        int totalCubes = boardSize.x * boardSize.y * boardSize.z;
        if (totalCubes == 0)
            Debug.LogError("Board size has a 0 dimension");

        int numberOfTileTypes = Enum.GetNames(typeof(TileType)).Length;
        int tileRepetitions = totalCubes / numberOfTileTypes;
        int leftOverTiles = totalCubes % numberOfTileTypes;

        //Debug.Log($"There are {totalCubes} total Cubes. With {cubeTypes.Count} different tiles. " +
        //    $"So, each tile repeates {tileRepetitions} times. With one type having an extra {leftOverTiles} tiles.");

        //Ideally some logic would be applied here to use a reduced number of tile types
        //to make it so we get an even number of leftover tiles.
        if (leftOverTiles % 2 != 0)
            Debug.LogError("There's an uneven number of leftover tiles.");

        tileList = CreateNewBoardFromPool(boardSize, tileRepetitions, leftOverTiles);
        BoardInitialized?.Invoke(this, EventArgs.Empty);
    }

    private List<Tile> CreateNewBoardFromPool(Vector3 boardSize, int tileRepetitions, int extraTiles)
    {
        //Create the board
        List<Tile> boardList = new();

        //This will center them perfectly at the origin
        Vector3 startingPos = new(
            -boardSize.x * 0.5f + 0.5f - gapBetweenCubes * 1.5f,
            -boardSize.y * 0.5f + 0.5f - gapBetweenCubes * 1.5f,
            -boardSize.z * 0.5f + 0.5f - gapBetweenCubes * 1.5f);

        int count = 0;
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int ii = 0; ii < boardSize.y; ii++)
            {
                for (int iii = 0; iii < boardSize.z; iii++)
                {
                    Tile newTile = tileSpawner.GetTile();
                    newTile.transform.position = startingPos + new Vector3(
                        i + gapBetweenCubes * i,
                        ii + gapBetweenCubes * ii,
                        iii + gapBetweenCubes * iii);

                    newTile.transform.parent = this.transform;
                    newTile.boardPosition = new Vector3Int(i, ii, iii);
                    newTile.name = $"Cube {i} {ii} {iii}";

                    boardList.Add(newTile);
                    count++;
                }
            }
        }

        boardList = UnityHelperFunctions.Shuffle(boardList);
        int numberOfTileTypes = Enum.GetNames(typeof(TileType)).Length;
        int currentBoardIndex = 0;
        for (int i = 0; i < tileRepetitions; i++)
        {
            for (int ii = 0; ii < numberOfTileTypes; ii++)
            {
                boardList[currentBoardIndex].Init((TileType)ii);
                currentBoardIndex++;
            }
        }

        //Extra tiles
        TileType extraTileType = (TileType)UnityEngine.Random.Range(0, numberOfTileTypes);
        for (int i = 0; i < extraTiles; i++)
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

        if (tileList.Count == 0) //Win Game
            BoardCleared?.Invoke(this, EventArgs.Empty);
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
                ClickedOnAStuckTile?.Invoke(this, EventArgs.Empty);
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
