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
    private float _sprintBoostSpeed = 2.0f;
    [SerializeField]
    private float _sprintBoostValue = 1.0f;

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
    private GameObject _shields1;
    [SerializeField]
    private GameObject _shields2;
    [SerializeField]
    private GameObject _shields3;
    private int _shieldLives;
    private int _arrowAmmo;

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
        _arrowAmmo = 15;

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

        if(Input.GetKey(KeyCode.LeftShift) && _sprintBoostValue > 0)
        {
            StopCoroutine(SprintBoostRechargeRoutine());
            transform.Translate(direction * _speed * _sprintBoostSpeed * Time.deltaTime);
            _sprintBoostValue -= 0.01f;
            _uiManager.UpdateSprintSlider(_sprintBoostValue);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        { 
            StartCoroutine(SprintBoostRechargeRoutine());
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        

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

    IEnumerator SprintBoostRechargeRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        while(_sprintBoostValue < 1.0f)
        {
            _sprintBoostValue += 0.1f;
            if(_sprintBoostValue >= 1.0f)
            {
                _sprintBoostValue = 1.0f;
            }
            _uiManager.UpdateSprintSlider(_sprintBoostValue);
            yield return new WaitForSeconds(0.5f);
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
        else if(_arrowAmmo > 0)
        {
            Instantiate(_arrowPrefab, transform.position + new Vector3(-0.3f, -0.7f, 0), Quaternion.identity);
            _arrowAmmo--;
            _uiManager.UpdateArrows(_arrowAmmo);
        }
        else
        {
            
        }
        _audioSource.Play();

    }

    public void CollectAmmo()
    {
        _arrowAmmo = 15;
        _uiManager.UpdateArrows(_arrowAmmo);
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
            switch(_shieldLives)
            {
                case 3:
                    _shields3.SetActive(false);
                    _shieldLives--;
                    break;

                case 2:
                    _shields2.SetActive(false);
                    _shieldLives--;
                    break;

                case 1:
                    _shields1.SetActive(false);
                    _shieldLives = 0;
                    _isShieldActive = false;
                    break;

                default:
                    break;
            }
            
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

    public void CollectHealing()
    {
        if(_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }
        else
        {
            return;
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
        _shields1.SetActive(true);
        _shields2.SetActive(true);
        _shields3.SetActive(true);
        _shieldLives = 3;
        _isShieldActive = true;
    }

    public void UpdateScore()
    {
        _score += 1;
        _uiManager.UpdateScore(_score);
    }
}
