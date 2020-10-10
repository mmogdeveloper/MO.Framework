using System;
using UnityEngine;

public static class RFX4_ColorHelper
{
    const float TOLERANCE = 0.0001f;
    static string[] colorProperties = { "_TintColor" , "_Color", "_EmissionColor", "_BorderColor", "_ReflectColor", "_RimColor", "_MainColor", "_CoreColor"};

    public struct HSBColor
    {
        public float H;
        public float S;
        public float B;
        public float A;

        public HSBColor(float h, float s, float b, float a)
        {
            this.H = h;
            this.S = s;
            this.B = b;
            this.A = a;
        }
    }

    public static HSBColor ColorToHSV(Color color)
    {
        HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);

        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, Mathf.Max(g, b));

        if (max <= 0)
            return ret;

        float min = Mathf.Min(r, Mathf.Min(g, b));
        float dif = max - min;

        if (max > min)
        {
            if (Math.Abs(g - max) < TOLERANCE)
                ret.H = (b - r)/dif*60f + 120f;
            else if (Math.Abs(b - max) < TOLERANCE)
                ret.H = (r - g)/dif*60f + 240f;
            else if (b > g)
                ret.H = (g - b)/dif*60f + 360f;
            else
                ret.H = (g - b)/dif*60f;
            if (ret.H < 0)
                ret.H = ret.H + 360f;
        }
        else
            ret.H = 0;

        ret.H *= 1f/360f;
        ret.S = (dif/max)*1f;
        ret.B = max;

        return ret;
    }

    public static Color HSVToColor(HSBColor hsbColor)
    {
        float r = hsbColor.B;
        float g = hsbColor.B;
        float b = hsbColor.B;
        if (Math.Abs(hsbColor.S) > TOLERANCE)
        {
            float max = hsbColor.B;
            float dif = hsbColor.B*hsbColor.S;
            float min = hsbColor.B - dif;

            float h = hsbColor.H*360f;

            if (h < 60f)
            {
                r = max;
                g = h*dif/60f + min;
                b = min;
            }
            else if (h < 120f)
            {
                r = -(h - 120f)*dif/60f + min;
                g = max;
                b = min;
            }
            else if (h < 180f)
            {
                r = min;
                g = max;
                b = (h - 120f)*dif/60f + min;
            }
            else if (h < 240f)
            {
                r = min;
                g = -(h - 240f)*dif/60f + min;
                b = max;
            }
            else if (h < 300f)
            {
                r = (h - 240f)*dif/60f + min;
                g = min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = -(h - 360f)*dif/60 + min;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }
        }

        return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.A);
    }

    public static Color ConvertRGBColorByHUE(Color rgbColor, float hue)
    {
        var brightness = ColorToHSV(rgbColor).B;
        if (brightness < TOLERANCE)
            brightness = TOLERANCE;
        var hsv = ColorToHSV(rgbColor / brightness);
        hsv.H = hue;
        var color = HSVToColor(hsv) * brightness;
        color.a = rgbColor.a;
        return color;
    }

    public static void ChangeObjectColorByHUE(GameObject go, float hue)
    {
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var rend in renderers)
        {
            var mat = rend.material;
            if (mat == null)
                continue;
            foreach (var colorProperty in colorProperties)
            {
                if (mat.HasProperty(colorProperty))
                {
                    setMatHUEColor(mat, colorProperty, hue);
                }
            }
        }

        var skinRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var skinRend in skinRenderers)
        {
            var mat = skinRend.material;
            if (mat == null)
                continue;
            foreach (var colorProperty in colorProperties)
            {
                if (mat.HasProperty(colorProperty))
                {
                    setMatHUEColor(mat, colorProperty, hue);
                }
            }
        }

        var projectors = go.GetComponentsInChildren<Projector>(true);
        foreach (var proj in projectors)
        {
            if (!proj.material.name.EndsWith("(Instance)"))
                proj.material = new Material(proj.material) { name = proj.material.name + " (Instance)" };
            var mat = proj.material;
            if (mat == null )
                continue;
            foreach (var colorProperty in colorProperties)
            {
                if (mat.HasProperty(colorProperty))
                {
                    proj.material = setMatHUEColor(mat, colorProperty, hue);
                }
            }
        }

        var lights = go.GetComponentsInChildren<Light>(true);
        foreach (var light in lights)
        {
            var hsv = ColorToHSV(light.color);
            hsv.H = hue;
            light.color = HSVToColor(hsv);
        }

        var particles = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in particles)
        {
#if !UNITY_5_5_OR_NEWER
            var hsv = ColorToHSV(ps.startColor);
            hsv.H = hue;
            ps.startColor = HSVToColor(hsv);
#else
            var main = ps.main;
            var hsv = ColorToHSV(ps.main.startColor.color);
            hsv.H = hue;
            main.startColor = HSVToColor(hsv);
#endif
        }

        var rfx4_trails = go.GetComponentsInChildren<RFX4_ParticleTrail>(true);
        foreach (var rfx4_trail in rfx4_trails)
        {
            var mat = rfx4_trail.TrailMaterial;
            if (mat == null)
                continue;
            mat = new Material(rfx4_trail.TrailMaterial);
            rfx4_trail.TrailMaterial = mat;

            foreach (var colorProperty in colorProperties)
            {
                if (mat.HasProperty(colorProperty))
                {
                    setMatHUEColor(mat, colorProperty, hue);
                }
            }
        }

        var rfx4_shaderColorGradients = go.GetComponentsInChildren<RFX4_ShaderColorGradient>(true);
        
        foreach (var rfx4_shaderColorGradient in rfx4_shaderColorGradients)
        {
            rfx4_shaderColorGradient.HUE = hue;
        }

        
    }

    static Material setMatHUEColor(Material mat, String name, float hueColor)
    {
        var oldColor = mat.GetColor(name);
        var color = ConvertRGBColorByHUE(oldColor, hueColor);
        mat.SetColor(name, color);
        return mat;
    }

    static Material setMatAlphaColor(Material mat, String name, float alpha)
    {
        var oldColor = mat.GetColor(name);
        oldColor.a = alpha;
        mat.SetColor(name, oldColor);
        return mat;
    }
}
