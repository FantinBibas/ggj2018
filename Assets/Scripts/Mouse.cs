using UnityEngine;
using UnityEngine.Tilemaps;

public class Mouse : MonoBehaviour
{
    public Tile PathRenderTile;

    private Map _map;
    private Tilemap _renderMap;
    private PlayerController _player;
    private Vector3Int _prevPos;

    private void Awake()
    {
        _map = FindObjectOfType<Map>();
        _player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        _prevPos = _player.Position;
        GameObject tileMapGameObject = new GameObject("__renderPathTileMap") {tag = "Ignore"};
        tileMapGameObject.transform.parent = _map.transform;
        _renderMap = tileMapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tileMapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingLayerName = "Path";
    }

    private Vector3Int PosOnMap()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3Int((int) (mousePos.x - _map.X), (int) (mousePos.y - _map.Y), 0);
    }

    private bool IsValidCoord(Vector3Int pos)
    {
        return (_map.Width > pos.x && pos.x >= 0 && _map.Height > pos.y && pos.y >= 0);
    }

    private void RenderPath()
    {
        _renderMap.ClearAllTiles();
        Path path = _map.NavigateTo(_player.Position, _prevPos);
        if (path == null) return;
        while (path.Length != 0)
        {
            Vector3Int node = path.Next();
            _renderMap.SetTile(node + _map.TopLeft, PathRenderTile);
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.PlayerTurn || _player.IsMoving) return;
        Vector3Int mousePosition = PosOnMap();
        if (!IsValidCoord(mousePosition)) return;
        if (Input.GetMouseButton(1))
        {
            _player.SetObjective(mousePosition);
            _renderMap.ClearAllTiles();
        }
        else if (_prevPos != mousePosition)
        {
            _prevPos = mousePosition;
     //       RenderPath();
        }
    }
}