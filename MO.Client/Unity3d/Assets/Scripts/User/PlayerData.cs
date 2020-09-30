using System;
using UnityEngine;

/// <summary>
/// 玩家信息
/// </summary>
public class PlayerData
{
    public GameObject GameObject { get; }
    public PlayerData(GameObject gameObject = null)
    {
        if (gameObject == null)
        {
            gameObject = (GameObject)Resources.Load("Player");
        }
        GameObject prefabInstance = GameObject.Instantiate(gameObject);
        prefabInstance.transform.SetParent(GameObject.Find("Canvas").gameObject.transform);
        GameObject = gameObject;
    }

    public Int64 UserId { get; set; }
    public string NickName { get; set; }
    public string HeadIcon { get; set; }
    public bool IsLoad { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
}