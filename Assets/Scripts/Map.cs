using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class Map : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Tile StationTile;
    public List<Vector3Int> Stations;


    public Grid Grid { get; private set; }

    public Vector3Int TopLeft
    {
        get { return new Vector3Int(X, Y, 0); }
    }

    private bool[,] _nodes;

    private void CalculateBounds()
    {
        int minx = 0;
        int miny = 0;
        int maxx = 0;
        int maxy = 0;
        foreach (Tilemap t in Grid.GetComponentsInChildren<Tilemap>())
        {
            BoundsInt b = t.cellBounds;
            if (b.xMin < minx)
                minx = b.xMin;
            if (b.yMin < miny)
                miny = b.yMin;
            if (b.xMax > maxx)
                maxx = b.xMax;
            if (b.yMax > maxy)
                maxy = b.yMax;
        }
        X = minx;
        Y = miny;
        Width = maxx - minx;
        Height = maxy - miny;
    }

    private void Awake()
    {
        Grid = GetComponent<Grid>();
        CalculateBounds();
    }

    public void Init()
    {
        GameObject tilemapGameObject = new GameObject("__buttonTileMap");
        tilemapGameObject.transform.parent = transform;
        Tilemap tilemap = tilemapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilerenderer = tilemapGameObject.AddComponent<TilemapRenderer>();
        tilerenderer.sortingLayerName = "Objects";
        tilemapGameObject.tag = "Ignore";
        foreach (Vector3Int station in Stations)
            tilemap.SetTile(station + TopLeft, StationTile);
        CalculateBounds();
        CreateNodes();
    }

    public bool IsSolid(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= Width || pos.y < 0 || pos.y >= Height)
            return true;
        return !_nodes[pos.x, pos.y];
    }

    private bool HasNodeAt(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= _nodes.GetLength(0))
            return false;
        return pos.y >= 0 && pos.y < _nodes.GetLength(1) && _nodes[pos.x, pos.y];
    }

    public void RemoveStationAt(Vector3Int pos)
    {
        Stations = Stations.Where(s => !s.Equals(pos + TopLeft)).ToList();
    }

    public bool IsStation(Vector3Int pos)
    {
        return Stations.Any(s => s.Equals(pos + TopLeft));
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
            List<Vector3Int> vecs = new List<Vector3Int>();
            while (node.From != null)
            {
                vecs.Insert(0, node.ToVector());
                node = node.From;
            }
            return new Path(node.ToVector(), ToVector(), vecs);
        }

        public Vector3Int To(PathfindingNode node)
        {
            return new Vector3Int(node.X - X, node.Y - Y, 0);
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
                if (current.From != null)
                    node.FScore += current.To(current.From).Equals(node.To(current)) ? 0 : 4;
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
        Tilemap[] tilemaps = Grid.GetComponentsInChildren<Tilemap>().Where(t => !t.CompareTag("Ignore")).ToArray();
        for (int x = 0; x < Width; x += 1)
        {
            for (int y = 0; y < Height; y += 1)
            {
                bool flag = true;
                foreach (Tilemap tilemap in tilemaps)
                {
                    bool solid = tilemap.CompareTag("Solid");
                    Vector3Int pos = new Vector3Int(x + X, y + Y, 0);
                    if (!tilemap.HasTile(pos)) continue;
                    if (solid)
                    {
                        flag = true;
                        break;
                    }
                    flag = false;
                }
                if (flag)
                    _nodes[x, y] = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(X + Width / 2, Y + Height / 2), new Vector3(Width, Height));
        /* Gizmos.color = Color.yellow;
         foreach (Vector3Int station in Stations)
             Gizmos.DrawWireSphere(station + new Vector3(0.5f, 0.5f, 0), 0.25f); */
    }
}