using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour {   
    public int Pos;

    public bool isValid()
    {
        return Pos > 0;
    }
}