using UnityEngine;
using UnityEngine.Pool;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] Tile genericTilePrefab;
    private ObjectPool<Tile> tilePool;

    private void Awake()
    {
        tilePool = new ObjectPool<Tile>(
            createFunc: CreateTile,
            actionOnGet: OnTakeTileFromPool,
            actionOnRelease: OnReturnTileToPool);
    }
    //Object Pooling Methods
    private Tile CreateTile() => Instantiate(genericTilePrefab);
    private void OnTakeTileFromPool(Tile tile) => tile.gameObject.SetActive(true);
    private void OnReturnTileToPool(Tile tile) => tile.SelfDisable();
    //Public Methods
    public Tile GetTile() => tilePool.Get();
    public void ReleaseTile(Tile tile) => tilePool.Release(tile);
    public Tile GetTileByType(TileType tileType)
    {
        Tile newTile = tilePool.Get();
        newTile.Init(tileType);
        return newTile;
    }
}
