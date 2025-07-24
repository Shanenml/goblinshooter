using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _speed = 4.0f;

    private Animator _animator;
    private Player _player;
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip[] _deathSound;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("The PLAYER is NULL");
        }
        
        _animator = GetComponent<Animator>();
        if(_animator == null)
        {
            Debug.LogError("The ANIMATOR is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogError("The AUDIO SOURCE is NULL");
        }

        _audioSource.clip = _deathSound[Random.Range(0, 3)];
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
            _player.UpdateScore();
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 0.8f);
        }


        if(other.tag == "Arrow")
        {
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
