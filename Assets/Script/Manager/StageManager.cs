using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [Header("=== ABOUT ROOMS ===")]
    [SerializeField] List<RoomCondition> roomList;
    public int currentRoomNum;
    private int nextRoomNum;

    [Header("=== ABOUT CAMERA ===")]
    [SerializeField] MainCamera _mainCamera;

    protected override void InitManager()
    {
    }

    public void F_ChangeRoom()
    {
        F_SetRoomNum();
        GameObject player = GameManager.Instance._player;
        player.transform.SetParent(roomList[nextRoomNum].transform);
        player.transform.localPosition = player.transform.parent.GetComponent<RoomCondition>().startPos;
        _mainCamera.cameraPosition = new Vector3(player.transform.position.x, _mainCamera.cameraPosition.y + _mainCamera.offsetY, _mainCamera.cameraPosition.z + _mainCamera.offsetZ);
        roomList[nextRoomNum].isVisited = true;

    }

    private void F_SetRoomNum()
    {
        nextRoomNum = Random.Range(0,roomList.Count);
        Debug.Log(nextRoomNum);
        if (roomList[nextRoomNum].isVisited)
        {
            Debug.Log("이미 방문한 방 : " + nextRoomNum + ", " + roomList[nextRoomNum].isVisited);
            F_SetRoomNum();
        }
        else
        {
            currentRoomNum = nextRoomNum;
            return;
        }
    }
}
