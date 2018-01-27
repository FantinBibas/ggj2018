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

    public Grid Grid { get; private set; }

    private bool[,] _nodes;

    private void Awake()
    {
        Grid = GetComponent<Grid>();
        if (Grid == null)
        {
            Destroy(this);
        }
    }

    public void Init()
    {
        CreateNodes();
    }

    private bool HasNodeAt(Vector3Int pos)
    {
        if (pos.x >= _nodes.GetLength(0))
            return false;
        return pos.y < _nodes.GetLength(1) && _nodes[pos.x, pos.y];
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
            FScore = 0;
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
            return (Math.Abs(node.X - X) + Math.Abs(node.Y - Y)) == 1;
        }

        public int Heuristic(PathfindingNode node)
        {
            int rx = Math.Abs(X - node.X);
            int ry = Math.Abs(Y - node.Y);
            return rx + ry;
        }

        private Vector3Int ToVector()
        {
            return new Vector3Int(X, Y, 0);
        }

        public Path ConstructPath()
        {
            PathfindingNode node = this;
            List<Vector3Int> vecs = new List<Vector3Int> {node.ToVector()};
            while (node.From != null)
            {
                vecs.Insert(0, node.ToVector());
                node = node.From;
            }
            return new Path(node.ToVector(), ToVector(), vecs);
        }
    }

    public Path NavigateTo(Vector3Int from, Vector3Int to)
    {
        if (!HasNodeAt(from) || !HasNodeAt(to))
            return null;
        List<PathfindingNode> nodes = new List<PathfindingNode>();
        PathfindingNode current = new PathfindingNode(from);
        PathfindingNode toNode = new PathfindingNode(to);
        for (int x = 0; x < Width; x += 1)
        {
            for (int y = 0; y < Height; y += 1)
            {
                if (!_nodes[x, y]) continue;
                PathfindingNode node = new PathfindingNode(x, y);
                nodes.Add(node);
                node.FScore = node.Heuristic(toNode);
                if (node.Equals(from))
                    current = node;
                if (node.Equals(to))
                    toNode = node;
            }
        }
        current.GScore = 0;
        List<PathfindingNode> closedSet = new List<PathfindingNode>();
        List<PathfindingNode> openSet = new List<PathfindingNode> {current};
        while (openSet.Count > 0)
        {
            current = openSet.OrderBy(n => n.FScore).First();
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
        for (int x = 0; x < Width; x += 1)
        {
            for (int y = 0; y < Height; y += 1)
            {
                _nodes[x, y] = true;
            }
        }
        foreach (Tilemap tilemap in Grid.GetComponentsInChildren<Tilemap>())
        {
            bool solid = tilemap.CompareTag("Solid");
            for (int x = 0; x < Width; x += 1)
            {
                for (int y = 0; y < Height; y += 1)
                {
                    Vector3Int pos = new Vector3Int(x + X, y + Y, 0);
                    bool flag = solid;
                    if (tilemap.HasTile(pos))
                        flag = !flag;
                    if (!flag)
                        _nodes[x, y] = false;
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