public class Level02 : GameLevel
{
    private readonly int[,,] level = new int[5, 5, 5] //Total 108 cubes
    {
        {                                                                            //this colum is the top level
            { 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 }
        },
        {
            { 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 0 }
        },
        {
            { 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 }, { 0, 1, 0, 1, 0 }
        },
        {
            { 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 0 }
        },
        {
            { 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 },{ 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 }
        },
    };

    private readonly TileType[] typesInLevel = new TileType[] //Has to have 8 types, so that (108 % 8) % 2 = 0
    {
        TileType.Owl,
        TileType.Panda,
        TileType.Parrot,
        TileType.Penguin,
        TileType.Pig,
        TileType.Snake,
        TileType.Chick,
        TileType.Gorilla
    };

    public override int[,,] GetLevel() => level;
    public override TileType[] GetTileTypesInLevel() => typesInLevel;
}