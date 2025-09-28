using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _speed = 4.0f;
    private bool _isAlive;
    private int _enemyType;

    private bool _serpentineUp;

    [SerializeField]
    private GameObject _enemyArrowPrefab;

    private Animator _animator;
    private Player _player;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;

    [SerializeField]
    private AudioClip[] _deathSound;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The PLAYER is NULL");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("The ANIMATOR is NULL");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The SPAWN MANAGER is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The AUDIO SOURCE is NULL");
        }

        

        _audioSource.clip = _deathSound[Random.Range(0, 3)];
        _isAlive = true;

        DetermineEnemyType();
        StartCoroutine(EnemyFireRoutine());
    }

    void Update()
    {
        CalculateMovement();
        CalculateNewPosition();
    }

    private void DetermineEnemyType()
    {
        _enemyType = Random.Range(0, 2);
        switch(_enemyType)
        {
            case 0:
                //normal enemy
                break;
            case 1:
                StartCoroutine(SerpentineRoutine());
                break;
            default:
                //normal enemy
                break;
        }
    }

    void CalculateMovement()
    {
        switch (_enemyType)
        {
            case 0: //normal enemy
                transform.Translate(Vector3.left * _speed * Time.deltaTime);
                break;
            case 1: //serpentine enemy
                transform.Translate(Vector3.left * _speed * Time.deltaTime);
                if(_serpentineUp == true)
                {
                    transform.Translate(Vector3.up * _speed * Time.deltaTime);
                }
                else
                {
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);
                }
                break;
            default:
                //normal enemy
                break;
        }
        
    }

    IEnumerator SerpentineRoutine()
    {
        while(_isAlive == true)
        {
            yield return new WaitForSeconds(1.5f);
            _serpentineUp = true;
            yield return new WaitForSeconds(1.5f);
            _serpentineUp = false;
        }
    }

    void CalculateNewPosition()
    {
        if (transform.position.x <= -11.0f)
        {
            float randomY = Random.Range(-2.0f, 5.0f);
            transform.position = new Vector3(11, randomY, 0);
        }
    }

    IEnumerator EnemyFireRoutine()
    {
        while(_isAlive == true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            _speed = 0;
            _animator.SetTrigger("OnEnemyIdle");
            _animator.SetTrigger("OnEnemyFire");

            yield return new WaitForSeconds(0.5f);
            if(_isAlive == false)
            {
                yield break;
            }
            else
            {
                FireArrow();
            }
            

            yield return new WaitForSeconds(0.6f);
            if(_isAlive == false)
            {
                yield break;
            }
            else
            {
                _animator.SetTrigger("OnEnemyRun");
                _speed = 4.0f;
            }
            
        }
    }

    private void FireArrow()
    {
        
        Instantiate(_enemyArrowPrefab, transform.position + new Vector3(0.3f, -0.3f, 0), Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isAlive == true)
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            else
            {
                Debug.LogError("PLAYER is NULL");
            }
            StopCoroutine(EnemyFireRoutine());
            _player.UpdateScore();
            _spawnManager.EnemyWaveTransitionTracker();
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            _isAlive = false;
            Destroy(this.gameObject, 0.8f);
        }


        if(other.tag == "Arrow" && _isAlive == true)
        {
            StopCoroutine(EnemyFireRoutine());
            if(_player != null)
            {
                _player.UpdateScore();
            }
            _spawnManager.EnemyWaveTransitionTracker();
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            _isAlive = false;
            Destroy(this.gameObject, 1.0f);
        }
    }

}
