using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int Size;
    public RoomDoor[] Doors;
    private Vector2Int Pos;

    public static bool CheckDoor(RoomDoor Door)
    {
        return Door && Door.isValid();
    }
   
    public bool AddToGrid(Grid grid)
    {
        /* Merge content + grid aux coord pos*/
        return true;
    }
}