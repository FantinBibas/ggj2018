using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public int X;
    public int Y;
    public uint Width;
    public uint Height;

    private Grid _grid;

    private List<Vector3Int> _nodes;

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

    private int GetNodeAt(Vector3Int pos)
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (pos.Equals(_nodes[i]))
                return i;
        }
        return -1;
    }

    public Path NavigateTo(Vector3Int from, Vector3Int to)
    {
        int fromIdx = GetNodeAt(from);
        int toIdx = GetNodeAt(to);
        if (fromIdx == -1 || toIdx == -1)
            return null;
        List<int> closedSet = new List<int>();
        List<int> openSet = new List<int>(fromIdx);
        Dictionary<int, int> cameFrom = new Dictionary<int, int>();
        Dictionary<int, int> gScore = _nodes.Select((n, i) => i).ToDictionary(i => i, i => -1);
        return new Path(from, to);
    }

    private void CreateNodes()
    {
        _nodes = new List<Vector3Int>();
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
                        _nodes.Add(pos);
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