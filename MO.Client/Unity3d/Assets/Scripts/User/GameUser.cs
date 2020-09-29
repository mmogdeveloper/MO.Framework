using GameFramework;
using GameFramework.Network;
using Google.Protobuf;
using MO.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

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
	public void Initialization()
	{
		Instance.CurPlayer = new PlayerData();
		Instance.CurPlayer.GameObject = (GameObject)Resources.Load("Self");
		GameObject prefabInstance = GameObject.Instantiate(Instance.CurPlayer.GameObject);
		prefabInstance.transform.parent = GameObject.Find("Canvas").gameObject.transform;
		Instance.ViewPlayers = new Dictionary<Int64, PlayerData>();
		var eventCom = GameEntry.GetComponent<EventComponent>();
		eventCom.SetDefaultHandler(EventCallback);
	}
    public Dictionary<Int64, PlayerData> ViewPlayers { get; set; }
    public PlayerData CurPlayer { get; set; }
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

	private void EventCallback(object sender, BaseEventArgs arg)
	{
		if (sender is WebRequestComponent)
		{
			if (arg is UnityGameFramework.Runtime.WebRequestSuccessEventArgs)
			{
				var webarg = (WebRequestSuccessEventArgs)arg;
				var userData = webarg.UserData as NetworkUserData;
				if (userData != null)
				{
					//登录成功
					if (userData.ActionId == 1003)
					{
						var strResult = Encoding.UTF8.GetString(webarg.GetWebResponseBytes());
						var moResult = MOMsgResult.Parser.ParseFrom(ByteString.FromBase64(strResult));
						var rep1003 = S2C_1003.Parser.ParseFrom(moResult.Content);
						GameUser.Instance.CurPlayer.UserId = rep1003.UserId;
						GameUser.Instance.Token = rep1003.Token;
						GameUser.Instance.GateIP = rep1003.GateIP;
						GameUser.Instance.GatePort = rep1003.GatePort;
						Log.Info(string.Format("{0},登录成功", GameUser.Instance.CurPlayer.UserId));

						var tcpNetwork = GameEntry.GetComponent<NetworkComponent>();
						if (GameUser.Instance.NetworkChannel == null)
						{
							GameUser.Instance.NetworkChannel = tcpNetwork.CreateNetworkChannel("Global", ServiceType.Tcp, new NetworkChannelHelper());
						}
						if (!GameUser.Instance.NetworkChannel.Connected)
						{
							GameUser.Instance.NetworkChannel.Connect(IPAddress.Parse(GameUser.Instance.GateIP), GameUser.Instance.GatePort);
						}
					}
				}
			}
		}

		if (sender is NetworkComponent)
		{
			if (arg is UnityGameFramework.Runtime.NetworkConnectedEventArgs)
			{
				C2S100000 content = new C2S100000();
				GameUser.Instance.NetworkChannel.Send(PacketHelper.BuildPacket(100000, content));
			}

			if (arg is UnityGameFramework.Runtime.NetworkClosedEventArgs)
			{

			}

			if (arg is UnityGameFramework.Runtime.NetworkErrorEventArgs)
			{

			}

			if (arg is UnityGameFramework.Runtime.NetworkCustomErrorEventArgs)
			{

			}

			if (arg is UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs)
			{

			}
		}
	}
}
