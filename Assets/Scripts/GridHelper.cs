using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHelper
{
    public static Tile[,] GenerateGrid(int width, int height, GameObject tilePrefab)
    {
        var grid = new Tile[width, height];
        var gridObject = new GameObject("Grid");

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = MonoBehaviour.Instantiate(tilePrefab, GridHelper.GridToCoord(new Vector2Int(x,y)), tilePrefab.transform.rotation, gridObject.transform);
                tile.name = $"Tile {x}:{y}";
                grid[x, y] = tile.GetComponent<Tile>();
            }
        }
        return grid;
    }

    public static Vector2Int CoordToGrid(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
    }

    public static Vector3 GridToCoord(Vector2Int cell)
    {
        return new Vector3(cell.x + 0.5f, 0, cell.y + 0.5f);
    }
}
