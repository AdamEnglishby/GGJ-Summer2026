using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelSpawner : MonoBehaviour
{
    
    [SerializeField] private Transform player;
    [SerializeField] private LevelSegment emptyPrefab;
    [SerializeField] private LevelSegment[] segmentPrefabs;
    [SerializeField] private int initialSegmentCount = 4;
    [SerializeField] private float spawnDistanceAhead = 60f;
    [SerializeField] private float despawnDistanceBehind = 20f;
    [SerializeField] private float difficultyIncreaseInterval = 5f;
    [SerializeField] private float difficultyIncreaseAmount = 1.1f;
    
    public bool NeedsReset;
    [NonSerialized] public float DifficultyModifier = 1f;
    private float _difficultyTimer;

    private readonly Queue<LevelSegment> _activeSegments = new();
    private Vector3 _nextSpawnPosition;
    private int _lastSegmentIndex = -1;

    private void Start()
    {
        _nextSpawnPosition = transform.position;

        SpawnSegment(emptyPrefab);
        for (var i = 0; i < initialSegmentCount - 1; i++) SpawnSegment();
    }

    private void Update()
    {
        _difficultyTimer += Time.deltaTime;
        if (_difficultyTimer >= difficultyIncreaseInterval)
        {
            DifficultyModifier *= difficultyIncreaseAmount;
            _difficultyTimer = 0f;
        }

        var furthestSegmentEnd = _nextSpawnPosition.z;
        while (furthestSegmentEnd - player.position.z < spawnDistanceAhead)
        {
            SpawnSegment();
            furthestSegmentEnd = _nextSpawnPosition.z;
        }

        while (_activeSegments.Count > 0)
        {
            var oldest = _activeSegments.Peek();
            if (player.position.z - oldest.transform.position.z < despawnDistanceBehind) break;

            NeedsReset = true;
            _activeSegments.Dequeue();
            
            Destroy(oldest.gameObject);
        }
    }

    private void SpawnSegment(LevelSegment prefabOverride = null)
    {
        LevelSegment prefab;

        if (prefabOverride)
        {
            prefab = prefabOverride;
        }
        else
        {
            int newSegmentIndex;
            do
            {
                newSegmentIndex = Random.Range(0, segmentPrefabs.Length);
            } while (newSegmentIndex == _lastSegmentIndex && segmentPrefabs.Length > 1);

            prefab = segmentPrefabs[newSegmentIndex];
            _lastSegmentIndex = newSegmentIndex;
        }

        var segment = Instantiate(prefab, _nextSpawnPosition, Quaternion.identity, transform);
        _activeSegments.Enqueue(segment);
        _nextSpawnPosition = segment.NextSegmentPosition;
    }
    
    public void ResetLevel()
    {
        while (_activeSegments.Count > 0)
            Destroy(_activeSegments.Dequeue().gameObject);

        _nextSpawnPosition = transform.position;
        _lastSegmentIndex = -1;
        DifficultyModifier = 1f;
        _difficultyTimer = 0f;

        SpawnSegment(emptyPrefab);
        for (var i = 0; i < initialSegmentCount - 1; i++) SpawnSegment();

        NeedsReset = false;
    }

}
