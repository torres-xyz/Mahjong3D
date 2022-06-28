public class Level03 : GameLevel
{
    private readonly int[,,] level = new int[5, 5, 5] //92 cubes
    {
        {
            { 0,0,1,0,0 }, { 0, 1, 1, 1, 0 }, { 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 0 }, { 0, 0, 1, 0, 0 }
        },
        {
            { 0,1,1,1,0 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 0 }
        },
        {
            { 1,1,1,1,1 }, { 1,1,1,1,1 }, { 1, 1, 0, 1, 1 }, { 1,1,1,1,1 }, { 1,1,1,1,1 } //putting a 0 in the center to make this even
        },
        {
            { 0,1,1,1,0 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 0 }
        },
        {
            { 0,0,1,0,0 }, { 0, 1, 1, 1, 0 }, { 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 0 }, { 0, 0, 1, 0, 0 }
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