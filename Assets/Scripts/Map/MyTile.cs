public class GridObject
{
    public TileType Type { get; private set; }

    public GridObject(TileType type)
    {
        Type = type;
    }
}