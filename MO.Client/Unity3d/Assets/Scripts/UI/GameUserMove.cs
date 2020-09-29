using MO.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class GameUserMove : MonoBehaviour
{
    //定义移动的速度
    public float MoveSpeed = 20f;

    void Start()
    {

    }

    void Update()
    {
        //如果按下W或上方向键
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            MoveForward();
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            MoveBack();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }

    void MoveForward()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + MoveSpeed);
        //this.transform.Translate(Vector3.up * MoveSpeed * Time.deltaTime);
        Upload();
    }
    void MoveBack()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - MoveSpeed);
        //this.transform.Translate(Vector3.down * MoveSpeed * Time.deltaTime);
        Upload();
    }
    void MoveLeft()
    {
        this.transform.position = new Vector3(this.transform.position.x - MoveSpeed, this.transform.position.y);
        //this.transform.Translate(Vector3.left * MoveSpeed * Time.deltaTime);
        Upload();
    }
    void MoveRight()
    {
        this.transform.position = new Vector3(this.transform.position.x + MoveSpeed, this.transform.position.y);
        //this.transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime);
        Upload();
    }
    void Upload()
    {
        if (Time.frameCount % 5 == 0)
        {
            var curX = transform.position.x;
            var curY = transform.position.y;

            if ((int)curX == (int)GameUser.Instance.CurPlayer.X &&
                (int)curY == (int)GameUser.Instance.CurPlayer.Y)
            {
                return;
            }
            Log.Info("{0}({1},{2})", GameUser.Instance.CurPlayer.UserId, curX, curY);
            C2S100003 content = new C2S100003();
            content.X = curX;
            content.Y = curY;
            var packet = PacketHelper.BuildPacket(100003, content);
            GameUser.Instance.CurPlayer.X = curX;
            GameUser.Instance.CurPlayer.Y = curY;
            GameUser.Instance.NetworkChannel.Send(packet);
        }
    }
}