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

    public Vector2Int SizeFromDoor(RoomDoor door, Vector2Int pos)
    {
        switch (door.Dir)
        {
            case Direction.to.NORTH:
                return new Vector2Int(pos.x - Pos.x, pos.y);
            case Direction.to.WEST:
                return new Vector2Int(pos.x, pos.y - Pos.y); 
            case Direction.to.SOUTH:
                return new Vector2Int(pos.x - Pos.x, pos.y - Size.y);
            case Direction.to.EAST:
                return new Vector2Int(pos.x - Size.x, pos.y - Pos.y);
            default:
                return pos;
        }
    }
}