using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public Vector2Int Size;
    public Direction.to From { get; set; }
    public Vector2Int[] RadarSpawns;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (Vector2Int radarSpawn in RadarSpawns)
        {
            Gizmos.DrawWireSphere(new Vector3(radarSpawn.x + 0.5f, radarSpawn.y + 0.5f, 0), 0.3f);
        }
    }

    private Vector2Int HalfSize
    {
        get { return new Vector2Int(Size.x / 2, Size.y / 2); }
    }

    public IEnumerable<RoomDoor> Doors
    {
        get { return GetComponents<RoomDoor>(); }
    }

    public Vector2Int Pos { get; set; }
    
    public static bool CheckDoor(RoomDoor Door)
    {
        return Door && Door.isValid();
    }

    public Vector2Int PosFromDoor(RoomDoor door, Vector2Int pos)
    {
        switch (door.Dir)
        {
            case Direction.to.SOUTH:
                return new Vector2Int(pos.x - door.Pos, pos.y);
            case Direction.to.NORTH:
                return new Vector2Int(pos.x - door.Pos, pos.y - Size.y + 1);
                
            case Direction.to.WEST:
                return new Vector2Int(pos.x, pos.y - door.Pos);
            case Direction.to.EAST:
                return new Vector2Int(pos.x - Size.x + 1, pos.y - door.Pos);
            default:
                return pos;
        }
    }

    public Vector2Int DoorPos(RoomDoor door)
    {
        switch (door.Dir)
        {
            case Direction.to.SOUTH:
                return new Vector2Int(Pos.x + door.Pos, Pos.y);
            case Direction.to.NORTH:
                return new Vector2Int(Pos.x + door.Pos, Pos.y + Size.y - 1);
                
            case Direction.to.WEST:
                return new Vector2Int(Pos.x, Pos.y + door.Pos);
            case Direction.to.EAST:
                return new Vector2Int(Pos.x + Size.x - 1, Pos.y + door.Pos);
                
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (RoomDoor door in GetComponents<RoomDoor>())
        {
            Vector2Int pos = DoorPos(door);
            Gizmos.DrawSphere(new Vector3(pos.x, pos.y, 0) + transform.position + new Vector3(0.5f, 0.5f, 0f), 0.2f);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(0, 0), new Vector3(Size.x, 0));
        Gizmos.DrawLine(new Vector3(0, 0), new Vector3(0, Size.y));
        Gizmos.DrawLine(new Vector3(Size.x, 0), new Vector3(Size.x, Size.y));
        Gizmos.DrawLine(new Vector3(0, Size.y), new Vector3(Size.x, Size.y));
    }
}