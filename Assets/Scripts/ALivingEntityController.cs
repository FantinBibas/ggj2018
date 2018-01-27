using System.Collections;
using UnityEngine;

public abstract class ALivingEntityController : MonoBehaviour
{
    private const int SMOOTH_MOVEMENT_STEPS = 10;

    public Vector2Int StartPosition;
    [Range(1, 64)] public int MovePerTurn = 1;

    private Path _objective;

    public Vector3Int Position { get; private set; }
    public bool IsMoving { get; private set; }
    
    private void Awake()
    {
        Position = new Vector3Int(StartPosition.x, StartPosition.y, 0);
        IsMoving = false;
    }

    public void SetObjective(Vector3Int target)
    {
        _objective = GameManager.Instance.Map.NavigateTo(Position, target);
    }

    protected virtual IEnumerator OnObjectiveReached()
    {
        yield break;
    }

    private IEnumerator MoveToNext()
    {
        if (_objective == null)
            yield break;
        Vector3Int position = _objective.Next();
        Vector3 target = transform.position + (position - Position);
        Vector3 start = transform.position;
        Position = position;
        for (int step = 0; step < SMOOTH_MOVEMENT_STEPS; step += 1)
        {
            transform.position = Vector3.Lerp(start, target, (float) step / SMOOTH_MOVEMENT_STEPS);
            yield return new WaitForEndOfFrame();
        }
        transform.position = target;
        if (_objective.Length != 0) yield break;
        _objective = null;
        IsMoving = false;
        yield return OnObjectiveReached();
        IsMoving = true;
    }

    public int GetNeededTurns()
    {
        if (_objective == null)
            return 0;
        return (int) Mathf.Ceil((float) _objective.Length / MovePerTurn);
    }

    protected virtual IEnumerator PreTurn()
    {
        yield break;
    }

    protected virtual IEnumerator PostTurn()
    {
        yield break;
    }

    public IEnumerator DoTurn()
    {
        yield return PreTurn();
        IsMoving = true;
        for (int i = 0; i < MovePerTurn; i += 1)
        {
            yield return MoveToNext();
        }
        IsMoving = false;
        yield return PostTurn();
    }

    public bool HasObjective()
    {
        return _objective != null;
    }
}