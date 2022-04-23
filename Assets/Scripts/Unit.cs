using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public int speed;
    public int health;

    public virtual void Move(Vector3 position)
    {

    }
}
