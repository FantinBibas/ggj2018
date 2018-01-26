using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class NaziManager : MonoBehaviour
{
    public Map Map;
    public Vector3Int[] RoundPosition;
    public Vector3Int Position = Vector3Int.zero;
    private uint _pointPos;

    private enum Direction
    {
        StartToEnd,
        EndToStart
    }

    private Direction _direction;

    private void Start()
    {
        _direction = Direction.StartToEnd;
    }

    public void Play()
    {
        Vector3Int startPos = RoundPosition[_pointPos];
        Vector3Int endPos = GetNextPoint();
        Path path = Map.NavigateTo(startPos, endPos);

        if (path == null)
            return;
        foreach (var pathPos in path)
        {
            Position = pathPos;
            checkEnemy();
        }
    }

    private bool checkEnemy()
    {
        return true;
    }
    
    private Vector3Int GetNextPoint()
    {
        Vector3Int curPointPos = RoundPosition[_pointPos];

        IncrementNextPointPos();
        return curPointPos;
    }

    private void IncrementNextPointPos()
    {
        if (_direction == Direction.StartToEnd)
            _pointPos++;
        else
            _pointPos--;
        if (_pointPos == RoundPosition.Length)
        {
            _direction = Direction.EndToStart;
            _pointPos--;
            IncrementNextPointPos();
        }
        else if (_pointPos == 0)
        {
            _direction = Direction.StartToEnd;
        }
    }
}