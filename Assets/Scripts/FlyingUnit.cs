using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnit : Unit
{
    public override void Move(Vector3 position)
    {
    position = GridHelper.GridToCoord(GridHelper.CoordToGrid(position));
    transform.position = position;
    }
}
