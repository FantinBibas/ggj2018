using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private Map _map;
    public Vector3Int[] RoundPosition;
    public uint audioRange = 40;
    public uint viewRange = 40;
    public float viewAngle = 180;
    public Vector3Int Position = Vector3Int.zero;
    private uint _pointPos;
    private PlayerController _player;

    private enum Direction
    {
        StartToEnd,
        EndToStart
    }

    private Direction _direction;

    private void Awake()
    {
        _direction = Direction.StartToEnd;
        _map = FindObjectOfType<Map>();
        _player = (PlayerController) FindObjectOfType(typeof(PlayerController));
    }

    public void Play()
    {
        Vector3Int startPos = RoundPosition[_pointPos];
        Vector3Int endPos = GetNextPoint();
        Path path = _map.NavigateTo(startPos, endPos);

        viewEnemy();
        if (path == null)
            return;
        foreach (var pathPos in path)
        {
            Position = pathPos;
            transform.position = Position;
            viewEnemy();
        }
        viewEnemy();
    }

    private bool viewEnemy()
    {
        RaycastHit hit;
        Vector3 rayDirection = _player.transform.position - transform.position;

        if (Vector3.Angle(rayDirection, Vector3.right) <= viewAngle * 0.5f )
        {
            Debug.Log("IsIn " + Vector3.Angle(rayDirection, Vector3.right));
            if (Physics.Raycast(transform.position, rayDirection, out hit, viewRange))
            {
                Debug.Log("Player");
                return hit.transform.CompareTag("player");
            }
        }

        return false;
    }

    private void Update()
    {
        viewEnemy();
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