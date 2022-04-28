using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    //generating width x height grid of tiles
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

    //convert world position to grid indexes
    public static Vector2Int CoordToGrid(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
    }
    
    //convert grid indexes to snaped world position
    public static Vector3 GridToCoord(Vector2Int cell)
    {
        return new Vector3(cell.x + 0.5f, 0, cell.y + 0.5f);
    }

    //searching for tiles unit can get by land
    public static Dictionary<Vector2Int, Vector2Int?> GetReachableTiles(Tile[,] grid, Vector2Int startPos, int range)
    {
        var tilesToCheck = new Queue<Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int, int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int?>();   //result containing all reachable tiles as well as path
        int width = grid.GetUpperBound(0);
        int height = grid.GetUpperBound(1);

        costSoFar.Add(startPos, 0);
        tilesToCheck.Enqueue(startPos);
        cameFrom.Add(startPos, null);

        while (tilesToCheck.Count > 0)
        {
            startPos = tilesToCheck.Dequeue();
            List<Vector2Int> neighbours = GetAdjacentTiles(width, height, startPos);
            foreach (Vector2Int neighbour in neighbours)
            {
                if (grid[neighbour.x, neighbour.y].isOccupied)
                    continue;
                int cost = costSoFar[startPos] + 1;
                if (cost <= range)
                {
                    if (!cameFrom.ContainsKey(neighbour))
                    {
                        costSoFar.Add(neighbour, cost);
                        tilesToCheck.Enqueue(neighbour);
                        cameFrom.Add(neighbour, startPos);
                    }
                    else if (costSoFar[neighbour] > cost)
                    {
                        costSoFar[neighbour] = cost;
                        cameFrom[neighbour] = startPos;
                    }
                }
            }
            
        }

        return cameFrom;
    }

    public static List<Vector2Int> GetTilesInRange(Tile[,] grid, Vector2Int startPos, int range)
    {
        var result = new List<Vector2Int>();
        int width = grid.GetUpperBound(0);
        int height = grid.GetUpperBound(1);

        for (int x = startPos.x - range; x <= startPos.x + range; x++)
        {
            for (int y = startPos.y - range; y <= startPos.y + range; y++)
            {
                if (x >= 0 && x <= width && y >=0 && y <= height)
                {
                    //if (!grid[x, y].isOccupied)       not checking for now
                    //{
                        result.Add(new Vector2Int(x, y));
                    //}
                }
            }
        }

        return result;
    }

    public static List<Vector2Int> GetAdjacentTiles(int width, int height, Vector2Int startPos) //count from 0
    {
        var result = new List<Vector2Int>();

        var N = new Vector2Int(startPos.x, startPos.y + 1);    //north
        var S = new Vector2Int(startPos.x, startPos.y - 1);    //south
        var W = new Vector2Int(startPos.x - 1, startPos.y);    //west
        var E = new Vector2Int(startPos.x + 1, startPos.y);    //east

        if (N.y <= height)
            result.Add(N);
        if (S.y >= 0)
            result.Add(S);
        if (W.x >= 0)
            result.Add(W);
        if (E.x <= width)
            result.Add(E);

        return result;
    }
}
