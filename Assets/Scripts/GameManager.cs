using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public Canvas GameOverPrefab;

    public PlayerController Player { get; private set; }

    public IEnumerable<GuardController> Guards
    {
        get { return _entities.OfType<GuardController>(); }
    }

    private ALivingEntityController[] _entities;
    private bool _end;

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
        }
    }

    private void Start()
    {
        if (MapGenerator != null)
            MapGenerator.GenerateMap(Map.Grid);
        Map.Init();
        _entities = FindObjectsOfType<ALivingEntityController>().ToArray();
        Player = FindObjectOfType<PlayerController>();
        foreach (ALivingEntityController e in _entities)
            e.OnCreate();
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        yield return new WaitForEndOfFrame();
        while (!_end)
        {
            Coroutine[] coroutines = _entities.Select(entity => StartCoroutine(entity.DoTurn())).ToArray();
            while (!_end && _entities.Any(e => e.IsCurrentTurn))
                yield return new WaitForEndOfFrame();
        }
    }

    public void StopGame()
    {
        _end = true;
        StopAllCoroutines();
    }

    public IEnumerator ShowMinigame()
    {
        HackGameManager minigame = Instantiate(MinigamePrefab);
        minigame.name = "__minigame";
        Map.gameObject.gameObject.SetActive(false);
        MapCamera cam = Camera.main.GetComponent<MapCamera>();
        float camZ = Camera.main.transform.position.z;
        cam.StopFollowing();
        Camera.main.transform.position = new Vector3(0, 4, camZ);
        while (!minigame.IsOver)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(3);
        Destroy(minigame.gameObject);
        cam.StartFollowing();
        Map.gameObject.gameObject.SetActive(true);
    }


    private static IEnumerator Restart()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GameOver()
    {
        Camera.main.GetComponent<MapCamera>().StopFollowing();
        StopGame();
        Map.gameObject.SetActive(false);
        Instantiate(GameOverPrefab);
        StartCoroutine(Restart());
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