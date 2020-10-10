using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class RFX4_ParticleLight : MonoBehaviour
{
    public float LightIntencityMultiplayer = 1;
    public LightShadows Shadows = LightShadows.None;

    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    Light[] lights;

    private int lightLimit = 20;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
#if !UNITY_5_5_OR_NEWER
        if(ps.maxParticles > lightLimit) ps.maxParticles = lightLimit;
        particles = new ParticleSystem.Particle[ps.maxParticles];

        lights = new Light[ps.maxParticles];
#else
        var main = ps.main;
        if (main.maxParticles > lightLimit) main.maxParticles = lightLimit;
        particles = new ParticleSystem.Particle[main.maxParticles];

        lights = new Light[main.maxParticles];
#endif
        for (int i = 0; i < lights.Length; i++)
        {
            var lightGO = new GameObject();
            //lightGO.hideFlags = HideFlags.HideAndDontSave;
            lights[i] = lightGO.AddComponent<Light>();
            lights[i].transform.parent = transform;
            lights[i].intensity = 0;
            lights[i].shadows = Shadows;
        }
    }

    void Update()
    {
        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            lights[i].gameObject.SetActive(true);
            lights[i].transform.position = particles[i].position;
            lights[i].color = particles[i].GetCurrentColor(ps);
            lights[i].range = particles[i].GetCurrentSize(ps);
            lights[i].intensity = particles[i].GetCurrentColor(ps).a / 255f * LightIntencityMultiplayer;
        }
        for (int i = count; i < particles.Length; i++)
        {
            lights[i].gameObject.SetActive(false);
        }
    }
}