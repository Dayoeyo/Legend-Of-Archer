using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float offsetY = 45f;
    public float offsetZ = -40f;

    private Vector3 cameraPosition;

    private void Update()
    {
        cameraPosition.y = GameManager.Instance._player.transform.position.y + offsetY;
        cameraPosition.z = GameManager.Instance._player.transform.position.z + offsetZ;

        transform.position = cameraPosition;
    }
}
