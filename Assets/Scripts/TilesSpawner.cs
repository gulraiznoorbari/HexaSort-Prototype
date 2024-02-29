using System.Collections.Generic;
using UnityEngine;

public class TilesSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject[] _tilesPrefabs;

    private GameObject[] _availableTilesPrefabs;

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

    private void SpawnTiles()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            //int randomIndex = Random.Range(0, _tilesPrefabs.Length);
            GameObject obj = Instantiate(_tilesPrefabs[i], _spawnPoints[i]);
            obj.name = _tilesPrefabs[i].name;
        }
        _availableTilesPrefabs = _tilesPrefabs;
    }

    private int FindIndexByName(string gameObjectName)
    {
        int index = -1;
        for (int i = 0; i < _availableTilesPrefabs.Length; i++)
        {
            if (_availableTilesPrefabs[i].name == gameObjectName)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private void RemoveByIndex(int index)
    {
        if (index >= 0 && index < _availableTilesPrefabs.Length)
        {
            List<GameObject> tempList = new List<GameObject>(_availableTilesPrefabs);
            tempList.RemoveAt(index);
            _availableTilesPrefabs = tempList.ToArray();
        }
    }

    public void RemoveFromAvailableTilesList(GameObject gameObject)
    {
        int index = FindIndexByName(gameObject.name);
        if (index != -1)
        {
            RemoveByIndex(index);
            if (_availableTilesPrefabs.Length == 0)
            {
                SpawnTiles();
            }
        }
        else
        {
            Debug.LogWarning("GameObject not found: " + gameObject.name);
        }
    }

}