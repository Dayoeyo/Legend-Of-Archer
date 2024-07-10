using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCondition : Singleton<RoomCondition>
{
    public bool playerInThisRoom = false;
    public bool isCleared = false;
    public bool isVisited = false;
    public int roomNum;

    [SerializeField] private List<GameObject> _monsterList = new List<GameObject>();
    public List<GameObject> monsterList => _monsterList;
    private bool isMonsterHere = false;

    private Vector3 startPosition = new Vector3(0, 0, -9);
    public Vector3 startPos => startPosition;

    protected override void InitManager()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            _monsterList.Add(other.gameObject);
            if (!isMonsterHere)
            {
                isMonsterHere = true;
            }
        }
        if(other.CompareTag("Player") && !isMonsterHere)
        {
            isCleared = true;
        }

        if (other.CompareTag("Player") && isMonsterHere)
        {
            playerInThisRoom = true;
            foreach (GameObject monster in _monsterList)
            {
                PlayerController.Instance.monsterDistance.Add(monster, 0f);
            }
            if (!PlayerController.Instance._isTracking)
            {
                PlayerController.Instance.F_PlayeTrackingStart();
                Debug.Log("몬스터 출현, 추격 시작");
                PlayerController.Instance._isTracking = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInThisRoom = false;
        }
        if (other.CompareTag("Monster"))
        {
            _monsterList.Remove(other.gameObject);
        }
    }

    public void F_ReturnEnemy(GameObject v_go)
    {
        v_go.SetActive(false);
        _monsterList.Remove(v_go);
        if (_monsterList.Count <= 0)
        {
            isCleared = true;
            PlayerController.Instance.F_PlayerTrackingEnd();
        }
    }
}
