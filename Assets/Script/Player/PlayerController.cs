using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : Singleton<PlayerController>
{
    public delegate void playerDelegate();
    public playerDelegate _player_Ctr;

    [Header("=== ABOUT TRACKING ===")]
    [SerializeField] private LayerMask _monsterLayer;
    public bool _isTracking = false;
    public Dictionary<GameObject, float> monsterDistance;
    public Transform monsterTransform;
    private RaycastHit monsterRaycastHit;

    [Header("=== ABOUT MOVEMENT ===")]
    private Rigidbody _rb;
    public Rigidbody rb => _rb;
    private float _moveSpeed = 5f;
    private Animator _player_Animator;
    public Animator playerAni => _player_Animator;

    [Header("=== ABOUT ATTACK ===")]
    [SerializeField] private GameObject _player_Bullet;
    [SerializeField] private GameObject _bullet_parent;
    private Queue<PlayerBullet> _bullet_Pool;
    private int _bullet_Count = 50;
    public Vector3 _bullet_MoveVec = Vector3.zero;

    protected override void InitManager()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Animator = GetComponent<Animator>();

        _player_Ctr = F_PlayerMove;

        monsterDistance = new Dictionary<GameObject, float>();

        //총알 오브젝트 풀링
        _bullet_Pool = new Queue<PlayerBullet>();
        for (int l = 0; l < _bullet_Count; l++)
        {
            F_CreateBullet();
        }
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
        StartCoroutine(F_PlayerAttack());
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
                if (Physics.Raycast(rayStartPosition, rayDirect, out monsterRaycastHit))
                {
                    if (monsterRaycastHit.collider.CompareTag("Monster"))
                    {
                        monsterDistance[RoomCondition.Instance.monsterList[l]] = Vector3.Distance(transform.position, RoomCondition.Instance.monsterList[l].transform.position);
                        _target = F_SetPlayerAttackMonster(monsterDistance);
                        monsterTransform = _target.transform;
                        _bullet_MoveVec = (monsterTransform.position - transform.position).normalized;
                        continue;
                }
                    else
                    {
                        monsterDistance[RoomCondition.Instance.monsterList[l]] = 1000f;
                        _target = null;
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
    
    private void F_CreateBullet()
    {
        PlayerBullet _bullet = Instantiate(_player_Bullet).GetComponent<PlayerBullet>();
        _bullet.gameObject.transform.SetParent(_bullet_parent.transform);
        _bullet_Pool.Enqueue(_bullet.GetComponent<PlayerBullet>());
    }

    private IEnumerator F_PlayerAttack()
    {
        yield return new WaitForSeconds(0.1f);
        if (_bullet_Pool.Count <= 0)
        {
            F_CreateBullet();
        }
        while (_bullet_Pool.Count > 0)
        {
            PlayerBullet _pb = _bullet_Pool.Dequeue();
            _pb.gameObject.SetActive(true);
            _pb.F_MoveBullet(_bullet_MoveVec);
            yield return new WaitForSeconds(1f);
        }
    }
    


    public void F_ReturnBulletPool(PlayerBullet v_pb)
    {
        _bullet_Pool.Enqueue(v_pb);
        v_pb.transform.SetParent(_bullet_parent.transform);
        v_pb.F_InitBullet();
    }

    public void F_PlayerMoveAnimation(bool v_state)
    {
        _player_Animator.SetBool("Move", v_state);
    }
}
