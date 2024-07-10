using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            _monsterList.Add(other.gameObject);
            PlayerController.Instance.monsterDistance.Add(other.gameObject, 0f);
            if (!PlayerController.Instance._isTracking)
            {
                PlayerController.Instance.F_PlayeTrackingStart();
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
}
