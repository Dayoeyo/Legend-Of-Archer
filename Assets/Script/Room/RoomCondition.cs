using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCondition : Singleton<RoomCondition>
{
    public bool playerInThisRoom = false;
    public bool isCleared = false;
    private List<GameObject> _monsterList = new List<GameObject>();
    public List<GameObject> monsterList => _monsterList;

    protected override void InitManager()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInThisRoom = true;
        }
        if (other.CompareTag("Monster"))
        {
            Debug.Log("There is Monster : " + other.gameObject.name);
            _monsterList.Add(other.gameObject);
            PlayerController.Instance.monsterDistance.Add(other.gameObject, 0f);
            PlayerController.Instance.F_PlayeTrackingStart();
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
}
