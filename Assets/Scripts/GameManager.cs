using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Map Map { get; private set; }

    public AMapGenerator MapGenerator;

    private PlayerController _player;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            PlayerTurn = true;
            Instance = this;
            Map = FindObjectOfType<Map>();
            if (MapGenerator != null)
                MapGenerator.GenerateMap(Map.Grid);
            Map.Init();
            _player = FindObjectOfType<PlayerController>();
            StartCoroutine(MainLoop());
        }
    }

    private IEnumerator MainLoop()
    {
        while (true)
        {
            if (PlayerTurn)
                yield return _player.DoTurn();
            else
                PlayerTurn = true;
        }
    }

    public bool PlayerTurn { get; private set; }
}