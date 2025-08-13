using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1.0f;

    [SerializeField]
    private int _score;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _arrowPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shields;

    [SerializeField]
    private AudioClip _arrowSound;
    private AudioSource _audioSource;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private Animator _animator;

    void Start()
    {
        transform.position = new Vector3(-5, 1, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("The SPAWN MANAGER is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null)
        {
            Debug.LogError("The UI MANAGER is NULL");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.LogError("The GAME MANAGER is NULL");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("The ANIMATOR is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogError("The AUDIO SOURCE is NULL");
        }
        else
        {
            _audioSource.clip = _arrowSound;
        }
    }

    void Update()
    {
        CalculateMovement();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireArrow();
        }
        
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);
        

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -5.0f, 6.0f), 0);

        if (transform.position.x <= -11.5f)
        {
            transform.position = new Vector3(11.5f, transform.position.y, 0);
        }
        else if (transform.position.x >= 11.5f)
        {
            transform.position = new Vector3(-11.5f, transform.position.y, 0);
        }
    }

    void FireArrow()
    {
        _canFire = Time.time + _fireRate;
        _animator.SetTrigger("OnPlayerFire");
        if(_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(-0.3f, -0.7f, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(_arrowPrefab, transform.position + new Vector3(-0.3f, -0.7f, 0), Quaternion.identity);
        }
        _audioSource.Play();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "EnemyArrow")
        {
            Damage();
            Destroy(other.gameObject);
        }
    }

    public void Damage()
    {

        if(_isShieldActive == true)
        {
            _shields.SetActive(false);
            _isShieldActive = false;
            return;
        }

        _lives--;
        _uiManager.UpdateLives(_lives);

        if(_lives <= 0)
        {
            _spawnManager.OnPlayerDeath();
            _gameManager.GameOver();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        while(_isTripleShotActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isTripleShotActive = false;
        }
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        while(_isSpeedBoostActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isSpeedBoostActive = false;
            _speed /= _speedMultiplier;
        }
    }

    public void ShieldActive()
    {
        _shields.SetActive(true);
        _isShieldActive = true;
    }

    public void UpdateScore()
    {
        _score += 1;
        _uiManager.UpdateScore(_score);
    }
}
