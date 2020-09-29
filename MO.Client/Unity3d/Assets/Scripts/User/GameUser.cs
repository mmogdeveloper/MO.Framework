using GameFramework.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自己信息
/// </summary>
public class GameUser
{
    public static GameUser Instance { get; }
    static GameUser()
    {
        Instance = new GameUser();
    }
    public PlayerData PlayerData { get; set; }
    public string Token { get; set; }
    private string _deviceId;
    public string DeviceId
    {
        get
        {
            if (string.IsNullOrEmpty(_deviceId))
                _deviceId = Guid.NewGuid().ToString("N");
            return _deviceId;
        }
    }
    private int _msgId;
    public int MsgId
    {
        get
        {
            _msgId++;
            return _msgId;
        }
    }

    public string GateIP { get; set; }
    public int GatePort { get; set; }
    public INetworkChannel NetworkChannel { get; set; }
}
