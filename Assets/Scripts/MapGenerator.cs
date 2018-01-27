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
    public ushort PathSize = 15;

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
        Room theRoom = room.GetComponent<Room>();

        foreach (RoomDoor door in theRoom.Doors)
        {
            Debug.Log("FROM ===> " + room.GetComponent<Room>().From);
            if (!(Random.Range(0, 100) < prob * 100) || door.Dir == room.GetComponent<Room>().From) continue;
            Grid newRoom = GenerateFromDoor(door, room.GetComponent<Room>().DoorPos(door));
            AddToGrid(newRoom);
            GenerateFromRoom(newRoom, 3 * prob / 4, from);
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
        do
        {
            for (int i = 0; i < PathSize; i++)
            {
                pos = Direction.GoAuto(new Vector2Int(pos.x, pos.y), 1, door.Dir);
                Tilemap firstOrDefault = _grid.GetComponentsInChildren<Tilemap>()
                    .FirstOrDefault(t => t.gameObject.name == "Path");
                if (firstOrDefault != null)
                    firstOrDefault
                        .SetTile(new Vector3Int(pos.x, pos.y, 0), PathTile);
            }
            pos = Direction.GoAuto(new Vector2Int(pos.x, pos.y), 1, door.Dir);
            room = AvailableRooms[Random.Range(0, AvailableRooms.Length)];
            roomPos = CanInsertRoom(room.GetComponent<Room>(), new Vector2Int(pos.x, pos.y), door.Dir);
        } while (roomPos == null);
        room.GetComponent<Room>().Pos = roomPos.GetValueOrDefault();
        return room;
    }

    public Vector2Int? CanInsertRoom(Room room, Vector2Int pos, Direction.to dir)
    {
        foreach (RoomDoor door in room.Doors)
        {
            if (Direction.GetOpposite(door.Dir) != dir) continue;
            room.From = door.Dir;
            return room.PosFromDoor(door, pos);
        }
        return null;
    }
}