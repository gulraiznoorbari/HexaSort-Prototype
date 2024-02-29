using System.Collections.Generic;
using UnityEngine;

public class TilesSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject[] _tilesPrefabs;

    private List<GameObject> _availableTilesPrefabs = new List<GameObject>();

    public static TilesSpawner instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeAvailableTilesPrefabs();
        SpawnTiles();
    }

    private void InitializeAvailableTilesPrefabs()
    {
        _availableTilesPrefabs.Clear();
        _availableTilesPrefabs.AddRange(_tilesPrefabs); 
    }

    private void SpawnTiles()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (i < _availableTilesPrefabs.Count)
            {
                GameObject obj = Instantiate(_availableTilesPrefabs[i], _spawnPoints[i]);
                obj.name = _availableTilesPrefabs[i].name;
            }
        }
    }

    public void RemoveFromAvailableTilesList(GameObject gameObject)
    {
        _availableTilesPrefabs.RemoveAll(tilePrefab => tilePrefab.name == gameObject.name);

        if (_availableTilesPrefabs.Count == 0)
        {
            InitializeAvailableTilesPrefabs();
            SpawnTiles();
        }
    }
}
