using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTeleport : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StageManager.Instance.F_ChangeRoom();
        }

    }
}
