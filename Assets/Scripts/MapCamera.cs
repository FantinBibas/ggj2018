using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MapCamera : MonoBehaviour
{
    private Camera _camera;
    private Coroutine _moveCoroutine;
    private Vector3Int _currentPos;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private IEnumerator MoveTo(Vector3 pos)
    {
        Vector3 start = transform.position;
        for (int i = 0; i < 20; i += 1)
        {
            transform.position = Vector3.Lerp(start, pos, (float) i / 20);
            yield return new WaitForEndOfFrame();
        }
        transform.position = pos;
    }

    private void CenterOn(Vector3Int pos)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        Map map = GameManager.Instance.Map;
        float x = map.X + pos.x;
        float hw = _camera.orthographicSize * _camera.aspect;
        float centerx = map.X + map.Width / 2;
        float minx = Mathf.Min(map.X + hw - 1, centerx - 1);
        float maxx = Mathf.Max(map.X + map.Width - hw + 1, centerx + 1);
        x = Math.Min(maxx, Math.Max(minx, x));
        float y = map.Y + pos.y;
        float hh = _camera.orthographicSize;
        float centery = map.Y + map.Height / 2;
        float miny = Mathf.Min(map.Y + hh - 1, centery - 1);
        float maxy = Mathf.Max(map.Y + map.Height - hh + 1, centery + 1);
        y = Math.Min(maxy, Math.Max(miny, y));
        Vector3 target = new Vector3(x, y, _camera.transform.position.z);
        _moveCoroutine = StartCoroutine(MoveTo(target));
    }

    private void Update()
    {
        PlayerController player = GameManager.Instance.Player;
        if (Vector3Int.Distance(_currentPos, player.Position) < 1) return;
        CenterOn(player.Position);
        _currentPos = player.Position;
    }
}