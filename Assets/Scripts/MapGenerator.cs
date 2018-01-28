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
    public Tile WallTile;
    public Tile FillTile;
    public ushort PathSize = 15;

    [Range(1, 1000)] public int Cap = 100;

    [Range(0.01f, 0.99f)] public float Rate = 1f;

    private BoundsInt GetGridBounds()
    {
        BoundsInt bounds = new BoundsInt();
        foreach (Tilemap tm in _grid.GetComponentsInChildren<Tilemap>())
        {
            BoundsInt cellBounds = tm.cellBounds;
            if (bounds.xMin > cellBounds.xMin)
                bounds.xMin = cellBounds.xMin;
            if (bounds.xMax < cellBounds.xMax)
                bounds.xMax = cellBounds.xMax;
            if (bounds.yMin > cellBounds.yMin)
                bounds.yMin = cellBounds.yMin;
            if (bounds.yMax < cellBounds.yMax)
                bounds.yMax = cellBounds.yMax;
        }
        return bounds;
    }

    private void FillWalls()
    {
        if (WallTile == null)
            return;
        GameObject go = new GameObject("__walls");
        Tilemap tm = go.AddComponent<Tilemap>();
        TilemapRenderer r = go.AddComponent<TilemapRenderer>();
        r.sortingLayerName = "Objects";
        go.tag = "Solid";
        BoundsInt bounds = GetGridBounds();
        Tilemap[] tilemaps = _grid.GetComponentsInChildren<Tilemap>()
            .Where(t => !t.gameObject.CompareTag("Ignore")).ToArray();
        for (int x = bounds.xMin - 1; x < bounds.xMax + 1; x++)
        {
            for (int y = bounds.yMin - 1; y < bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemaps.Any(t => t.HasTile(pos)))
                    continue;
                Vector3Int top = new Vector3Int(x, y + 1, 0);
                Vector3Int left = new Vector3Int(x + 1, y, 0);
                Vector3Int bot = new Vector3Int(x, y - 1, 0);
                Vector3Int right = new Vector3Int(x - 1, y, 0);
                if (tilemaps.Any(t => t.HasTile(top) || t.HasTile(left) || t.HasTile(bot) || t.HasTile(right)))
                    tm.SetTile(pos, WallTile);
            }
        }
        go.transform.parent = _grid.transform;
    }

    private void FillEmpty()
    {
        if (FillTile == null)
            return;
        GameObject go = new GameObject("__fill");
        Tilemap tm = go.AddComponent<Tilemap>();
        TilemapRenderer r = go.AddComponent<TilemapRenderer>();
        r.sortingLayerName = "Ground";
        if (WallTile != null)
            go.tag = "Ignore";
        BoundsInt bounds = GetGridBounds();
        Tilemap[] tilemaps = _grid.GetComponentsInChildren<Tilemap>()
            .Where(t => !t.gameObject.CompareTag("Ignore")).ToArray();
        for (int x = bounds.xMin - 1; x < bounds.xMax + 1; x++)
        {
            for (int y = bounds.yMin - 1; y < bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemaps.Any(t => t.HasTile(pos)))
                    continue;
                tm.SetTile(pos, FillTile);
            }
        }
        go.transform.parent = _grid.transform;
    }

    public override void GenerateMap(Grid grid)
    {
        _grid = grid;
        FirstNode.GetComponent<Room>().Pos = new Vector2Int(0, 0);
        FirstNode.GetComponent<Room>().From = Direction.to.NONE;
        AddToGrid(FirstNode);
        GenerateFromRoom(FirstNode, 1, Direction.to.NONE);
        FillWalls();
        FillEmpty();
    }

    public bool GenerateFromRoom(Grid room, float prob, Direction.to from)
    {
        if (Cap <= 0) return false;
        Cap--;
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
        do
        {
            for (int i = 0; i < PathSize; i++)
            {
                pos = Direction.GoAuto(new Vector2Int(pos.x, pos.y), 1, door.Dir);
                length++;
            }
            if (tmp && !TestCol(tmp, pos, new Vector2Int(1, 1)))
                return null;
            room = AvailableRooms[Random.Range(0, AvailableRooms.Length)];
            roomPos = CanInsertRoom(room.GetComponent<Room>(), new Vector2Int(pos.x, pos.y), door.Dir);
        } while (roomPos == null);

        DrawPath(pos, length, room.GetComponent<Room>().From);
        room.GetComponent<Room>().Pos = roomPos.GetValueOrDefault();
        return room;
    }

    public void DrawPath(Vector2Int pos, int length, Direction.to dir)
    {
        for (int i = 0; i < length; i++)
        {
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
        foreach (RoomDoor door in room.Doors)
        {
            if (Direction.GetOpposite(door.Dir) != dir) continue;
            room.From = door.Dir;
            Vector2Int roomPos = room.PosFromDoor(door, pos);
            Tilemap tmp = _grid.GetComponentsInChildren<Tilemap>().FirstOrDefault(t => t.gameObject.name == "Path");
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