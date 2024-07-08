using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public delegate void playerDelegate();
    public playerDelegate _player_Ctr;
    

    [Header("=== ABOUT MOVEMENT ===")]
    private Rigidbody _rb;
    private float _moveSpeed = 5f;

    protected override void InitManager()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Ctr = F_PlayerMove;
    }

    private void Update()
    {
        _player_Ctr();
    }

    private void F_PlayerMove()
    {
        float _moveHorizontal = Input.GetAxis("Horizontal");
        float _moveVertical = Input.GetAxis("Vertical");

        _rb.velocity = new Vector3(_moveHorizontal, _rb.velocity.y, _moveVertical) * _moveSpeed;
    }

}
