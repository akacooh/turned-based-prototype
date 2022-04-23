using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } 

    private List<Unit> unitsList;

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
        grid = GridHelper.GenerateGrid(gridWidth, gridHeight, tilePrefab);
        dummy.transform.position = GridHelper.GridToCoord(new Vector2Int(10, 4));

    }

    private void Update() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveUnit();
        }
        
    }

    private void MoveUnit()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector2Int distance = GridHelper.CoordToGrid(hit.point) - GridHelper.CoordToGrid(unitsList[0].transform.position);
            bool canMove = distance.magnitude <= unitsList[0].speed;
            if (canMove)
            {
                unitsList[0].Move(hit.point);
            }
        }
    }


}
