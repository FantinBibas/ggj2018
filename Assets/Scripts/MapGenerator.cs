using System.Linq;
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
        AddToGrid(FirstNode);
        GenerateFromRoom(FirstNode, 1);
    }

    public bool GenerateFromRoom(Grid room, float prob)
    {
        Room theRoom = room.GetComponent<Room>();

        foreach (RoomDoor door in theRoom.Doors)
        {
            if (!(Random.Range(0, 100) < prob * 100)) continue;
            if (door.isLink) continue;
            Grid newRoom = GenerateFromDoor(door, room.GetComponent<Room>().DoorPos(door));
            Debug.Log("(" + newRoom.GetComponent<Room>().Pos.x + ", " + newRoom.GetComponent<Room>().Pos.x + ")");
            GenerateFromRoom(newRoom, 3 * prob / 4);
            AddToGrid(newRoom);
        }
        return true;
    }

    public void CopySquare(Tilemap dest, Tilemap from, Vector2Int pos)
    {
        BoundsInt bounds = from.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
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
            Tilemap tilemap = _grid
                .GetComponentsInChildren<Tilemap>()
                .FirstOrDefault(t => t.gameObject.name == tmp.gameObject.name);
            if (tilemap == null)
            {
                GameObject go = new GameObject(tmp.gameObject.name);
                go.transform.parent = _grid.transform;
                tilemap = go.AddComponent<Tilemap>();
                TilemapRenderer r = go.AddComponent<TilemapRenderer>();
                r.sortingLayerName = tmp.GetComponent<TilemapRenderer>().sortingLayerName;
            }
            Room room = toAdd.GetComponent<Room>();
            CopySquare(tilemap, tmp, room.Pos);
        }
    }

    public Grid GenerateFromDoor(RoomDoor door, Vector2Int pos)
    {
        Grid room;
        Vector2Int? roomPos;

        do
        {
            Direction.to dir;
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
            if (!room.TestPos(_grid, pos)) continue;
            door.isLink = true;
            return pos;
        }
        return null;
    }
}