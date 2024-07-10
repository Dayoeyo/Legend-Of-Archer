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
    private bool _canAttack = false;
    private GameObject _target;
    private GameObject _Finaltarget;

    [Header("=== ABOUT MOVEMENT ===")]
    private Rigidbody _rb;
    public Rigidbody rb => _rb;
    private float _moveSpeed = 5f;
    private Animator _player_Animator;
    public Animator playerAni => _player_Animator;

    [Header("=== ABOUT ATTACK ===")]
    [SerializeField] private GameObject _player_Bullet;
    [SerializeField] private GameObject _bullet_parent;
    private IEnumerator _attack_Coroutine;
    private RoomCondition _currentRoom;
    private Queue<PlayerBullet> _bullet_Pool;
    private int _bullet_Count = 50;
    private float _attack_Range = 5f;
    public Vector3 _bullet_MoveVec = Vector3.zero;

    protected override void InitManager()
    {
        _rb = GetComponent<Rigidbody>();
        _player_Animator = GetComponent<Animator>();

        //델리게이트
        _player_Ctr = F_PlayerMove;

        //방 설정
        transform.parent.GetComponent<RoomCondition>().isVisited = true;
        StageManager.Instance.currentRoomNum = transform.parent.gameObject.GetComponent<RoomCondition>().roomNum;
        monsterDistance = new Dictionary<GameObject, float>();

        //코루틴 설정
        _attack_Coroutine = F_PlayerAttack();

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
        _currentRoom = transform.parent.gameObject.GetComponent<RoomCondition>();
        _player_Ctr += F_PlayerTrackingMonster;
        StartCoroutine(_attack_Coroutine);
    }

    public void F_PlayerTrackingEnd()
    {
        _player_Ctr -= F_PlayerTrackingMonster;
        _player_Animator.SetBool("Attack", false);
        StopCoroutine(_attack_Coroutine);
    }
    private void F_PlayerTrackingMonster()
    {
            for (int l = 0; l < _currentRoom.monsterList.Count; l++)
            {
                Vector3 rayStartPosition = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
                Vector3 rayEndPosition = new Vector3(_currentRoom.monsterList[l].transform.position.x, _currentRoom.monsterList[l].transform.position.y + 0.3f,
                    _currentRoom.monsterList[l].transform.position.z);
                Vector3 rayDirect = rayEndPosition - rayStartPosition;
                Debug.DrawRay(rayStartPosition, rayDirect, Color.red);
                _canAttack = Physics.Raycast(rayStartPosition, rayDirect, out monsterRaycastHit);
                if (_canAttack)
                {
                    if (monsterRaycastHit.collider.CompareTag("Monster"))
                    {
                        monsterDistance[_currentRoom.monsterList[l]] = Vector3.Distance(transform.position, _currentRoom.monsterList[l].transform.position);
                        _target = F_SetPlayerAttackMonster(monsterDistance);
                        _Finaltarget = _target;
                        monsterTransform = _target.transform;
                        _bullet_MoveVec = (monsterTransform.position - transform.position).normalized;
                        continue;
                    }
                    else
                    {
                        monsterDistance[_currentRoom.monsterList[l]] = 1000f;
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
        yield return new WaitForSeconds(0.5f);
        if (_bullet_Pool.Count <= 0)
        {
            F_CreateBullet();
        }
        while (_bullet_Pool.Count > 0)
        {
            //Debug.Log(_Finaltarget);
            if (_Finaltarget != null)
            {
                if (Vector3.Distance(transform.position, _Finaltarget.transform.position) <= _attack_Range)
                {
                    PlayerBullet _pb = _bullet_Pool.Dequeue();
                    _pb.gameObject.SetActive(true);
                    _pb.F_MoveBullet(_bullet_MoveVec);
                    _player_Animator.SetBool("Attack", true);
                }
                else
                {
                    _player_Animator.SetBool("Attack", false);
                }
            }
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
