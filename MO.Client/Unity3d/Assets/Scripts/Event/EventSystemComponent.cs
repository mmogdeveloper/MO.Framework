using GameFramework;
using GameFramework.Network;
using Google.Protobuf;
using MO.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

public class EventSystemComponent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var eventCom = GameEntry.GetComponent<EventComponent>();
		eventCom.SetDefaultHandler(EventCallback);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
						PlayerData playerData = new PlayerData();
						playerData.UserId = rep1003.UserId;
						GameUser.Instance.PlayerData = playerData;
						GameUser.Instance.Token = rep1003.Token;
						GameUser.Instance.GateIP = rep1003.GateIP;
						GameUser.Instance.GatePort = rep1003.GatePort;
						Log.Info(string.Format("{0},登录成功", GameUser.Instance.PlayerData.UserId));

						var tcpNetwork = GameEntry.GetComponent<NetworkComponent>();
						GameUser.Instance.NetworkChannel = tcpNetwork.CreateNetworkChannel("Global", ServiceType.Tcp, new NetworkChannelHelper());
						GameUser.Instance.NetworkChannel.Connect(IPAddress.Parse(GameUser.Instance.GateIP), GameUser.Instance.GatePort);
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

			if(arg is UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs)
            {

            }
		}
	}
}
