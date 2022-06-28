public class Level00 : GameLevel
{
    private readonly int[,,] level = new int[2, 2, 2] //2*2*2 = 8
    {
        {
            { 1, 1},{ 1, 1 }
        },
        {
            { 1, 1},{ 1, 1 }
        },
    };

    private readonly TileType[] typesInLevel = new TileType[]
    {
        TileType.Owl,
        TileType.Panda,
        TileType.Parrot,
    };

    public override int[,,] GetLevel() => level;
    public override TileType[] GetTileTypesInLevel() => typesInLevel;
}