using GameFramework.Network;
using Google.Protobuf;
using MO.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class UIEvent : MonoBehaviour
{
	public void OnLoginBtnClick()
	{
		var webComponent = GameEntry.GetComponent<WebRequestComponent>();
		
		C2S_1003 content = new C2S_1003();
		content.DeviceId = GameUser.Instance.DeviceId;
		content.MobileType = 1;
		var url = string.Format("http://localhost:8001/api/c2s1003?data={0}",
			content.ToByteString().ToBase64());
		webComponent.AddWebRequest(url, new NetworkUserData() { ActionId = 1003 });
	}

	public void OnLogoutBtnClick()
    {

    }

	public void OnEnterBtnClick()
	{
		C2S100001 content = new C2S100001();
		content.RoomId = 10000;
		var packet = PacketHelper.BuildPacket(100001, content);
		GameUser.Instance.NetworkChannel.Send(packet);
	}

	public void OnLeaveBtnClick()
    {
		C2S100005 content = new C2S100005();
		var packet = PacketHelper.BuildPacket(100005, content);
		GameUser.Instance.NetworkChannel.Send(packet);
	}
}
