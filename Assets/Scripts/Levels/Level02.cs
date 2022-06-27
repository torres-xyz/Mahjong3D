public class Level02 : GameLevel
{
    public override int[,,] GetLevel() => new int[5, 5, 5]
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

    public override TileType[] GetTileTypesInLevel() => new TileType[]
    {
        TileType.Owl,
        TileType.Panda,
        TileType.Parrot,
        TileType.Penguin,
        TileType.Pig,
        TileType.Snake
    };
}