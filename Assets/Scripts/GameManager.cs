using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Map Map { get; private set; }

    public bool PlayerTurn
    {
        get { return _player.IsCurrentTurn; }
    }

    public AMapGenerator MapGenerator;

    private PlayerController _player;
    private ALivingEntityController[] _entities;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            Map = FindObjectOfType<Map>();
            if (MapGenerator != null)
                MapGenerator.GenerateMap(Map.Grid);
            Map.Init();
            _entities = FindObjectsOfType<ALivingEntityController>().ToArray();
            _player = FindObjectOfType<PlayerController>();
            StartCoroutine(MainLoop());
        }
    }

    private IEnumerator MainLoop()
    {
        while (true)
        {
            foreach (ALivingEntityController entity in _entities)
            {
                yield return entity.DoTurn();
            }
        }
    }
}