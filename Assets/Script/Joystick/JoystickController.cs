using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : Singleton<JoystickController>
{
    [Header("=== ABOUT CONTROLLER ===")]
    [SerializeField] private GameObject smallStick;
    [SerializeField] private GameObject bigStick;
    private float stickRadius;
    private Vector3 stickStartPosition;
    public Vector3 joystickVector;



    protected override void InitManager()
    {
        stickRadius = bigStick.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2;
        stickStartPosition = bigStick.transform.position;
    }

    public void PointDown()
    {
        bigStick.transform.position = Input.mousePosition;
        smallStick.transform.position = Input.mousePosition;
        stickStartPosition = Input.mousePosition;
        GameManager.Instance._player.GetComponent<PlayerController>().F_PlayerMoveAnimation(true);
    }

    public void Drag(BaseEventData v_baseEvent)
    {
        PointerEventData ped = v_baseEvent as PointerEventData;
        Vector3 DragPosition = ped.position;
        joystickVector = (DragPosition - stickStartPosition).normalized;
        float stickDistance = Vector3.Distance(DragPosition, stickStartPosition);

        if (stickDistance < stickRadius)
        {
            smallStick.transform.position = stickStartPosition + joystickVector * stickDistance;
        }
        else
        {
            smallStick.transform.position = stickStartPosition + joystickVector * stickRadius;
        }
        GameManager.Instance._player.GetComponent<PlayerController>().rb.rotation =
            Quaternion.LookRotation(new Vector3(joystickVector.x, 0, joystickVector.y));

    }

    public void Drop()
    {
        joystickVector = Vector3.zero;
        smallStick.transform.position = stickStartPosition;
        bigStick.transform.position = stickStartPosition;
        if (PlayerController.Instance._finaltarget != null)
        {
            GameManager.Instance._player.GetComponent<PlayerController>().transform.LookAt(PlayerController.Instance.monsterTransform);
        }
        GameManager.Instance._player.GetComponent<PlayerController>().playerAni.SetBool("Move", false);
    }

}
