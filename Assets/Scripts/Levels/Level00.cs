public class Level00 : GameLevel
{
    private readonly int[,,] level = new int[3, 3, 3] //3*3*3 = 27, so 0 at center to make it 26
    {
        {
            { 1, 1, 1},{ 1, 1, 1 },{ 1, 1, 1}
        },
        {
            { 1, 1, 1},{ 1, 0, 1 },{ 1, 1, 1}
        },
        {
            { 1, 1, 1},{ 1, 1, 1 },{ 1, 1, 1}
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