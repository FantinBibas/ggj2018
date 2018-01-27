using UnityEngine;

public class MapGenerator : AMapGenerator
{
    public Grid Grid;
    public Grid[] AvailableRooms;
    
    public override void GenerateMap(Grid grid)
    {
        Grid = grid;
    }

    public bool GenerateFromRoom(Grid room, float prob)
    {
        Grid NewRoom;
        
        foreach (RoomDoor door in room.GetComponent<Room>().Doors)
        {
            if (Random.Range(0, 100) > prob * 100) {
                Grid = GenerateFromDoor(door, room.GetComponent<Room>().DoorPos(door));
                //if (Grid) {
                //GenerateFromRoom(Grid, 3 * prob / 4);
                //InsertRoom();
                //}
            }
        }
        return true;
    }

    public Grid GenerateFromDoor(RoomDoor door, Vector2Int pos)
    {
        Grid room;
        Vector2Int? roomPos;
        
        do
        {
            room = Instantiate(AvailableRooms[Random.Range(0, AvailableRooms.Length)]);
            roomPos = CanInsertRoom(room.GetComponent<Room>(), pos);
        } while (roomPos != null);
        return room;
    }
    
    public Vector2Int? CanInsertRoom(Room room, Vector2Int pos)
    {
        foreach (RoomDoor door in room.Doors)
        {
            pos = Direction.GoAuto(pos, 1, door.Dir);
            pos = room.PosFromDoor(door, pos);
            if (room.TestPos(Grid, pos))
                return pos;
        }
        return null;
    }
}