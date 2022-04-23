using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int attackRange;
    public int speed;
    public int health;
    public int strength;

    public virtual void Move(Vector3 position)
    {
        position = GridHelper.GridToCoord(GridHelper.CoordToGrid(position));
        transform.position = position;
    }

    public virtual void Attack(Unit enemy)
    {
        enemy.health -= strength;
        Debug.Log("Attack commited");
    }
}
