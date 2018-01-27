using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private Map _map;
    public Vector3Int[] RoundPosition;
    public float audioRange = 40f;
    public float viewRange = 40f;
    public float viewAngle = 180f;
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

        listenEnemy();
        viewEnemy();
        if (path == null)
            return;
        foreach (var pathPos in path)
        {
            Position = pathPos;
            transform.position = Position;
            listenEnemy();
            viewEnemy();
        }
    }

    private bool viewEnemy()
    {
        RaycastHit hit;
        Vector3 rayDirection = _player.transform.position - transform.position;
        float rayRange = Vector3.Distance(_player.transform.position, transform.position);

        if (Vector3.Angle(rayDirection, Vector3.right) <= viewAngle * 0.5f && rayRange < viewRange && rayRange <= viewRange)
        {
            Debug.Log("View Player " + Vector3.Angle(rayDirection, Vector3.right));
            if (Physics.Raycast(transform.position, rayDirection, out hit, viewRange))
            {
                Debug.Log("Hit something");
                return hit.transform.CompareTag("player");
            }
        }

        return false;
    }

    private bool listenEnemy()
    {
        float rayRange = Vector3.Distance(_player.transform.position, transform.position);

        if (rayRange <= audioRange)
        {
            Debug.Log("Listen Player");
            return true;
        }

        return false;
    }

    private void Update()
    {
        listenEnemy();
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