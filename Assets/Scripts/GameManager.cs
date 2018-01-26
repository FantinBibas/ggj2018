using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Grid Map { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            Map = FindObjectOfType<Grid>();
            PlayerController player = FindObjectOfType<PlayerController>();
            Debug.Log(player.SetObjective(new Vector3Int(5, 1, 0)));
            StartCoroutine(player.DoTurn());
        }
    }

    public bool playerTurn;
}