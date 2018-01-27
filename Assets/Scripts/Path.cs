using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : IEnumerable<Vector3Int>
{
    public Vector3Int Start { get; private set; }
    public Vector3Int End { get; private set; }

    private readonly List<Vector3Int> _nodes;

    private Vector3? _direction;
    private Vector3 _prevPos;

    public int Length
    {
        get { return _nodes.Count; }
    }

    public Path(Vector3Int start, Vector3Int end, params Vector3Int[] nodes)
        : this(start, end, (IEnumerable<Vector3Int>) nodes)
    {
    }

    public Path(Vector3Int start, Vector3Int end, IEnumerable<Vector3Int> nodes)
    {
        Start = start;
        End = end;
        _nodes = new List<Vector3Int>();
        _nodes.AddRange(nodes);
        _direction = null;
        _prevPos = start;
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
        foreach (Vector3Int node in _nodes)
        {
            yield return node;
            UpdateDirection(node);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Vector3Int Next()
    {
        Vector3Int ret = _nodes[0];
        _nodes.RemoveAt(0);
        UpdateDirection(ret);
        return ret;
    }

    private void UpdateDirection(Vector3Int current)
    {
        _direction = current - _prevPos;
        _prevPos = current;
    }

    public Vector3 Direction
    {
        get { return _direction ?? Vector3.up; }
    }
}