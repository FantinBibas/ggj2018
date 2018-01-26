using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePos : MonoBehaviour {
    Vector2Int posOnMap()
    {
        Vector3 mousePos = Input.mousePosition;
        return new Vector2Int((int) (mousePos.x - map.X), (int) (mousePos.y - map.Y));
    }
	
    public Map map;
}