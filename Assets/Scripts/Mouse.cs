using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour {
    private Map _map;
    private PlayerController _player;
   
    private void Awake()
    {
        _map = FindObjectOfType<Map>();
        _player = FindObjectOfType<PlayerController>();
    }

    Vector3Int posOnMap()
    {
        Vector3 mousePos = Input.mousePosition;
        return new Vector3Int((int) (mousePos.x - _map.X), (int) (mousePos.y - _map.Y), (int) mousePos.z);
    }

    bool isValidCoord(Vector3Int pos)
    {
        return (_map.X > pos.x && pos.x >= 0 && _map.Y > pos.y && pos.y >= 0);
    }
    
    public IEnumerator DoTurn()
    {
        Vector3Int obj;
        do {
            obj = posOnMap();
            yield return new WaitForEndOfFrame();
            /* Render le chemin Lol */
        } while (!Input.GetMouseButtonDown(1) || !isValidCoord(obj));
        _player.SetObjective(obj);
        yield return _player.DoTurn();
    }
}