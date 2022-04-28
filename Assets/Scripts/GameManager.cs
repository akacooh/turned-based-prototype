using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } 

    public List<Unit> unitsList { get; private set; }
    private int selectedUnit;
    private bool isMoving, isAttacking;

    [SerializeField] private Material tileNormal, tileMovement, tileAttack;
    private Dictionary<TileMat, Material> matDict;
    [SerializeField] private LayerMask layerMask;

    public Tile[,] grid;
    private List<MeshRenderer> changedTilesRenderers;
    public int gridHeight;
    public int gridWidth;
    [SerializeField] private GameObject tilePrefab;
    public GameObject dummy;
    public UnityEvent OrderChanged;
    

    [SerializeField] private Camera mainCamera;

    private void Awake() 
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        } 
    }

    private void Start() 
    {
        matDict = new Dictionary<TileMat, Material>();
        changedTilesRenderers = new List<MeshRenderer>();
        matDict[TileMat.attack] = tileAttack;
        matDict[TileMat.normal] = tileNormal;
        matDict[TileMat.move] = tileMovement;

        unitsList = new List<Unit>();
        var units = GameObject.FindGameObjectsWithTag("Player");


        selectedUnit = 0;

        grid = GridHelper.GenerateGrid(gridWidth, gridHeight, tilePrefab);
        dummy.transform.position = GridHelper.GridToCoord(new Vector2Int(10, 4));
        grid[10, 4].isOccupied = true;
        grid[10, 4].occupiedBy = dummy.GetComponent<Unit>();

        int x = 0,  y = 0; //test
        foreach (var unit in units)
        {
            var unitComponent = unit.GetComponentInParent<Unit>();
            unitsList.Add(unitComponent);
            //test deployment
            unit.transform.position = GridHelper.GridToCoord(new Vector2Int(x, y));
            grid[x, y].isOccupied = true;
            grid[x, y].occupiedBy = unitComponent;
            x++;
            y++;
        }
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ClearTiles();
            Vector2Int currentPos = GridHelper.CoordToGrid(unitsList[selectedUnit].transform.position);
            Dictionary<Vector2Int, Vector2Int?> reachableTiles = GridHelper.GetReachableTiles(grid, currentPos, unitsList[selectedUnit].speed);
            List<Vector2Int> tiles = new List<Vector2Int>();
            foreach (var key in reachableTiles.Keys)
            {
                tiles.Add(key);
            }
            DrawTiles(tiles, TileMat.move);
            isMoving = true;
        }
        if ((isMoving || isAttacking) && Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ClearTiles();
            Vector2Int currentPos = GridHelper.CoordToGrid(unitsList[selectedUnit].transform.position);
            List<Vector2Int> tiles = GridHelper.GetTilesInRange(grid, currentPos, unitsList[selectedUnit].attackRange);
            DrawTiles(tiles, TileMat.attack);
            isAttacking = true;
        }
    }

    private void OnClick()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f,layerMask))
        {
            var tileHit = GridHelper.CoordToGrid(hit.point);
            var tile = new Vector2Int(tileHit.x, tileHit.y);

            if (isMoving)
            {
                Vector2Int currentPos = GridHelper.CoordToGrid(unitsList[selectedUnit].transform.position);
                Dictionary<Vector2Int, Vector2Int?> reachableTiles = GridHelper.GetReachableTiles(grid, currentPos, unitsList[selectedUnit].speed);
                if (reachableTiles.ContainsKey(tile))
                {
                    grid[currentPos.x, currentPos.y].isOccupied = false;
                    grid[currentPos.x, currentPos.y].occupiedBy = null;
                    unitsList[selectedUnit].Move(hit.point);
                    unitsList[selectedUnit].actionPoints -= 1;
                    grid[tile.x, tile.y].isOccupied = true;
                    grid[tile.x, tile.y].occupiedBy = unitsList[selectedUnit];
                    isMoving = false;
                    ClearTiles();
                }
            }
            else if (isAttacking)
            {
                
                Vector2Int currentPos = GridHelper.CoordToGrid(unitsList[selectedUnit].transform.position);
                List<Vector2Int> reachableTiles = GridHelper.GetTilesInRange(grid, currentPos, unitsList[selectedUnit].attackRange);
                if (reachableTiles.Contains(tile) && grid[tile.x, tile.y].occupiedBy.CompareTag("Enemy"))
                {
                    Debug.Log("hit");
                    unitsList[selectedUnit].Attack(grid[tile.x, tile.y].occupiedBy);
                    unitsList[selectedUnit].actionPoints -= 2;
                    isAttacking = false;
                    ClearTiles();
                }
            }

            if (unitsList[selectedUnit].actionPoints <= 0)
            {
                EndTurn();
            }
        }
    }

    //changes color of tiles at <=radius from tile on startPos. undo if clearFlag set to true
/*     private void GetAdjacentTiles(Vector2Int startPos, int radius, TileMat tileMat)
    {
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -(radius - Mathf.Abs(i)); j <= radius - Mathf.Abs(i); j++)
            {
                int X = startPos.x + i;
                int Y = startPos.y + j;
                if (X >= 0 && Y >= 0 && X < gridWidth && Y < gridHeight)
                {
                    MeshRenderer renderer = grid[X, Y].GetComponent<MeshRenderer>();
                    Action(renderer);
                }
            }
        }
    } */

    private void ClearTiles()
    {
        foreach (MeshRenderer tile in changedTilesRenderers)
        {
            tile.material = matDict[TileMat.normal];
        }
        changedTilesRenderers.Clear();
    }

    private void DrawTiles(List<Vector2Int> coordsList, TileMat tileMat)
    {
        foreach (Vector2Int coords in coordsList)
        {
            MeshRenderer renderer = grid[coords.x, coords.y].GetComponent<MeshRenderer>();
            changedTilesRenderers.Add(renderer);
            renderer.material = matDict[tileMat];
        }
    }

    public void EndTurn()
    {
        ClearTiles();
        unitsList[selectedUnit].actionPoints = 2;
        selectedUnit++;
        OrderChanged.Invoke();
        if (selectedUnit == unitsList.Count)
        {
            selectedUnit = 0;
        }
    }
}

enum TileMat
{
    attack,
    normal,
    move
}
