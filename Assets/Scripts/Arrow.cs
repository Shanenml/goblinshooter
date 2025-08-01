using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.0f;
    
    void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if (transform.position.x >= 11.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
