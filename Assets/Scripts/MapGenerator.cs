using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : AMapGenerator
{
    private Grid _grid;
    public Grid FirstNode;
    public Grid[] AvailableRooms;
    public Tile PathTile;
    
    [Range(0, 100)]
    public ushort PathSize = 15;
    
    [Range(0, 100)]
    public ushort StationsSpawnRate = 15;
    
    [Range(1, 500)]
    public int cap = 100;

    [Range(0.01f, 0.99f)]
    public float Rate = 1f;

    public override void GenerateMap(Grid grid)
    {
        _grid = grid;
        FirstNode.GetComponent<Room>().Pos = new Vector2Int(0, 0);
        FirstNode.GetComponent<Room>().From = Direction.to.NONE;
        AddToGrid(FirstNode);
        GenerateFromRoom(FirstNode, 1, Direction.to.NONE);
    }

    public bool GenerateFromRoom(Grid room, float prob, Direction.to from)
    {
        if (cap <= 0) return false;
        cap--;
        if (room == null)
            return false;
        Room theRoom = room.GetComponent<Room>();

        foreach (RoomDoor door in theRoom.Doors)
        {
            if (!(Random.Range(0, 100) < prob * 100) || door.Dir == room.GetComponent<Room>().From) continue;
            Grid newRoom = GenerateFromDoor(door, room.GetComponent<Room>().DoorPos(door));
            AddToGrid(newRoom);
            GenerateFromRoom(newRoom, prob * Rate, from);
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
                TileBase t = from.GetTile(new Vector3Int(x, y, 0));
                if (t != null)
                    dest.SetTile(new Vector3Int(x + pos.x, y + pos.y, 0), t);
            }
        }
    }

    public void AddToGrid(Grid toAdd)
    {
        if (toAdd == null)
            return;
        Tilemap[] tiles = toAdd.GetComponentsInChildren<Tilemap>();
        Room room = toAdd.GetComponent<Room>();
        foreach (Tilemap tmp in tiles)
        {
            Tilemap tilemap = _grid
                .GetComponentsInChildren<Tilemap>()
                .FirstOrDefault(t => t.gameObject.name == tmp.gameObject.name);
            if (tilemap == null)
            {
                Debug.Log(tmp.gameObject.name + " " + tmp.gameObject.tag);
                GameObject go = new GameObject(tmp.gameObject.name) {tag = tmp.gameObject.tag};
                go.transform.parent = _grid.transform;
                tilemap = go.AddComponent<Tilemap>();
                TilemapRenderer r = go.AddComponent<TilemapRenderer>();
                r.sortingLayerName = tmp.GetComponent<TilemapRenderer>().sortingLayerName;
            }

            CopySquare(tilemap, tmp, room.Pos);
        }

        if (room.RadarSpawns.Length > 0 && Random.Range(0, 100) < StationsSpawnRate)
        {
            uint size = (uint) room.RadarSpawns.Length;
            Vector2Int stationPosition = room.RadarSpawns[(int) Random.Range(0, size)];
            stationPosition.x += room.Pos.x;
            stationPosition.y += room.Pos.y;
            GameManager.Instance.Map.Stations.Add(new Vector3Int(stationPosition.x, stationPosition.y, 0));
        }

        foreach (GuardController guard in toAdd.GetComponentsInChildren<GuardController>())
        {
            GameObject go = Instantiate(guard.gameObject);
            go.transform.parent = _grid.transform;
            go.transform.position += new Vector3(room.Pos.x, room.Pos.y);
        }
    }

    public Grid GenerateFromDoor(RoomDoor door, Vector2Int pos)
    {
        Grid room;
        Vector2Int? roomPos;
        int length = 1;
        Vector2Int initPos = pos;
        Tilemap tmp = _grid.GetComponentsInChildren<Tilemap>().FirstOrDefault(t => t.gameObject.name == "Ground");
        int maxTry = 0;
        do
        {
            if (maxTry > 100) return null;
            for (int i = 0; i < PathSize; i++) {
                pos = Direction.GoAuto(new Vector2Int(pos.x, pos.y), 1, door.Dir);
                length++;
            }
            if (tmp && !TestCol(tmp, pos, new Vector2Int(1, 1)))
                return null;
            room = AvailableRooms[Random.Range(0, AvailableRooms.Length)];
            roomPos = CanInsertRoom(room.GetComponent<Room>(), new Vector2Int(pos.x, pos.y), door.Dir);    
        } while (roomPos == null);
        
        drawPath(pos, length, room.GetComponent<Room>().From);
        room.GetComponent<Room>().Pos = roomPos.GetValueOrDefault();
        return room;
    }

    public void drawPath(Vector2Int pos, int length, Direction.to dir)
    {
        for (int i = 0; i < length; i++) {
            Tilemap firstOrDefault = _grid.GetComponentsInChildren<Tilemap>()
                .FirstOrDefault(t => t.gameObject.name == "Path");
            if (firstOrDefault != null)
                firstOrDefault
                    .SetTile(new Vector3Int(pos.x, pos.y, 0), PathTile);
            pos = Direction.GoAuto(new Vector2Int(pos.x, pos.y), 1, dir);
        }
    }
    
    public Vector2Int? CanInsertRoom(Room room, Vector2Int pos, Direction.to dir)
    {
        Vector2Int roomPos;
        Tilemap tmp;
        foreach (RoomDoor door in room.Doors)
        {
            if (Direction.GetOpposite(door.Dir) != dir) continue;
            room.From = door.Dir;
            roomPos = room.PosFromDoor(door, pos);
            tmp = _grid.GetComponentsInChildren<Tilemap>().FirstOrDefault(t => t.gameObject.name == "Path");
            if (!TestCol(tmp, roomPos, room.Size)) continue;
            tmp = _grid.GetComponentsInChildren<Tilemap>().FirstOrDefault(t => t.gameObject.name == "Ground");
            if (!TestCol(tmp, roomPos, room.Size)) continue;
            return roomPos;
        }
        return null;
    }

    public bool TestCol(Tilemap toTest, Vector2Int pos, Vector2Int size)
    {
        for (int x = pos.x - 1; x < pos.x + size.x + 1; x++)
        {
            for (int y = pos.y - 1; y < pos.y + size.y + 1; y++)
            {
                if (toTest.HasTile(new Vector3Int(x, y, 0)))
                    return false;
            }
        }
        return true;
    }
}