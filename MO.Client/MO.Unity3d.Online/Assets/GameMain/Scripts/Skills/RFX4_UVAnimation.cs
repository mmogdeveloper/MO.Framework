using UnityEngine;

public class RFX4_UVAnimation : MonoBehaviour
{
    public int TilesX = 4;
    public int TilesY = 4;
    public int FPS = 30;
    public int StartFrameOffset;
    public bool IsLoop = true;
    public float StartDelay = 0;
    public bool IsReverse;
    public bool IsInterpolateFrames;
    public RFX4_TextureShaderProperties[] TextureNames = { RFX4_TextureShaderProperties._MainTex };

    public AnimationCurve FrameOverTime = AnimationCurve.Linear(0, 1, 1, 1);

    private int count;
    private Renderer currentRenderer;
    private Projector projector;
    private Material instanceMaterial;
    private float animationStartTime;
    private bool canUpdate;
    private int previousIndex;
    private int totalFrames;
    private float currentInterpolatedTime;
    private int currentIndex;
    private Vector2 size;
    private bool isInitialized;

    private void OnEnable()
    {
        if (isInitialized) InitDefaultVariables();
    }

    private void Start()
    {
        InitDefaultVariables();
        isInitialized = true;
    }

    void Update()
    {
        if (!canUpdate) return;
        UpdateMaterial();
        SetSpriteAnimation();
        if (IsInterpolateFrames)
            SetSpriteAnimationIterpolated();
    }

    private void InitDefaultVariables()
    {
        InitializeMaterial();

        totalFrames = TilesX * TilesY;
        previousIndex = 0;
        canUpdate = true;
        count = TilesY * TilesX;
        var offset = Vector3.zero;
        StartFrameOffset = StartFrameOffset - (StartFrameOffset / count) * count;
        size = new Vector2(1f / TilesX, 1f / TilesY);
        animationStartTime = Time.time;
        if (instanceMaterial != null)
        {
            foreach (var textureName in TextureNames) {
                instanceMaterial.SetTextureScale(textureName.ToString(), size);
                instanceMaterial.SetTextureOffset(textureName.ToString(), offset);
            }
        }
    }

    private void InitializeMaterial()
    {
        currentRenderer = GetComponent<Renderer>();
        if (currentRenderer == null)
        {
            projector = GetComponent<Projector>();
            if (projector != null)
            {
                if (!projector.material.name.EndsWith("(Instance)"))
                    projector.material = new Material(projector.material) {name = projector.material.name + " (Instance)"};
                instanceMaterial = projector.material;
            }
        }
        else
            instanceMaterial = currentRenderer.material;
    }

    private void UpdateMaterial()
    {
        if (currentRenderer == null)
        {
            if (projector != null)
            {
                if (!projector.material.name.EndsWith("(Instance)"))
                    projector.material = new Material(projector.material) { name = projector.material.name + " (Instance)" };
                instanceMaterial = projector.material;
            }
        }
        else
            instanceMaterial = currentRenderer.material;
    }

    void SetSpriteAnimation()
    {
        int index = (int)((Time.time - animationStartTime) * FPS);
        index = index % totalFrames;

        if (!IsLoop && index < previousIndex)
        {
            canUpdate = false;
            return;
        }

        if (IsInterpolateFrames && index != previousIndex)
        {
            currentInterpolatedTime = 0;
        }
        previousIndex = index;

        if (IsReverse)
            index = totalFrames - index - 1;

        var uIndex = index % TilesX;
        var vIndex = index / TilesX;

        float offsetX = uIndex * size.x;
        float offsetY = (1.0f - size.y) - vIndex * size.y;
        var offset = new Vector2(offsetX, offsetY);

        if (instanceMaterial != null)
        {
            foreach (var textureName in TextureNames)
            {
                instanceMaterial.SetTextureScale(textureName.ToString(), size);
                instanceMaterial.SetTextureOffset(textureName.ToString(), offset);
            }
        }
    }

    private void SetSpriteAnimationIterpolated()
    {
        currentInterpolatedTime += Time.deltaTime;

        var nextIndex = previousIndex + 1;
        if (nextIndex == totalFrames)
            nextIndex = previousIndex;
        if (IsReverse)
            nextIndex = totalFrames - nextIndex - 1;

        var uIndex = nextIndex%TilesX;
        var vIndex = nextIndex/TilesX;

        float offsetX = uIndex*size.x;
        float offsetY = (1.0f - size.y) - vIndex*size.y;
        var offset = new Vector2(offsetX, offsetY);
        if (instanceMaterial != null)
        {
            instanceMaterial.SetVector("_Tex_NextFrame", new Vector4(size.x, size.y, offset.x, offset.y));
            instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(currentInterpolatedTime*FPS));
        }
    }
}