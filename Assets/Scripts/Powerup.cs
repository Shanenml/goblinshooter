using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    private float _randomStopSpot;
    
    [SerializeField]
    private int _powerupID;
    //ID for Powerups
    //0 = Triple Shot
    //1 = Speed
    //2 = Shields
    private Player _player;

    [SerializeField]
    private AudioClip _collectedClip;
    void Start()
    {
        _randomStopSpot = Random.Range(-1.0f, 4.0f);
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("PLAYER is NULL");
        }
        

    }

    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= _randomStopSpot)
        {
            _speed = 0f;
            StartCoroutine(PowerUpDespawnRoutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(_collectedClip, transform.position, 1f);
            switch (_powerupID)
            {
                case 0:
                    _player.TripleShotActive();
                    break;
                case 1:
                    _player.SpeedBoostActive();
                    break;
                case 2:
                    _player.ShieldActive();
                    break;
                default:
                    Debug.Log("Default Powerup Value");
                    break;
            }
            
            Destroy(this.gameObject);
        }
    }

    IEnumerator PowerUpDespawnRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(5.0f);
            Destroy(this.gameObject);
        }
    }


}
