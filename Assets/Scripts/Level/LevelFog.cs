using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelFog : MonoBehaviour
{
    private GridXY<TileVisibility> grid;
    [SerializeField] private Sprite sprUnknown;

    [Header("Debug")]
    public bool hideLevel = true;
    public bool showDebugColors = false;
    public bool showDebugLog = false;

    private void Awake()
    {
        grid = new GridXY<TileVisibility>();
        
        Singletons.main.lvlManager.grid.OnGridCreated += (object sender, GridCreationEventArgs args) =>
        {
            grid.CreateGridXY(args.width, args.height, args.cellSize, args.originPosition);
            grid.SetAllTiles(hideLevel ? TileVisibility.Invisible : TileVisibility.Visible);
        };
        Singletons.main.lvlManager.grid.OnGridObjectChanged += (object sender, GridXY<TileType>.GridObjectChangedEventArgs args) =>
        {
            if (hideLevel)
            {
                HideTile(args.x, args.y);
                if (args.value == TileType.Goal)
                    DiscoverTile(args.x, args.y);
            } else
                DiscoverTile(args.x, args.y);
        };

        grid.OnGridObjectChanged += (object sender, GridXY<TileVisibility>.GridObjectChangedEventArgs args) =>
        {
            Color visualColor = Color.black;
            if (showDebugColors)
                if (args.value == TileVisibility.Visible)
                    visualColor = new Color(0.1529f, 0.6823f, 0.3764f, 1.0f); //green
                else if (args.value == TileVisibility.ReadyToVisible)
                    visualColor = new Color(0.9019f, 0.4941f, 0.1333f, 1.0f); //orange
                else if (args.value == TileVisibility.Invisible)
                    visualColor = new Color(0.7529f, 0.2235f, 0.1686f, 1.0f); //red

            if (args.value == TileVisibility.Visible)
                Singletons.main.lvlVisual.ResetTileVisual(args.x, args.y, visualColor);
            else
                Singletons.main.lvlVisual.SetTileVisual(args.x, args.y, sprUnknown, visualColor);

            CheckTilesVisibilityAround(args.x, args.y);
        };

        LevelManager.OnLevelReady += (object sender, LevelManager.OnLevelReadyEventArgs args) => CheckNullTiles();

        Player.OnPlayerStartedMove += (object sender, GridCoordsEventArgs args) => DiscoverTile(args.x, args.y);
        Player.OnPlayerStoppedMove += (object sender, GridCoordsEventArgs args) =>
        {
            DiscoverTile(args.x + 1, args.y);
            DiscoverTile(args.x - 1, args.y);
            DiscoverTile(args.x, args.y + 1);
            DiscoverTile(args.x, args.y - 1);
        };
    }

    private void SetTileVisibility(int x, int y, TileVisibility visibility)
    {
        if (grid.CellIsValid(x, y) && (visibility == TileVisibility.Invisible || grid.GetTile(x, y) != visibility))
        {
            if(showDebugLog) Debug.Log($"Setting Tile {x},{y} Visibility ({visibility})");
            grid.SetTile(x, y, visibility);
        }
    }
    private void DiscoverTile(int x, int y) { SetTileVisibility(x, y, TileVisibility.Visible); }
    private void HideTile(int x, int y) { SetTileVisibility(x, y, TileVisibility.Invisible); }

    private void CheckNullTiles()
    {
        for(int x = 0; x < grid.width; x++)
            for (int y = 0; y < grid.height; y++)
                IsNullTileReadyToVisible(x, y);
    }

    private bool IsNullTileReadyToVisible(int x, int y)
    {
        bool isReadyToVisible = false;

        if (grid.CellIsValid(x, y))
        {
            if (Singletons.main.lvlManager.grid.GetTile(x, y) == TileType.NULL && grid.GetTile(x, y) == TileVisibility.Invisible)
            {
                if (showDebugLog) Debug.Log($"Is Tile {x},{y} ReadyToVisible?");
                isReadyToVisible = true;

                TileType type;
                TileVisibility visibility;

                List<Vector2Int> neighbours = MyUtils.GatherNeighbours(x, y, 1, true, false);
                Vector2Int neighbour;
                for (int i = 0; i < neighbours.Count && isReadyToVisible; i++)
                {
                    neighbour = neighbours[i];
                    if (Singletons.main.lvlManager.grid.CellIsValid(neighbour.x, neighbour.y) && grid.CellIsValid(neighbour.x, neighbour.y))
                    {
                        type = Singletons.main.lvlManager.grid.GetTile(neighbour.x, neighbour.y);
                        visibility = grid.GetTile(neighbour.x, neighbour.y);
                        isReadyToVisible = visibility == TileVisibility.Visible || type == TileType.NULL;
                        if (showDebugLog) Debug.Log($"Checked neighbour {x},{y} ({visibility}) => {isReadyToVisible}");
                    }
                }

                if (isReadyToVisible)
                    grid.SetTile(x, y, TileVisibility.ReadyToVisible);
            } else
                isReadyToVisible = grid.GetTile(x, y) != TileVisibility.ReadyToVisible;
        }

        return isReadyToVisible;
    }

    private void CheckTilesVisibilityAround(int x, int y)
    {
        TileType type = Singletons.main.lvlManager.grid.GetTile(x, y);
        TileVisibility visibility = grid.GetTile(x, y);

        if (showDebugLog) Debug.Log($"Checking Tiles Visibility around {x},{y} ({visibility})");

        if (type != TileType.NULL && visibility == TileVisibility.Visible)
        {
            List<Vector2Int> neighbours = MyUtils.GatherNeighbours(x, y, 1, true, false);
            foreach (Vector2Int neighbour in neighbours)
                IsNullTileReadyToVisible(neighbour.x, neighbour.y);
        } else if (type == TileType.NULL && visibility == TileVisibility.ReadyToVisible)
        {
            if (showDebugLog) Debug.Log("Checking for cluster completion");

            List<Vector2Int> cellsChecked = new List<Vector2Int>() { };
            bool clusterIsOk = CheckClusterTileVisibility(x, y, ref cellsChecked);
            if (clusterIsOk)
                foreach (Vector2Int cell in cellsChecked)
                    DiscoverTile(cell.x, cell.y);
        }
    }

    private bool CheckClusterTileVisibility(int x, int y, ref List<Vector2Int> cellsChecked)
    {
        Vector2Int cell = new Vector2Int(x, y);

        if (!cellsChecked.Contains(cell))
            cellsChecked.Add(cell);

        bool result = grid.CellIsValid(x, y) && grid.GetTile(x, y) != TileVisibility.Invisible;

        if (showDebugLog) Debug.Log($"Checking cluster Tile {x},{y} ({grid.GetTile(x, y)}) = {result}");

        List<Vector2Int> neighbours = MyUtils.GatherNeighbours(x, y, 1, true, true);
        Vector2Int neighbour;
        for (int i = 0; i < neighbours.Count && result; i++)
        {
            neighbour = neighbours[i];
            if (grid.CellIsValid(neighbour.x, neighbour.y))
                if (!cellsChecked.Contains(neighbour))
                    if (Singletons.main.lvlManager.grid.GetTile(neighbour.x, neighbour.y) == TileType.NULL && grid.GetTile(neighbour.x, neighbour.y) != TileVisibility.Visible)
                        result = CheckClusterTileVisibility(neighbour.x, neighbour.y, ref cellsChecked);
        }

        return result;
    }
}