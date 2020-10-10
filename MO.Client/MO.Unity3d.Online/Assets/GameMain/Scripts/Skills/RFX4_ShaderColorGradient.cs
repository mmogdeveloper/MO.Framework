using UnityEngine;
using System.Collections;

public class RFX4_ShaderColorGradient : MonoBehaviour {

    public RFX4_ShaderProperties ShaderColorProperty = RFX4_ShaderProperties._TintColor;
    public Gradient Color = new Gradient();
    public float TimeMultiplier = 1;
    public bool IsLoop;
    public bool UseSharedMaterial;
    [HideInInspector] public float HUE = -1;

    [HideInInspector]
     public bool canUpdate;
    private Material mat;
    private int propertyID;
    private float startTime;
    private Color startColor;
  
    private bool isInitialized;
    private string shaderProperty;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        shaderProperty = ShaderColorProperty.ToString();
        startTime = Time.time;
        canUpdate = true;
        var rend = GetComponent<Renderer>();
        if (rend==null) {
            var projector = GetComponent<Projector>();
            if (projector!=null) {
                if (!projector.material.name.EndsWith("(Instance)"))
                    projector.material = new Material(projector.material) {name = projector.material.name + " (Instance)"};
                mat = projector.material;
            }
        }
        else
        {
            if (!UseSharedMaterial) mat = rend.material;
            else mat = rend.sharedMaterial;
        }

        if (mat == null)
        {
            canUpdate = false;
            return;
        }

        if (!mat.HasProperty(shaderProperty))
        {
            canUpdate = false;
            return;
        }
        if (mat.HasProperty(shaderProperty))
            propertyID = Shader.PropertyToID(shaderProperty);

        startColor = mat.GetColor(propertyID);
        var eval = Color.Evaluate(0);
        mat.SetColor(propertyID, eval * startColor);
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (!isInitialized) return;
        startTime = Time.time;
        canUpdate = true;
      
    }

    private void Update()
    {
        if (mat == null) return;
        var time = Time.time - startTime;
        if (canUpdate)
        {
            var eval = Color.Evaluate(time / TimeMultiplier);
            if (HUE > -0.9f)
            {
                eval = RFX4_ColorHelper.ConvertRGBColorByHUE(eval, HUE);
                startColor = RFX4_ColorHelper.ConvertRGBColorByHUE(startColor, HUE);
            }
            mat.SetColor(propertyID, eval * startColor);
        }
        if (time >= TimeMultiplier) {
            if (IsLoop) startTime = Time.time;
            else canUpdate = false;
        }
    }

    void OnDisable()
    {
        if (mat == null) return;
        if (UseSharedMaterial) mat.SetColor(propertyID, startColor);
        mat.SetColor(propertyID, startColor);
    }

    //void OnDestroy()
    //{
    //    if (!UseSharedMaterial)
    //    {
    //        if (mat != null)
    //            DestroyImmediate(mat);
    //        mat = null;
    //    }
    //}
}
