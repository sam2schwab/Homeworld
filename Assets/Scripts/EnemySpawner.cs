﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject ship;
    public GameObject capitalShip;
    public float difficultyFactor = 1;

    //waves
    public bool lmsMode; //Last Man Standing : the next wave starts only when all enemies are dead.
    private bool lmsCheck = true;
    public float waveDuration = 10;
    public float pauseDuration = 2f;
    public float scalingFactor = 1.1f;
    [SerializeField]
    private float _nextPause;
    private float _nextWave;

    public float intervalBetweenShips = 12f;
    private float _nextSpawn;
    public float intervalBetweenCapitalShips = 53f;
    private float _nextCapitalSpawn;
    private float _spawnRadius;
    private float _maxZ;
    private float _maxX;

    // Start is called before the first frame update
    void Start()
    {
        var worldPoint = GameManager.Instance.MainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, GameManager.Instance.MainCamera.transform.position.y)); //Melchi : I corrected the z value to be fed automatically by the game, so if we change the camera it doesn't break.
        _spawnRadius = worldPoint.magnitude + 2f; // Melchi : I corrected the radius so any value is only clamped on 1 axis at the time.  
        _maxX = -worldPoint.x + 2f;
        _maxZ = -worldPoint.z + 2f;
        var now = Time.time;
        _nextSpawn = now;
        _nextPause = now + waveDuration;
        _nextWave = _nextPause + pauseDuration;
        _nextCapitalSpawn = now + intervalBetweenCapitalShips;
        GameManager.Instance.wave = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (lmsMode) lmsUpdate();
        else normalUpdate();
    }

    private void normalUpdate()
    {
        UpdateDifficulty();
        if (Time.time > _nextSpawn)
        {
            Spawn(SpawnType.Ship);
            _nextSpawn += intervalBetweenShips / difficultyFactor;
        }
        if (Time.time > _nextCapitalSpawn)
        {
            Spawn(SpawnType.CapitalShip);
            _nextCapitalSpawn += intervalBetweenCapitalShips / difficultyFactor;
        }
    }

    private void UpdateDifficulty()
    {
        if (Time.time > _nextWave)
        {
            GameManager.Instance.wave++;
            _nextWave += waveDuration + pauseDuration;
            difficultyFactor *= scalingFactor; 
            _nextSpawn += (_nextSpawn - Time.time) / scalingFactor; 
            _nextCapitalSpawn += (_nextCapitalSpawn - Time.time) / scalingFactor;
        }

        if (Time.time > _nextPause)
        {
            _nextPause += waveDuration + pauseDuration;
            _nextSpawn += pauseDuration;
            _nextCapitalSpawn += pauseDuration;
        }
    }

    private void lmsUpdate()
    {
        if (Time.time < _nextPause)
        {
            if (Time.time > _nextSpawn)
            {
                Spawn(SpawnType.Ship).SetParent(transform);
                _nextSpawn += intervalBetweenShips / difficultyFactor;
                lmsCheck = true;
            }
            if (Time.time > _nextCapitalSpawn)
            {
                Spawn(SpawnType.CapitalShip).SetParent(transform);
                _nextCapitalSpawn += intervalBetweenCapitalShips / difficultyFactor;
                lmsCheck = true;
            }
        }
        else if (transform.childCount == 0 && lmsCheck)
        {
            lmsNextWave();
            lmsCheck = false;
        }
    }

    private void lmsNextWave()
    {
        GameManager.Instance.wave++;
        difficultyFactor *= scalingFactor;
        _nextSpawn = Time.time;
        _nextCapitalSpawn = Time.time + intervalBetweenCapitalShips / difficultyFactor;
        _nextPause = Time.time + waveDuration;
    }

    private Transform Spawn(SpawnType type)
    {

        var angle = GameManager.Rng.NextDouble() * 360;
        var position =  Quaternion.Euler(0, (float) angle, 0) * Vector3.left * _spawnRadius;
        position.x = Mathf.Clamp(position.x, -_maxX, _maxX);
        position.z = Mathf.Clamp(position.z, -_maxZ, _maxZ);
        position.y = 8;

        switch (type)
        {
            case SpawnType.Ship:
                return Instantiate(ship, position, Quaternion.identity).transform;
            case SpawnType.CapitalShip:
                return Instantiate(capitalShip, position, Quaternion.identity).transform;
            case SpawnType.ShipCluster:
                return null;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }
}

internal enum SpawnType
{
    Ship,
    CapitalShip,
    ShipCluster
}
