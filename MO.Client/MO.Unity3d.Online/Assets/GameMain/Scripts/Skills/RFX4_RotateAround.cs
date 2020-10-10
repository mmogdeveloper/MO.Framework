using UnityEngine;
using System.Collections;

public class RFX4_RotateAround : MonoBehaviour
{
    public Vector3 Offset = Vector3.forward;
    public Vector3 RotateVector = Vector3.forward;
    public float LifeTime = 1;

    private Transform t;
    private float currentTime;
    private Quaternion rotation;

    // Use this for initialization
    private void Start()
    {
        t = transform;
        rotation = t.rotation;
    }

    private void OnEnable()
    {
        currentTime = 0;
        if(t!=null) t.rotation = rotation;
    }

    private void Update()
    {
        if (currentTime >= LifeTime && LifeTime > 0.0001f)
            return;
        currentTime += Time.deltaTime;
        t.Rotate(RotateVector * Time.deltaTime);
    }
}