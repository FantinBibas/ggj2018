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

    private Vector2Int NextWaypoint
    {
        get
        {
            _currentWaypointIndex = (_currentWaypointIndex += 1) % Waypoints.Length;
            return Waypoints[_currentWaypointIndex];
        }
    }

    private void Start()
    {
        _currentWaypointIndex = 0;
        if (LoopMode != ELoopMode.REVERSE || Waypoints.Length <= 2) return;
        Vector2Int[] waypoints = new Vector2Int[Waypoints.Length * 2 - 2];
        for (int i = 0; i < Waypoints.Length; i += 1)
            waypoints[i] = Waypoints[i];
        for (int i = Waypoints.Length - 2; i > 0; i -= 1)
            waypoints[i + Waypoints.Length - 1] = Waypoints[i];            
        Waypoints = waypoints;
    }

    private void OnDrawGizmos()
    {
        Map map = FindObjectOfType<Map>();
        Vector3 prev = new Vector3(-1, -1);
        for (int i = 0; i < Waypoints.Length; i += 1)
        {
            Gizmos.color = i == _currentWaypointIndex ? Color.green : Color.red;
            Vector2Int waypoint = Waypoints[i];
            Vector3 pos = new Vector3(waypoint.x + map.X + 0.5f, waypoint.y + map.Y + 0.5f);
            Gizmos.DrawWireSphere(pos, 0.5f);
            if (prev.x >= 0)
                Gizmos.DrawLine(pos, prev);
            prev = pos;
        }
    }

    protected override IEnumerator OnObjectiveReached()
    {
        Vector2Int vec2 = NextWaypoint;
        SetObjective(new Vector3Int(vec2.x, vec2.y, 0));
        yield return base.OnObjectiveReached();
    }
}
