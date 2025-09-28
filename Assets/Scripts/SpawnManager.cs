using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _isPlayerAlive = true;
    [SerializeField]
    private bool _isEnemyWaveSpawning = true;

    [SerializeField]
    private int _enemiesSpawned = 0;
    [SerializeField]
    private int _enemiesDestroyed = 0;
    [SerializeField]
    private int _waveNumber = 1;
    [SerializeField]
    private int _enemiesIncreasePerWave = 3;
    [SerializeField]
    private int _waveTransitionTarget;

    private UIManager _uiManager;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null)
        {
            Debug.LogError("The UI MANAGER is NULL");
        }

        _waveTransitionTarget = 5; //first wave
    }
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    public void OnPlayerDeath()
    {
        _isPlayerAlive = false;
    }
    IEnumerator SpawnEnemyRoutine()
    {
        while(_isPlayerAlive == true)
        {
            Vector3 posToSpawn = new Vector3(11, Random.Range(-2.0f, 5.0f), 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            _enemiesSpawned++;

            if(_enemiesSpawned >= _waveTransitionTarget)
            {
                _isEnemyWaveSpawning = false;
                yield break;
            }

            yield return new WaitForSeconds(5.0f);
        }
    }

    public void EnemyWaveTransitionTracker()
    {
        _enemiesDestroyed++;
        if(_enemiesDestroyed >= _waveTransitionTarget && _isEnemyWaveSpawning == false)
        { 
            _waveTransitionTarget += _enemiesSpawned + (_waveNumber * _enemiesIncreasePerWave);
            
            StartCoroutine(EnemyNextWaveRoutine());
        }
    }

    IEnumerator EnemyNextWaveRoutine()
    {
        yield return new WaitForSeconds(5.5f);
        Debug.Log("Enemy Wave " + _waveNumber + " is spawning " + _waveTransitionTarget + " number of enemies");
        _waveNumber++;
        _uiManager.UpdateWave(_waveNumber, _waveTransitionTarget);
        _isEnemyWaveSpawning = true;
        yield return new WaitForSeconds(5.5f);
        StartCoroutine(SpawnEnemyRoutine());

    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while(_isPlayerAlive == true)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 6, 0);
            int randomPowerUp = Random.Range(0, 6);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
        }
        
    }

}
