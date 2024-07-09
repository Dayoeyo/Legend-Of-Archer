using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public delegate void playerDelegate();
    public playerDelegate _player_Ctr;

    [Header("=== ABOUT TRACKING ===")]
    [SerializeField] private LayerMask _monsterLayer;
    private RaycastHit monsterRaycastHit;
    public Dictionary<GameObject, float> monsterDistance;
    public Transform monsterTransform;

    [Header("=== ABOUT MOVEMENT ===")]
    private Rigidbody _rb;
    public Rigidbody rb => _rb;
    private float _moveSpeed = 5f;
    private Animator _player_Animator;
    public Animator playerAni => _player_Animator;


    protected override void InitManager()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Animator = GetComponent<Animator>();
        _player_Ctr = F_PlayerMove;
        monsterDistance = new Dictionary<GameObject, float>();
    }

    private void Update()
    {
        _player_Ctr();
    }

    private void F_PlayerMove()
    {
        float _moveHorizontal = Input.GetAxis("Horizontal");
        float _moveVertical = Input.GetAxis("Vertical");
        _rb.velocity = new Vector3(JoystickController.Instance.joystickVector.x, 0, JoystickController.Instance.joystickVector.y) * _moveSpeed;

    }

    public void F_PlayeTrackingStart()
    {
        _player_Ctr += F_PlayerTrackingMonster;
    }

    private void F_PlayerTrackingMonster()
    {
        GameObject _target;
        for (int l = 0; l < RoomCondition.Instance.monsterList.Count; l++)
        {
            Vector3 rayStartPosition = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            Vector3 rayEndPosition = new Vector3(RoomCondition.Instance.monsterList[l].transform.position.x, RoomCondition.Instance.monsterList[l].transform.position.y + 0.3f,
                RoomCondition.Instance.monsterList[l].transform.position.z);
            Vector3 rayDirect = rayEndPosition - rayStartPosition;
            Debug.DrawRay(rayStartPosition, rayDirect);
            if (Physics.Raycast(rayStartPosition, rayDirect, out monsterRaycastHit))
            {
                if (!monsterRaycastHit.collider.CompareTag("Monster"))
                {
                    monsterDistance[RoomCondition.Instance.monsterList[l]] = 1000f;
                    _target = null;
                    continue;
                }
                else
                {
                    monsterDistance[RoomCondition.Instance.monsterList[l]] = Vector3.Distance(transform.position, RoomCondition.Instance.monsterList[l].transform.position);
                    _target = F_SetPlayerAttackMonster(monsterDistance);
                    monsterTransform = _target.transform;
                    continue;
                }
            }
        }
    }

    private GameObject F_SetPlayerAttackMonster(Dictionary<GameObject, float> v_distanceDic)
    {
        v_distanceDic = v_distanceDic.OrderBy(d => d.Value).ToDictionary(d => d.Key, d => d.Value);
        return v_distanceDic.First().Key;
    }
    
    public void F_PlayerMoveAnimation(bool v_state)
    {
        _player_Animator.SetBool("Move", v_state);
    }
}
