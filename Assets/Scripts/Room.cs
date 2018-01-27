using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int Size;

    public RoomDoor NorthDoor;
    public RoomDoor SouthDoor;
    public RoomDoor EstDoor;
    public RoomDoor WestDoor;

    public static bool CheckDoor(RoomDoor Door)
    {
        return Door && Door.isValid();
    }
}