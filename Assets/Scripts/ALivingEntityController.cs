using System.Collections;
using UnityEngine;

public abstract class ALivingEntityController : MonoBehaviour
{
    private const int SMOOTH_MOVEMENT_STEPS = 10;
    
    public Vector2Int StartPosition;
    [Range(1, 64)] public int MovePerTurn = 1;

    private Vector3Int _position;
    private Path _objective;

    private void Awake()
    {
        _position = new Vector3Int(StartPosition.x, StartPosition.y, 0);
    }

    public void SetObjective(Vector3Int target)
    {
        _objective = GameManager.Instance.Map.NavigateTo(_position, target);
    }

    protected virtual void OnObjectiveReached()
    {
    }

    private IEnumerator MoveToNext()
    {
        if (_objective == null)
            yield break;
        Vector3Int position = _objective.Next();
        Vector3 target = transform.position + (position - _position);
        Vector3 start = transform.position;
        _position = position;
        for (int step = 0; step < SMOOTH_MOVEMENT_STEPS; step += 1)
        {
            transform.position = Vector3.Lerp(start, target, (float) step / SMOOTH_MOVEMENT_STEPS);
            yield return new WaitForEndOfFrame();
        }
        transform.position = target;
        if (_objective.Length != 0) yield break;
        _objective = null;
        OnObjectiveReached();
    }

    public int GetNeededTurns()
    {
        if (_objective == null)
            return 0;
        return (int) Mathf.Ceil((float) _objective.Length / MovePerTurn);
    }

    public IEnumerator DoTurn()
    {
        for (int i = 0; i < MovePerTurn; i += 1)
        {
            yield return MoveToNext();
        }
    }
}