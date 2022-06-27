public class Level01 : GameLevel
{
    private readonly int[,,] level = new int[4, 4, 4] //Solid cube, all 1s
    {
        {
            { 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 }
        },
        {
            { 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 }
        },
        {
            { 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 }
        },
        {
            { 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 },{ 1, 1, 1, 1 }
        },
    };

    private readonly TileType[] typesInLevel = new TileType[]
    {
        TileType.Owl,
        TileType.Panda,
        TileType.Parrot,
        TileType.Penguin,
        TileType.Pig,
        TileType.Snake
    };

    public override int[,,] GetLevel() => level;
    public override TileType[] GetTileTypesInLevel() => typesInLevel;
}