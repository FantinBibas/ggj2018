using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Map Map { get; private set; }

    public AMapGenerator MapGenerator;

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
            PlayerController player = FindObjectOfType<PlayerController>();
            Debug.Log(player.SetObjective(new Vector3Int(5, 1, 0)));
            StartCoroutine(player.DoTurn());
        }
    }

    public bool playerTurn;
}