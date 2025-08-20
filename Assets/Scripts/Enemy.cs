using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _speed = 4.0f;
    private bool _isAlive;

    [SerializeField]
    private GameObject _enemyArrowPrefab;

    private Animator _animator;
    private Player _player;
    private AudioSource _audioSource;

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

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The AUDIO SOURCE is NULL");
        }

        _audioSource.clip = _deathSound[Random.Range(0, 3)];
        _isAlive = true;

        StartCoroutine(EnemyFireRoutine());
    }

    void Update()
    {
        CalculateMovement();
        CalculateNewPosition();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
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
            FireArrow();
            yield return new WaitForSeconds(0.6f);
            _animator.SetTrigger("OnEnemyRun");
            _speed = 4.0f;
        }
    }

    private void FireArrow()
    {
        
        Instantiate(_enemyArrowPrefab, transform.position + new Vector3(0.3f, -0.3f, 0), Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
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
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 0.8f);
        }


        if(other.tag == "Arrow")
        {
            StopCoroutine(EnemyFireRoutine());
            Destroy(other.gameObject);
            if(_player != null)
            {
                _player.UpdateScore();
            }
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 1.0f);
        }
    }

}
