using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } 

    private List<Unit> unitsList;
    private Unit selectedUnit;
    private bool isMoving, isAttacking;

    [SerializeField] private Material tileNormal, tileMovement, tileAttack;

    public Tile[,] grid;
    public int gridHeight;
    public int gridWidth;
    [SerializeField] private GameObject tilePrefab;
    public GameObject dummy;
    

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
        unitsList = new List<Unit>();
        unitsList.Add(GameObject.FindWithTag("Player").GetComponentInParent<Unit>());
        selectedUnit = unitsList[0];

        grid = GridHelper.GenerateGrid(gridWidth, gridHeight, tilePrefab);
        dummy.transform.position = GridHelper.GridToCoord(new Vector2Int(10, 4));
        grid[10, 4].isOccupied = true;
        grid[10, 4].occupiedBy = dummy.GetComponent<Unit>();

    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetAdjacentTiles(GridHelper.CoordToGrid(selectedUnit.transform.position), selectedUnit.speed, ShowMovementRange);
            isMoving = true;
        }
        if ((isMoving || isAttacking) && Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetAdjacentTiles(GridHelper.CoordToGrid(selectedUnit.transform.position), selectedUnit.attackRange, ShowAttackRange);
            isAttacking = true;
        }
    }

    private void OnClick()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var tileHit = GridHelper.CoordToGrid(hit.point);
            Vector2Int distance = tileHit - GridHelper.CoordToGrid(unitsList[0].transform.position);
            if (isMoving)
            {
                bool canMove = distance.magnitude <= unitsList[0].speed;
                if (canMove)
                {
                    GetAdjacentTiles(GridHelper.CoordToGrid(selectedUnit.transform.position), selectedUnit.speed, ClearTiles);
                    unitsList[0].Move(hit.point);
                    isMoving = false;
                }
            }
            else if (isAttacking)
            {
                var tile = grid[tileHit.x, tileHit.y];
                bool canAttack = (distance.magnitude <= unitsList[0].attackRange) && tile.isOccupied;
                if (canAttack && tile.occupiedBy.transform.CompareTag("Enemy"))
                {
                    Debug.Log("hit");
                    GetAdjacentTiles(GridHelper.CoordToGrid(selectedUnit.transform.position), selectedUnit.speed, ClearTiles);
                    unitsList[0].Attack(tile.occupiedBy);
                    isAttacking = false;
                }
            }
        }
    }

    //changes color of tiles at <=radius from tile on startPos. undo if clearFlag set to true
    private void GetAdjacentTiles(Vector2Int startPos, int radius, UnityAction<MeshRenderer> Action)
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
    }

    private void ShowMovementRange(MeshRenderer renderer)
    {
        renderer.material = tileMovement;
    }

    private void ShowAttackRange(MeshRenderer renderer)
    {
        renderer.material = tileAttack;
    }
    
    private void ClearTiles(MeshRenderer renderer)
    {
        renderer.material = tileNormal;
    }
}
