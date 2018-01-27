using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : ALivingEntityController
{
    public enum ELoopMode
    {
        REVERSE,
        GOTO_FIRST
    }

    public ELoopMode LoopMode;

    public Vector2Int[] Waypoints;
    private int _currentWaypointIndex;
    public float AudioRange = 2f;
    public float ViewRange = 10f;
    public float ViewAngle = 120f;

    private Vector2Int NextWaypoint
    {
        get
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % Waypoints.Length;
            return Waypoints[_currentWaypointIndex];
        }
    }

    protected override void Init()
    {
        _currentWaypointIndex = 0;
        if (LoopMode != ELoopMode.REVERSE || Waypoints.Length <= 2) return;
        Vector2Int[] waypoints = new Vector2Int[Waypoints.Length * 2 - 2];
        for (int i = 0; i < Waypoints.Length; i += 1)
            waypoints[i] = Waypoints[i];
        for (int i = 1; i < Waypoints.Length - 1; i += 1)
            waypoints[i + Waypoints.Length - 1] = Waypoints[Waypoints.Length - i - 1];
        Waypoints = waypoints;
    }

    public void CheckForPlayer()
    {
        PlayerController player = GameManager.Instance.Player;
        Vector3Int rel = player.Position - Position;
        if (Mathf.Abs(Vector3.Dot(rel, Direction) - rel.magnitude) > float.Epsilon)
            return;
        Vector3Int pos = Position;
        Map map = GameManager.Instance.Map;
        Vector3Int dir = new Vector3Int((int) Direction.x, (int) Direction.y, 0);
        while (!pos.Equals(player.Position))
        {
            Debug.Log(pos);
            if (map.IsSolid(pos))
                return;
            pos += dir;
        }

        GameObject.FindGameObjectWithTag("GlobalLoseMsg").GetComponent<Canvas>().enabled = true;
        Camera.main.GetComponent<MapCamera>().StopFollowing();
        GameManager.Instance.StopGame();
    }

    protected override IEnumerator OnMove()
    {
        CheckForPlayer();
        yield return base.OnMove();
    }

    protected override IEnumerator PreTurn()
    {
        CheckForPlayer();
        yield return base.PreTurn();
    }

    protected override IEnumerator PostTurn()
    {
        CheckForPlayer();
        yield return base.PostTurn();
    }

    protected override IEnumerator OnObjectiveReached()
    {
        Vector2Int vec2 = NextWaypoint;
        SetObjective(new Vector3Int(vec2.x, vec2.y, 0));
        yield return base.OnObjectiveReached();
    }
}