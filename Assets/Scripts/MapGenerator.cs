using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : AMapGenerator
{
    private Grid _grid;
    public Grid FirstNode;
    public Grid[] AvailableRooms;
    
    public override void GenerateMap(Grid grid)
    {
        _grid = grid;
        FirstNode.GetComponent<Room>().Pos = new Vector2Int(0, 0);
        GenerateFromRoom(FirstNode, 1);
    }

    public bool GenerateFromRoom(Grid room, float prob)
    {
        Room theRoom = room.GetComponent<Room>();
        Grid NewRoom;
        
        foreach (RoomDoor door in theRoom.Doors)
        {
            if (Random.Range(0, 100) < prob * 100) {
                if (!door.isLink) {
                    NewRoom = GenerateFromDoor(door, room.GetComponent<Room>().DoorPos(door));
                    Debug.Log("(" + NewRoom.GetComponent<Room>().Pos.x + ", " + NewRoom.GetComponent<Room>().Pos.x + ")");
                    GenerateFromRoom(NewRoom, 3 * prob / 4);
                    AddToGrid(NewRoom);
                }
            }
        }
        return true;
    }

    public void CopySquare(Tilemap dest, Tilemap from, Vector2Int pos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                dest.SetTile(new Vector3Int(x + pos.x, y + pos.y, 0), from.GetTile(new Vector3Int(x, y, 0)));
            }
        }
    }
    
    public void AddToGrid(Grid toAdd)
    {
        Tilemap[] tiles = toAdd.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tmp in tiles)
        {
        }
    }
    
    public Grid GenerateFromDoor(RoomDoor door, Vector2Int pos)
    {
        Grid room;
        Vector2Int? roomPos;
        Direction.to dir;
        
        do
        {
            do
            {
                dir = Direction.IntToDir(Random.Range(0, 4));
            } while (dir == Direction.GetOpposide(door.Dir));
            pos = Direction.GoAuto(pos, 3, dir);
            room = AvailableRooms[Random.Range(0, AvailableRooms.Length)];
            roomPos = CanInsertRoom(room.GetComponent<Room>(), pos);
        } while (roomPos == null);
        Vector2Int tmp = roomPos.GetValueOrDefault();
        room.GetComponent<Room>().Pos = new Vector2Int(tmp.x, tmp.y);
        return room;
    }
    
    public Vector2Int? CanInsertRoom(Room room, Vector2Int pos)
    {
        foreach (RoomDoor door in room.Doors)
        {
            pos = Direction.GoAuto(pos, 1, door.Dir);
            pos = room.PosFromDoor(door, pos);
            if (room.TestPos(_grid, pos))
            {
                door.isLink = true;
                return pos;
            }
        }
        return null;
    }
}