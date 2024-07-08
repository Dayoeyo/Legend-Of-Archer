using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("=== PLAYER INFORMATION ===")]
    public GameObject _player;

    protected override void InitManager()
    {
    }
}
