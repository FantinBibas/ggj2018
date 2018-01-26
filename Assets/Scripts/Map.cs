using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public int X;
    public int Y;
    public int Width;
    public int Height;

    private Grid _grid;

    private bool[,] _nodes;

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

    private bool HasNodeAt(Vector3Int pos)
    {
        if (pos.x <= _nodes.GetLength(0))
            return false;
        return pos.y <= _nodes.GetLength(1) && _nodes[pos.x, pos.y];
    }

    private class PathfindingNode
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int FScore { get; set; }
        public int GScore { get; set; }
        public PathfindingNode From { get; set; }

        public PathfindingNode(int x, int y)
        {
            X = x;
            Y = y;
            FScore = int.MaxValue;
            GScore = int.MaxValue;
            From = null;
        }

        public PathfindingNode(Vector3Int pos) : this(pos.x, pos.y)
        {
        }

        public bool Equals(Vector3Int pos)
        {
            return pos.x == X && pos.y == Y;
        }

        public bool IsNeighboor(PathfindingNode node)
        {
            return Math.Abs(node.X - X + node.Y - Y) == 1;
        }

        public int Heuristic(PathfindingNode node)
        {
            int rx = X - node.X;
            int ry = Y - node.Y;
            return rx * rx + ry * ry;
        }

        public Vector3Int ToVector()
        {
            return new Vector3Int(X, Y, 0);
        }

        public Path ConstructPath()
        {
            PathfindingNode node = this;
            List<Vector3Int> vecs = new List<Vector3Int>();
            while (node.From != null)
                vecs.Add(node.ToVector());
            return new Path(node.ToVector(), ToVector(), vecs);
        }
    }

    public Path NavigateTo(Vector3Int from, Vector3Int to)
    {
        if (!HasNodeAt(from) || !HasNodeAt(to))
            return null;
        List<PathfindingNode> nodes = new List<PathfindingNode>();
        for (int x = 0; x < Width; x += 1)
        {
            for (int y = 0; y < Height; y += 1)
            {
                if (_nodes[x, y])
                    nodes.Add(new PathfindingNode(x, y));
            }
        }
        PathfindingNode current = new PathfindingNode(from);
        nodes.Add(current);
        PathfindingNode toNode = new PathfindingNode(to);
        nodes.Add(toNode);
        List<PathfindingNode> closedSet = new List<PathfindingNode>();
        List<PathfindingNode> openSet = new List<PathfindingNode> {current};
        while (openSet.Count > 0)
        {
            current = nodes.OrderByDescending(n => n.FScore).First();
            if (current.Equals(to))
                return current.ConstructPath();
            openSet.Remove(current);
            closedSet.Add(current);
            foreach (PathfindingNode node in nodes.Where(n => n.IsNeighboor(current)))
            {
                if (closedSet.Contains(node))
                    continue;
                if (!openSet.Contains(node))
                    openSet.Add(node);
                int tentativeGScore = current.GScore + 1;
                if (tentativeGScore >= node.GScore)
                    continue;
                node.GScore = tentativeGScore;
                node.FScore = node.GScore + node.Heuristic(toNode);
                node.From = current;
            }
        }
        return null;
    }

    private void CreateNodes()
    {
        _nodes = new bool[Width, Height];
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
                        _nodes[x, y] = true;
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