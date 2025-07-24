using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    [SerializeField]
    private GameObject _wood;
    private SpriteRenderer _spriteRenderer;
    private SpawnManager _spawnManager;

    private bool _isSheepFree = false;
    private bool _sheepConfused = false;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_spriteRenderer == null)
        {
            Debug.LogError("The SPRITE MANAGER is NULL");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("The SPAWN MANAGER is NULL");
        }

    }
    void Update()
    {
        if(_isSheepFree == true)
        {
            transform.Translate(Vector3.left * 8 * Time.deltaTime);
        }

        if(transform.position.x < -11)
        {
            _spawnManager.StartSpawning();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Arrow")
        {
            _wood.SetActive(false);
            Destroy(other.gameObject);
            _sheepConfused = true;
            StartCoroutine(RunningAwayRoutine());
        }
    }

    IEnumerator RunningAwayRoutine()
    {
        while(_sheepConfused == true)
        {
            yield return new WaitForSeconds(0.5f);
            _spriteRenderer.flipX = true;
            yield return new WaitForSeconds(0.5f);
            _spriteRenderer.flipX = false;
            yield return new WaitForSeconds(0.5f);
            _spriteRenderer.flipX = true;
            _sheepConfused = false;
            _isSheepFree = true;
        }
    }


}
