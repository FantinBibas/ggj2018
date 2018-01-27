using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Map Map { get; private set; }

    public bool PlayerTurn
    {
        get { return Player.IsCurrentTurn; }
    }

    public AMapGenerator MapGenerator;

    public PlayerController Player { get; private set; }
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
            Player = FindObjectOfType<PlayerController>();
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

    private void Update()
    {
        foreach (ALivingEntityController entity in _entities)
        {
            if (entity.CompareTag("Guard"))
            {
                RaycastHit hit;
                GuardController guard = entity.GetComponent<GuardController>();
                Vector3 rayDirection = Player.transform.position - entity.transform.position;
                float rayRange = Vector3.Distance(Player.transform.position, entity.transform.position);

                if (Vector3.Angle(rayDirection, entity.Direction) <=
                    guard.ViewAngle * 0.5f &&
                    rayRange < guard.ViewRange &&
                    rayRange <= guard.ViewRange)
                {
                    Debug.Log("View Player " + Vector3.Angle(rayDirection, entity.Direction));
                    if (Physics.Raycast(entity.transform.position, rayDirection, out hit, guard.ViewRange))
                    {
                        Debug.Log("Hit something");
                        //return hit.transform.CompareTag("Player");
                    }
                }
            }
        }
    }
}