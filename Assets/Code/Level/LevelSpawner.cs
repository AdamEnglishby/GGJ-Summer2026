using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelSpawner : MonoBehaviour
{
    
    [SerializeField] private Transform player;
    [SerializeField] private LevelSegment[] segmentPrefabs;
    [SerializeField] private int initialSegmentCount = 4;
    [SerializeField] private float spawnDistanceAhead = 60f;
    [SerializeField] private float despawnDistanceBehind = 20f;
    [SerializeField] private float difficultyIncreaseInterval = 5f;
    [SerializeField] private float difficultyIncreaseAmount = 1.1f;
    
    [NonSerialized] public float DifficultyModifier = 1f;
    private float _difficultyTimer;

    private readonly Queue<LevelSegment> _activeSegments = new();
    private Vector3 _nextSpawnPosition;
    private int _lastSegmentIndex = -1;

    private void Start()
    {
        _nextSpawnPosition = transform.position;

        for (var i = 0; i < initialSegmentCount; i++) SpawnSegment();
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

            _activeSegments.Dequeue();
            
            Destroy(oldest.gameObject);
        }
    }

    private void SpawnSegment()
    {
        int newSegmentIndex;
        do
        {
            newSegmentIndex = Random.Range(0, segmentPrefabs.Length);
        } while (newSegmentIndex == _lastSegmentIndex && segmentPrefabs.Length > 1);

        var prefab = segmentPrefabs[newSegmentIndex];
        _lastSegmentIndex = newSegmentIndex;

        var segment = Instantiate(prefab, _nextSpawnPosition, Quaternion.identity, transform);
        _activeSegments.Enqueue(segment);
        _nextSpawnPosition = segment.NextSegmentPosition;
    }
    
}
