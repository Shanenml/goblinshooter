using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.0f;
    [SerializeField]
    private int _arrowID;

    private bool _didArrowPenetrate = false;
    
    void Update()
    {

        CalculateMovement();
        if (transform.position.x >= 11.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    void CalculateMovement()
    {
        switch(_arrowID)
        {
            case 0:
                transform.Translate(Vector3.right * _speed * Time.deltaTime);
                break;
            case 1: //penetrating arrow
                transform.Translate(Vector3.right * _speed * Time.deltaTime);
                break;

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch(_arrowID)
        {
            case 0:
                if (other.tag == "Enemy")
                {
                    Destroy(this.gameObject);
                }
                break;
            case 1:
                if (other.tag == "Enemy" && _didArrowPenetrate == true)
                {
                    Destroy(this.gameObject);
                }
                else if(other.tag == "Enemy" && _didArrowPenetrate == false)
                {
                    _didArrowPenetrate = true;
                }
                break;
        }
        
    }
}
