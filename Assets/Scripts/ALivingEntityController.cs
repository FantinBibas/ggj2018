﻿using System.Collections;
using UnityEngine;

public abstract class ALivingEntityController : MonoBehaviour, ITurnBasedEntity
{
    private const int SMOOTH_MOVEMENT_STEPS = 10;

    private Animator _animator;
    private AudioSource _audio;

//    public Vector2Int StartPosition;
    [Range(1, 64)] public int MovePerTurn = 1;

    private Path _objective;

    public Vector3Int Position { get; private set; }
    public Vector3 Direction { get; private set; }
    public bool IsMoving { get; private set; }

    public bool IsInit { get; private set; }

    public bool IsCurrentTurn { get; private set; }

    public int RemainingMoves { get; private set; }

    private void Awake()
    {
        Direction = new Vector3(1, 0, 0);
        IsMoving = false;
        IsInit = false;
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        if (_audio)
            _audio.loop = true;
    }

    public void OnCreate()
    {
        Map map = GameManager.Instance.Map;
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(transform.position.x),
            Mathf.FloorToInt(transform.position.y), 0);
        pos -= new Vector3Int(map.X, map.Y, 0);
        if (pos.x < 0 || pos.x >= map.Width || pos.y < 0 || pos.y >= map.Height)
            Destroy(this);
        else
            Position = new Vector3Int(pos.x, pos.y, 0);
        Init();
    }

    protected virtual void Init()
    {
    }

    public void SetObjective(Vector3Int target)
    {
        _objective = GameManager.Instance.Map.NavigateTo(Position, target);
        if (_objective == null || _objective.Length == 0)
            _objective = null;
    }

    protected virtual IEnumerator OnObjectiveReached()
    {
        yield break;
    }

    private IEnumerator MoveToNext()
    {
        if (_objective == null || _objective.Length == 0)
            yield break;
        Vector3Int position = _objective.Next();
        Vector3 direction = _objective.Direction;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg);
        Vector3 target = transform.position + (position - Position);
        Vector3 start = transform.position;
        Position = position;
        if (_animator)
            _animator.SetBool("moving", true);
        if (_audio)
            _audio.Play();
        for (int step = 0; step < SMOOTH_MOVEMENT_STEPS; step += 1)
        {
            Direction = (target - transform.position).normalized;
            transform.position = Vector3.Lerp(start, target, (float) step / SMOOTH_MOVEMENT_STEPS);
            yield return new WaitForEndOfFrame();
        }
        if (_audio)
            _audio.Stop();
        transform.position = target;
        yield return OnMove();
        if (_animator)
            _animator.SetBool("moving", false);
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

    protected virtual IEnumerator OnMove()
    {
        yield break;
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
        IsCurrentTurn = true;
        RemainingMoves = MovePerTurn;
        if (!IsInit)
        {
            yield return OnObjectiveReached();
            IsInit = true;
        }
        yield return PreTurn();
        IsMoving = true;
        while (RemainingMoves > 0)
        {
            RemainingMoves -= 1;
            yield return MoveToNext();
        }
        IsMoving = false;
        yield return PostTurn();
        IsCurrentTurn = false;
    }

    public bool HasObjective()
    {
        return _objective != null;
    }

    public void CancelObjective()
    {
        _objective = null;
    }
}