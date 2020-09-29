using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerMove : MonoBehaviour {

    public float MoveSpeed = 20f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //if (GameUser.Instance != null)
        //{
        //	foreach (var player in GameUser.Instance.ViewPlayers)
        //	{
        //		player.Value.GameObject.transform.position = new Vector3(player.Value.X, player.Value.Y);
        //	}
        //}
        if (Time.frameCount % 5 == 0)
        {
            var userId = Int64.Parse(this.name.Remove(this.name.IndexOf("(")));
            PlayerData playerData;
            if (GameUser.Instance.ViewPlayers.TryGetValue(userId, out playerData))
            {
                var curX = transform.position.x;
                var curY = transform.position.y;
                if ((int)curX == playerData.X &&
                    (int)curY == playerData.Y)
                {
                    return;
                }
                this.transform.position = Vector3.MoveTowards(new Vector3(curX, curY),
                    new Vector3(playerData.X, playerData.Y), MoveSpeed);
            }
            //var curX = transform.position.x;
            //var curY = transform.position.y;
            //this.name

            //if ((int)curX == (int)GameUser.Instance.CurPlayer.X &&
            //    (int)curY == (int)GameUser.Instance.CurPlayer.Y)
            //{
            //    return;
            //}
            //Log.Info("{0}({1},{2})", GameUser.Instance.CurPlayer.UserId, curX, curY);
            //C2S100003 content = new C2S100003();
            //content.X = curX;
            //content.Y = curY;
            //var packet = PacketHelper.BuildPacket(100003, content);
            //GameUser.Instance.CurPlayer.X = curX;
            //GameUser.Instance.CurPlayer.Y = curY;
            //GameUser.Instance.NetworkChannel.Send(packet);
        }
    }
}
