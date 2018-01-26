using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Map Map;
    public Vector3Int Position = Vector3Int.zero;
    public uint MovementsPerTurn = 4;
    [CanBeNull] private Path _objectivePath;
    private uint _objectiveIdx;

    public Vector3Int? NextPosition()
    {
        return _objectivePath == null ? null : _objectivePath[(int) _objectiveIdx];
    }

    public Vector3Int CurrentPosition()
    {
        return Position;
    }

    private bool Move()
    {
        var nextPosition = NextPosition();
        if (nextPosition == null) return false;
        this.Position = nextPosition.Value;
        _objectiveIdx++;
        if (_objectivePath != null && _objectiveIdx != _objectivePath.Length) return true;
        _objectiveIdx = 0;
        _objectivePath = null;
        return true;

    }

    public Path GetPath(Vector3Int objective)
    {
        return Map == null ? null : Map.NavigateTo(Position, objective);
    }

    public int GetNeededTurns(Path path)
    {
        return (int) Mathf.Ceil((float) path.Length / MovementsPerTurn);
    }

    public bool SetObjective(Vector3Int objective)
    {
        _objectivePath = Map.NavigateTo(Position, objective);
        _objectiveIdx = 0;
        return _objectivePath != null;
    }

    public bool DoTurn()
    {
        if (_objectivePath == null)
            return false;
        for (var i = 0; i < MovementsPerTurn; i++)
            if (!Move())
                return false;
            else if (_objectivePath == null)
                return true;
        return true;
    }
}
