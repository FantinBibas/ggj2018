using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Map Map { get; private set; }

    public HackGameManager MinigamePrefab;

    public bool PlayerTurn
    {
        get { return Player.IsCurrentTurn; }
    }

    public AMapGenerator MapGenerator;

    public PlayerController Player { get; private set; }
    private ALivingEntityController[] _entities;
    private bool _end = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            Map = FindObjectOfType<Map>();
            _entities = FindObjectsOfType<ALivingEntityController>().ToArray();
            Player = FindObjectOfType<PlayerController>();
        }
    }

    private void Start()
    {
        if (MapGenerator != null)
            MapGenerator.GenerateMap(Map.Grid);
        Map.Init();
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (!_end)
        {
            foreach (ALivingEntityController entity in _entities)
            {
                if (_end) break;
                yield return entity.DoTurn();
            }
        }
    }

    public void StopGame()
    {
        _end = true;
    }

    public IEnumerator ShowMinigame()
    {
        HackGameManager minigame = Instantiate(MinigamePrefab);
        minigame.name = "__minigame";
        Map.gameObject.gameObject.SetActive(false);
        while (!minigame.IsOver)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(3);
        Destroy(minigame.gameObject);
        Map.gameObject.gameObject.SetActive(true);
    }

    
    
/*    private void Update()
    {
        foreach (ALivingEntityController entity in _entities)
        {
            if (!entity.CompareTag("Guard")) continue;
            GuardController guard = entity.GetComponent<GuardController>();
            Vector3 rayDirection = Player.transform.position - entity.transform.position;
            float rayRange = Vector3.Distance(Player.transform.position, entity.transform.position);

            if (!(Vector3.Angle(rayDirection, entity.Direction) <=
                  guard.ViewAngle * 0.5f) || !(rayRange < guard.ViewRange) ||
                !(rayRange <= guard.ViewRange)) continue;
            RaycastHit2D hit = Physics2D.Raycast(entity.transform.position, rayDirection, guard.ViewRange);
            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("Hit Player");
            }
        }
    } */
}