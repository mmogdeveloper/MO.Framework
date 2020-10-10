using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class RFX4_Turbulence : MonoBehaviour
{
    public float TurbulenceStrenght = 1;
    public bool TurbulenceByTime;
    public float TimeDelay = 0;
    public AnimationCurve TurbulenceStrengthByTime = AnimationCurve.EaseInOut(1, 1, 1, 1);
    public Vector3 Frequency = new Vector3(1, 1, 1);
    public Vector3 OffsetSpeed = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 Amplitude = new Vector3(5, 5, 5);
    public Vector3 GlobalForce;
    public bool UseGlobalOffset = true;
    public MoveMethodEnum MoveMethod;
    public PerfomanceEnum Perfomance = PerfomanceEnum.High;
    public float ThreshholdSpeed = 0;
    public AnimationCurve VelocityByDistance = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public float AproximatedFlyDistance = -1;

    public enum MoveMethodEnum
    {
        Position,
        Velocity,
        // RelativePositionHalf,
        RelativePosition
    }

    public enum PerfomanceEnum
    {
        High,
        Low
    }

    private float lastStopTime;
    private Vector3 currentOffset;
    private float deltaTime;
    private float deltaTimeLastUpdateOffset;
    private ParticleSystem.Particle[] particleArray;
    private ParticleSystem particleSys;
    private float time;
    private int currentSplit;
    private float fpsTime;
    private int FPS;
    private int splitUpdate = 2;
    private PerfomanceEnum perfomanceOldSettings;
    private bool skipFrame;
    private Transform t;
    private float currentDelay;
    //private bool isInitilised;
    //private bool canUpdateEval;

    private void Start()
    {
        t = transform;
        particleSys = GetComponent<ParticleSystem>();
#if !UNITY_5_5_OR_NEWER
        if (particleArray==null || particleArray.Length < particleSys.maxParticles)
            particleArray = new ParticleSystem.Particle[particleSys.maxParticles];
#else
        if (particleArray == null || particleArray.Length < particleSys.main.maxParticles)
            particleArray = new ParticleSystem.Particle[particleSys.main.maxParticles];
#endif

        perfomanceOldSettings = Perfomance;
        UpdatePerfomanceSettings();
    }

    void OnEnable()
    {
        currentDelay = 0;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            deltaTime = Time.realtimeSinceStartup - lastStopTime;
            lastStopTime = Time.realtimeSinceStartup;
        }
        else
            deltaTime = Time.deltaTime;
        currentDelay += deltaTime;
        if (currentDelay < TimeDelay) return;
        if (!UseGlobalOffset)
            currentOffset += OffsetSpeed * deltaTime;
        else
        {
            if (Application.isPlaying) currentOffset = OffsetSpeed * Time.time;
            else currentOffset = OffsetSpeed * Time.realtimeSinceStartup;
        }
        if (Perfomance != perfomanceOldSettings)
        {
            perfomanceOldSettings = Perfomance;
            UpdatePerfomanceSettings();
        }
        time += deltaTime;
        
        if (QualitySettings.vSyncCount == 2)
            UpdateTurbulence();
        else if (QualitySettings.vSyncCount == 1)
        {
            if (Perfomance == PerfomanceEnum.Low)
            {
                if (skipFrame)
                    UpdateTurbulence();
                skipFrame = !skipFrame;
            }
            if (Perfomance == PerfomanceEnum.High)
                UpdateTurbulence();
        }
        else
        {
            if (QualitySettings.vSyncCount == 0)
            {
                if (time >= fpsTime)
                {
                    time = 0;
                    UpdateTurbulence();
                    deltaTimeLastUpdateOffset = 0;
                }
                else
                    deltaTimeLastUpdateOffset += deltaTime;
            }
        }

    }

    private void UpdatePerfomanceSettings()
    {
       if (Perfomance == PerfomanceEnum.High)
        {
            FPS = 80;
            splitUpdate = 2;
        }
        if (Perfomance == PerfomanceEnum.Low)
        {
            FPS = 40;
            splitUpdate = 2;
        }
        fpsTime = 1.0f / FPS;
    }

    private void UpdateTurbulence()
    {
        int start;
        int end;
        var numParticlesAlive = particleSys.GetParticles(particleArray);
        var turbulenceStrenghtMultiplier = 1;
        if (splitUpdate > 1)
        {
            start = (numParticlesAlive / splitUpdate) * currentSplit;
            end =  Mathf.CeilToInt((numParticlesAlive * 1.0f / splitUpdate) * (currentSplit + 1.0f));
            turbulenceStrenghtMultiplier = splitUpdate;
        }
        else
        {
            start = 0;
            end = numParticlesAlive;
    }
    for (int i = start; i < end; i++)
        {
            var particle = particleArray[i];
            float timeTurbulenceStrength = 1;
#if !UNITY_5_5_OR_NEWER
            if (TurbulenceByTime)
                timeTurbulenceStrength = TurbulenceStrengthByTime.Evaluate(1 - particle.lifetime / particle.startLifetime);
#else
             if (TurbulenceByTime)
                timeTurbulenceStrength = TurbulenceStrengthByTime.Evaluate(1 - particle.remainingLifetime / particle.startLifetime);
#endif
            if (ThreshholdSpeed > 0.0000001f && timeTurbulenceStrength < ThreshholdSpeed) return;
            var pos = particle.position;
            pos.x /= (Frequency.x + 0.0000001f);
            pos.y /= (Frequency.y + 0.0000001f);
            pos.z /= (Frequency.z + 0.0000001f);
            var turbulenceVector = new Vector3();
            var timeOffset = deltaTime + deltaTimeLastUpdateOffset;
            turbulenceVector.x = ((Mathf.PerlinNoise(pos.z - currentOffset.z, pos.y - currentOffset.y) * 2 - 1) * Amplitude.x) * timeOffset;
            turbulenceVector.y = ((Mathf.PerlinNoise(pos.x - currentOffset.x, pos.z - currentOffset.z) * 2 - 1) * Amplitude.y) * timeOffset;
            turbulenceVector.z = ((Mathf.PerlinNoise(pos.y - currentOffset.y, pos.x - currentOffset.x) * 2 - 1) * Amplitude.z) * timeOffset;
            var lerpedTurbulence = TurbulenceStrenght * timeTurbulenceStrength * turbulenceStrenghtMultiplier;

            float velocityByDistanceMultiplier = 1;
            var distance = Mathf.Abs((particle.position - t.position).magnitude);
            if (AproximatedFlyDistance > 0)
                velocityByDistanceMultiplier = VelocityByDistance.Evaluate(Mathf.Clamp01(distance / AproximatedFlyDistance));

            turbulenceVector *= lerpedTurbulence;
            if (MoveMethod == MoveMethodEnum.Position)
                particleArray[i].position += turbulenceVector * velocityByDistanceMultiplier;
            if (MoveMethod == MoveMethodEnum.Velocity)
                particleArray[i].velocity += turbulenceVector * velocityByDistanceMultiplier;
            if (MoveMethod == MoveMethodEnum.RelativePosition)
            {
                particleArray[i].position += turbulenceVector * particleArray[i].velocity.magnitude;
                particleArray[i].velocity = particleArray[i].velocity * 0.85f + turbulenceVector.normalized * 0.15f * velocityByDistanceMultiplier + GlobalForce * velocityByDistanceMultiplier;
            }
        }
        particleSys.SetParticles(particleArray, numParticlesAlive);

        currentSplit++;
        if (currentSplit >= splitUpdate)
            currentSplit = 0;


    }
}
