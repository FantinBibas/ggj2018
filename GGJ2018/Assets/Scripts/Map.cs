using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public int X;
    public int Y;
    public uint Width;
    public uint Height;

    private Grid _grid;

    private List<Vector3Int> nodes;

    private void Start()
    {
        _grid = GetComponent<Grid>();
        if (_grid == null)
        {
            Destroy(this);
        }
        else
        {
            CreateNodes();
        }
    }

    private void CreateNodes()
    {
        foreach (Tilemap tilemap in _grid.GetComponentsInChildren<Tilemap>())
        {
            if (tilemap.CompareTag("Solid")) continue;
            for (int x = 0; x < Width; x += 1)
            {
                for (int y = 0; y < Height; y += 1)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(pos))
                    {
                        nodes.Add(pos);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(X + Width / 2, Y + Height / 2), new Vector3(Width, Height));
    }
}