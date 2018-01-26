using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : IEnumerable<Vector3Int>
{
    public Vector3Int Start { get; private set; }
    public Vector3Int End { get; private set; }

    private readonly List<Vector3Int> _nodes;

    public int Length
    {
        get { return _nodes.Count; }
    }

    public Path(Vector3Int start, Vector3Int end, params Vector3Int[] nodes)
    {
        Start = start;
        End = end;
        _nodes = new List<Vector3Int>();
        _nodes.AddRange(nodes);
    }

    public Vector3Int? this[int index]
    {
        get
        {
            if (_nodes.Count <= index)
                return null;
            return _nodes[index];
        }
    }

    public IEnumerator<Vector3Int> GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}