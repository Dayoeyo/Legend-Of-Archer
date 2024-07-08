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
    private Animator _player_Animator;
    public Animator playerAni => _player_Animator;

    protected override void InitManager()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Animator = GetComponent<Animator>();
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
        _rb.velocity = new Vector3(JoystickController.Instance.joystickVector.x, _rb.velocity.y, JoystickController.Instance.joystickVector.y) * _moveSpeed;
        _rb.rotation = Quaternion.LookRotation(new Vector3(JoystickController.Instance.joystickVector.x,0, JoystickController.Instance.joystickVector.y));
    }

    public void F_PlayerMoveAnimation(bool v_state)
    {
        _player_Animator.SetBool("Move", v_state);
        Debug.Log(_player_Animator.GetBool("Move"));
    }
}
