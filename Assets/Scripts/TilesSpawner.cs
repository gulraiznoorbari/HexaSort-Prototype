using System.Collections.Generic;
using UnityEngine;

public class TilesSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    public GameObject[] _availableTilesPrefab;
    public static TilesSpawner instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        SpawnTiles();
    }

    private void Update()
    {
        if (_availableTilesPrefab.Length < 1)
        {
            SpawnTiles();
        }
    }

    public void SpawnTiles()
    {
        for (int i = 0; i < _availableTilesPrefab.Length; i++)
        {
            int randomIndex = Random.Range(0, _availableTilesPrefab.Length);
            GameObject obj = Instantiate(_availableTilesPrefab[randomIndex], _spawnPoints[i]);
            obj.name = _availableTilesPrefab[randomIndex].name;
        }
    }

    private int FindIndexByName(string gameObjectName)
    {
        for (int i = 0; i < _availableTilesPrefab.Length; i++)
        {
            if (_availableTilesPrefab[i].name == gameObjectName)
            {
                return i;
            }
        }
        return -1;
    }

    private void RemoveByIndex(int index)
    {
        if (index >= 0 && index < _availableTilesPrefab.Length)
        {
            _availableTilesPrefab[index] = null;
        }
    }

    public void RemoveFromAvailableTilesList(GameObject gameObject)
    {
        int index = FindIndexByName(gameObject.name);
        if (index != -1)
        {
            RemoveByIndex(index);
        }
        else
        {
            Debug.LogWarning("GameObject not found in _availableTilesPrefab array: " + gameObject.name);
        }
    }

}
