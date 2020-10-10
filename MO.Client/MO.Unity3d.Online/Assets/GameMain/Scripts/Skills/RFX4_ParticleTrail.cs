using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ParticleSystem))]
public class RFX4_ParticleTrail : MonoBehaviour
{
    public GameObject Target;
    public Vector2 DefaultSizeMultiplayer = Vector2.one;
    public float VertexLifeTime = 2;
    public float TrailLifeTime = 2;
    public bool UseShaderMaterial;
    public Material TrailMaterial;
    public bool UseColorOverLifeTime = false;
    public Gradient ColorOverLifeTime = new Gradient();
    public float ColorLifeTime = 1;

    public bool UseUvAnimation = false;
    public int TilesX = 4;
    public int TilesY = 4;
    public int FPS = 30;
    public bool IsLoop = true;

    [Range(0.001f, 1)]
    public float MinVertexDistance = 0.01f;
    public bool GetVelocityFromParticleSystem = false;
    public float Gravity = 0.01f;
    public Vector3 Force = new Vector3(0, 0.01f, 0);
    public float InheritVelocity = 0;
    public float Drag = 0.01f;
     [Range(0.001f, 10)]
    public float Frequency = 1;
     [Range(0.001f, 10)]
    public float OffsetSpeed = 0.5f;
    public bool RandomTurbulenceOffset = false;
     [Range(0.001f, 10)]
    public float Amplitude = 2;
    public float TurbulenceStrength = 0.1f;
    public AnimationCurve VelocityByDistance = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public float AproximatedFlyDistance = -1;
    public bool SmoothCurves = true;

    private Dictionary<int, LineRenderer> dict = new Dictionary<int, LineRenderer>(); 
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    TrailRenderer[] trails;
    private Color psColor;
    private Transform targetT;
    private int layer;
    private bool isLocalSpace = true;
    private Transform t;
    //private bool isInitialized;

    void OnEnable()
    {
        if (Target != null) targetT = Target.transform;
        ps = GetComponent<ParticleSystem>();
       // ps.startRotation3D = new Vector3(100000, 100000, 100000);

        t = transform;
#if !UNITY_5_5_OR_NEWER
        isLocalSpace = ps.simulationSpace==ParticleSystemSimulationSpace.Local;
        particles = new ParticleSystem.Particle[ps.maxParticles];
#else
        isLocalSpace = ps.main.simulationSpace == ParticleSystemSimulationSpace.Local;
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
#endif
        if (TrailMaterial!=null) {
            psColor = TrailMaterial.GetColor(TrailMaterial.HasProperty("_TintColor") ? "_TintColor" : "_Color"); 
        }


        // InvokeRepeating("RemoveEmptyTrails", 1, 1);
        layer = gameObject.layer;
        //isInitialized = true;
        Update();
    }


    void ClearTrails()
    {
        foreach (var trailRenderer in trails) {
            if(trailRenderer!=null) Destroy(trailRenderer.gameObject);
        }
        trails = null;

    }

    //void OnEnable()
    //{
    //    if(isInitialized) Update();
    //}

    private void Update()
    {
        if (dict.Count > 10)
            RemoveEmptyTrails();

        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++) {
            var hash = (particles[i].rotation3D).GetHashCode();
            if (!dict.ContainsKey(hash)) {
                var go = new GameObject(hash.ToString());
                go.transform.parent = transform;
                //go.hideFlags = HideFlags.HideAndDontSave;
                go.transform.position = ps.transform.position;

                if(TrailLifeTime> 0.00001f) Destroy(go, TrailLifeTime + VertexLifeTime);
                go.layer = layer;

                var lineRenderer = go.AddComponent<LineRenderer>();
#if !UNITY_5_5_OR_NEWER
                lineRenderer.SetWidth(0, 0);
#else
                lineRenderer.startWidth = 0;
                lineRenderer.endWidth = 0;
#endif
                lineRenderer.sharedMaterial = TrailMaterial;
                lineRenderer.useWorldSpace = false;

                if (UseColorOverLifeTime) {
                    var shaderColor = go.AddComponent<RFX4_ShaderColorGradient>();
                    shaderColor.Color = ColorOverLifeTime;
                    shaderColor.TimeMultiplier = ColorLifeTime;
                }

                if (UseUvAnimation)
                {
                    var uvAnimation = go.AddComponent<RFX4_UVAnimation>();
                    uvAnimation.TilesX = TilesX;
                    uvAnimation.TilesY = TilesY;
                    uvAnimation.FPS = FPS;
                    uvAnimation.IsLoop = IsLoop;
                }

                dict.Add(hash, lineRenderer);
            }
            else {
                var trail = dict[hash];
                if (trail == null) continue;

               
                if (!trail.useWorldSpace) {
                    trail.useWorldSpace = true; 
                    InitTrailRenderer(trail.gameObject);
                }
                var size = DefaultSizeMultiplayer * particles[i].GetCurrentSize(ps);
               
#if !UNITY_5_5_OR_NEWER
                trail.SetWidth(size.y, size.x);
#else
                trail.startWidth = size.y;
                trail.endWidth = size.x;
#endif
                if (Target!=null) {

#if !UNITY_5_5_OR_NEWER
                    var time = 1 - particles[i].lifetime / particles[i].startLifetime;
#else
                     var time = 1 - particles[i].remainingLifetime / particles[i].startLifetime;
#endif
                    var pos = Vector3.Lerp(particles[i].position, targetT.position, time);
                    trail.transform.position = Vector3.Lerp(pos, targetT.position, Time.deltaTime * time);
                }
                else {
                    trail.transform.position = isLocalSpace ? ps.transform.TransformPoint(particles[i].position) : particles[i].position;
                }
                trail.transform.rotation = t.rotation;
                var particleColor = particles[i].GetCurrentColor(ps);
                var color = psColor * particleColor;
                //if (!UseShaderMaterial) trail.material.SetColor("_TintColor", color); 
               
#if !UNITY_5_5_OR_NEWER
                trail.SetColors(color, color);
#else
                trail.startColor = color;
                trail.endColor = color;
#endif
            }
        }
        ps.SetParticles(particles, count);
       
    }

    private void InitTrailRenderer(GameObject go)
    {
        var trailRenderer = go.AddComponent<RFX4_TrailRenderer>();

        trailRenderer.Amplitude = Amplitude;
        trailRenderer.Drag = Drag;
        trailRenderer.Gravity = Gravity;
        trailRenderer.Force = Force;
        trailRenderer.Frequency = Frequency;
        trailRenderer.InheritVelocity = InheritVelocity;
        trailRenderer.VertexLifeTime = VertexLifeTime;
        trailRenderer.TrailLifeTime = TrailLifeTime;
        trailRenderer.MinVertexDistance = MinVertexDistance;
        trailRenderer.OffsetSpeed = OffsetSpeed;
        trailRenderer.SmoothCurves = SmoothCurves;
        trailRenderer.AproximatedFlyDistance = AproximatedFlyDistance;
        trailRenderer.VelocityByDistance = VelocityByDistance;
        trailRenderer.RandomTurbulenceOffset = RandomTurbulenceOffset;
        trailRenderer.TurbulenceStrength = TurbulenceStrength;
    }

    private void RemoveEmptyTrails()
    {
        for (int i = 0; i < dict.Count; i++) {
            var element = dict.ElementAt(i);
            if (element.Value==null)
                dict.Remove(element.Key);
        }
    }

    void OnDisable()
    {
        foreach (var trailRenderer in dict) {
            if(trailRenderer.Value!=null) Destroy(trailRenderer.Value.gameObject);
        }
        dict.Clear();
    }
}
