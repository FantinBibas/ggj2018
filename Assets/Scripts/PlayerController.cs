using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Map _map;
    private Mouse _mouse;
    public Vector3Int Position = Vector3Int.zero;
    public uint MovementsPerTurn = 4;
    [CanBeNull] private Path _objectivePath;
    private uint _objectiveIdx;

    private void Awake()
    {
        _map = FindObjectOfType<Map>();
        _mouse = FindObjectOfType<Mouse>();
    }

    public Vector3Int? NextPosition()
    {
        return _objectivePath == null ? null : _objectivePath[(int) _objectiveIdx];
    }

    public Vector3Int CurrentPosition()
    {
        return Position;
    }

    private IEnumerator Move()
    {
        Vector3Int? nextPosition = NextPosition();
        if (nextPosition == null) yield break;
        Vector3 target = nextPosition.Value - Position + transform.position;
        Position = nextPosition.Value;
        _objectiveIdx++;
        Vector3 from = transform.position;
        float progress = 0;
        while (progress < 1)
        {
            transform.position = Vector3.Lerp(from, target, progress);
            progress += 0.1f;
            yield return new WaitForEndOfFrame();
        }
        transform.position = target;
        if (_objectivePath != null && _objectiveIdx != _objectivePath.Length) yield break;
        _objectiveIdx = 0;
        _objectivePath = null;
    }

    public Path GetPath(Vector3Int objective)
    {
        return _map == null ? null : _map.NavigateTo(Position, objective);
    }

    public int GetNeededTurns(Path path)
    {
        return (int) Mathf.Ceil((float) path.Length / MovementsPerTurn);
    }

    public bool SetObjective(Vector3Int objective)
    {
        _objectivePath = _map.NavigateTo(Position, objective);
        _objectiveIdx = 0;
        return _objectivePath != null;
    }
    
    public IEnumerator DoTurn()
    {
        if (_objectivePath == null)
            yield break;
        for (int i = 0; i < MovementsPerTurn; i++)
        {
            yield return Move();
        }
/*            if (!Move())
                return false;
            else if (_objectivePath == null)
                return true; */
    }
}