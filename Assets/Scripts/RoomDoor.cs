using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour {   
    public int Pos;
    public Direction.to Dir;
    public bool isLink;

    public bool isValid()
    {
        return Pos > 0;
    }
}