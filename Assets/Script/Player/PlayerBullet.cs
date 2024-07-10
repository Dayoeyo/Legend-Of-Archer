using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody rb;
    private float _moveSpeed = 4f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        F_InitBullet();
    }

    public void F_InitBullet()
    {
        transform.localPosition = Vector3.zero;
        rb.velocity = Vector3.zero; 
        gameObject.SetActive(false);
    }

    public void F_MoveBullet(Vector3 v_Vec)
    {
        transform.SetParent(null);
        rb.velocity = v_Vec * _moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Wall"))
        {
            PlayerController.Instance.F_ReturnBulletPool(this);
        }
        
    }
}
