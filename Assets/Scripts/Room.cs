using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public Vector2Int Size;
    public RoomDoor[] Doors;
    public Vector2Int Pos { get; set; }

    public static bool CheckDoor(RoomDoor Door)
    {
        return Door && Door.isValid();
    }

    public Vector2Int PosFromDoor(RoomDoor door, Vector2Int pos)
    {
        switch (door.Dir)
        {
            case Direction.to.NORTH:
                return new Vector2Int(pos.x - door.Pos, pos.y);
            case Direction.to.WEST:
                return new Vector2Int(pos.x, pos.y - door.Pos); 
            case Direction.to.SOUTH:
                return new Vector2Int(pos.x - door.Pos, pos.y - Size.y);
            case Direction.to.EAST:
                return new Vector2Int(pos.x - Size.x, pos.y - door.Pos);
            default:
                return pos;
        }
    }

    public Vector2Int DoorPos(RoomDoor door)
    {
        switch (door.Dir)
        {
            case Direction.to.NORTH:
                return new Vector2Int(Pos.x + door.Pos, Pos.y);
            case Direction.to.WEST:
                return new Vector2Int(Pos.x, Pos.y + door.Pos); 
            case Direction.to.SOUTH:
                return new Vector2Int(Pos.x + door.Pos, Pos.y + Size.y);
            case Direction.to.EAST:
                return new Vector2Int(Pos.x + Size.x, Pos.y + door.Pos);
            default:
                return new Vector2Int(Pos.x, Pos.y);
        }
    }

    public bool TestPos(Grid grid, Vector2Int pos)
    {
        foreach (Tilemap map in grid.GetComponentsInChildren<Tilemap>())
        {
            for (int x = pos.x; x < Size.x + pos.x; pos.x++) {
                for (int y = pos.y; y < Size.y + pos.y; pos.y++) {
                    if (map.HasTile(new Vector3Int(x, y, 0)))
                        return false;
                }
            }                  
        }
        return true;
    }
}