using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class RFX4_TrailRenderer : MonoBehaviour
{
    public float VertexLifeTime = 2;
    public float TrailLifeTime = 2;

    [Range(0.001f, 1)]
    public float MinVertexDistance = 0.01f;
    public float Gravity = 0.01f;
    public Vector3 Force = new Vector3(0, 0, 0);
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
    public bool SmoothCurves = false;

    private LineRenderer lineRenderer;
    private List<Vector3> positions;
    private List<float> currentTimes;
    private List<Vector3> velocities;

    [HideInInspector]
    public float currentLifeTime;
    private Transform t;
    private Vector3 prevPosition;
    private Vector3 startPosition;
    private List<Vector3> controlPoints = new List<Vector3>();
    private int curveCount;
    private const float MinimumSqrDistance = 0.01f;
    private const float DivisionThreshold = -0.99f;
    private const float SmoothCurvesScale = 0.5f;

    private float currentVelocity;
    private float turbulenceRandomOffset;
    private bool isInitialized;


    void Start()
    {
        Init();
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (isInitialized) Init();
    }

    private void Init()
    {
        positions = new List<Vector3>();
        currentTimes = new List<float>();
        velocities = new List<Vector3>();
        currentLifeTime = 0;
        curveCount = 0;
        currentVelocity = 0;

        t = transform;
        prevPosition = t.position;
        startPosition = t.position;
        lineRenderer = GetComponent<LineRenderer>();
#if !UNITY_5_6_OR_NEWER
        lineRenderer.SetVertexCount(0);
        lineRenderer.SetColors(Color.white, Color.white);
#else
       
        lineRenderer.positionCount = 0;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
#endif
        positions.Add(t.position);
        currentTimes.Add(currentLifeTime);
        velocities.Add(Vector3.zero);
        turbulenceRandomOffset = RandomTurbulenceOffset ? Random.Range(0, 10000f) / 1000f : 0;
    }

    private void Update()
    {
        UpdatePositionsCount();

        UpdateForce();
        UpdateImpulse();
        UpdateVelocity();
        var lastDeletedIndex = GetLastDeletedIndex();
        
        RemovePositionsBeforeIndex(lastDeletedIndex);
        if (SmoothCurves && positions.Count > 2)
        {
            InterpolateBezier(positions, SmoothCurvesScale);
            var bezierPositions = GetDrawingPoints();
#if !UNITY_5_6_OR_NEWER
            lineRenderer.SetVertexCount(bezierPositions.Count);
#else
            lineRenderer.positionCount  = bezierPositions.Count;
#endif
            lineRenderer.SetPositions(bezierPositions.ToArray());
        }
        else
        {
#if !UNITY_5_6_OR_NEWER
            lineRenderer.SetVertexCount(positions.Count);
#else
            lineRenderer.positionCount = positions.Count;
#endif
            lineRenderer.SetPositions(positions.ToArray());
        }
    }

    private int GetLastDeletedIndex()
    {
        int lastDeletedIndex = -1;
        var count = currentTimes.Count;
       
        for (int i = 1; i < count; i++)
        {
            currentTimes[i] -= Time.deltaTime;
            if (currentTimes[i] <= 0)
                lastDeletedIndex = i;
        }
        return lastDeletedIndex;
    }

    private void UpdatePositionsCount()
    {
        if (TrailLifeTime > 0.0001f && currentLifeTime > TrailLifeTime)
            return;
        
        currentLifeTime += Time.deltaTime;

        var lastPosition = positions.Count != 0 ? positions[positions.Count - 1] : Vector3.zero;

        if (Mathf.Abs((t.position - lastPosition).magnitude) > MinVertexDistance && positions.Count > 0)
        {
            AddInterpolatedPositions(lastPosition, t.position);
        }
    }

    void AddInterpolatedPositions(Vector3 start, Vector3 end)
    {
        var distance = (start - end).magnitude;
        var count = (int)(distance / MinVertexDistance);
        var previousTime = currentTimes.LastOrDefault();
        
        
        var vectorZero = Vector3.zero;
        for (int i = 1; i <= count-1; i++) {
            var interpolatedPoint = start + (end - start) * i * 1.0f / count;
            var interpolatedTime = previousTime + (VertexLifeTime - previousTime) * i * 1.0f / count;
           
            positions.Add(interpolatedPoint);
            currentTimes.Add(interpolatedTime);
            velocities.Add(vectorZero);
        }
    }

    private void RemovePositionsBeforeIndex(int lastDeletedIndex)
    {
        if (lastDeletedIndex == -1)
            return;
        var newSize = positions.Count - lastDeletedIndex;

        if (newSize == 1)
        {
            positions.Clear();
            currentTimes.Clear();
            velocities.Clear();
            return;
        }

        positions.RemoveRange(0, lastDeletedIndex);
        currentTimes.RemoveRange(0, lastDeletedIndex);
        velocities.RemoveRange(0, lastDeletedIndex);
    }

    private void UpdateForce()
    {
        if (positions.Count < 1)
            return;
        var gravity = Gravity * Vector3.down * Time.deltaTime;
        var force = t.rotation * Force * Time.deltaTime;

        for (int i = 0; i < positions.Count; i++) {

            var turbulenceVel = Vector3.zero;
            if (TurbulenceStrength > 0.000001f) {
                var pos = positions[i] / Frequency;
                var speed = (Time.time + turbulenceRandomOffset) * OffsetSpeed;
                pos -= speed * Vector3.one;

                turbulenceVel.x += ((Mathf.PerlinNoise(pos.z, pos.y) * 2 - 1) * Amplitude) * Time.deltaTime * TurbulenceStrength / 10f;
                turbulenceVel.y += ((Mathf.PerlinNoise(pos.x, pos.z) * 2 - 1) * Amplitude) * Time.deltaTime * TurbulenceStrength / 10f;
                turbulenceVel.z += ((Mathf.PerlinNoise(pos.y, pos.x) * 2 - 1) * Amplitude) * Time.deltaTime * TurbulenceStrength / 10f;
            }
            var currentForce = (gravity + force + turbulenceVel);
            if (AproximatedFlyDistance > 0.01f)
            {
                var distance = Mathf.Abs((positions[i] - startPosition).magnitude);
                currentForce *= VelocityByDistance.Evaluate(Mathf.Clamp01(distance / AproximatedFlyDistance));
            }
            velocities[i] += currentForce;
        }
    }

    private void UpdateImpulse()
    {
        if (velocities.Count == 0)
            return;

        currentVelocity = ((t.position - prevPosition).magnitude) / Time.deltaTime;
        var directionVelocity = (t.position - prevPosition).normalized;
        prevPosition = t.position;
        velocities[velocities.Count - 1] += currentVelocity * InheritVelocity * directionVelocity * Time.deltaTime;

    }

    private void UpdateVelocity()
    {
        if (velocities.Count==0)
            return;

        var count = positions.Count;
        for (int i = 0; i < count; i++) {
            if (Drag > 0.00001f)
                velocities[i] -= Drag * velocities[i] * Time.deltaTime;
            if (velocities[i].magnitude < 0.00001f)
                velocities[i] = Vector3.zero;
            positions[i] += velocities[i] * Time.deltaTime;
        }
    }

#region Bezier

    public void InterpolateBezier(List<Vector3> segmentPoints, float scale)
    {
        controlPoints.Clear();

        if (segmentPoints.Count < 2)
            return;

        for (int i = 0; i < segmentPoints.Count; i++)
        {
            if (i == 0) // is first
            {
                Vector3 p1 = segmentPoints[i];
                Vector3 p2 = segmentPoints[i + 1];

                Vector3 tangent = (p2 - p1);
                Vector3 q1 = p1 + scale * tangent;

                controlPoints.Add(p1);
                controlPoints.Add(q1);
            }
            else if (i == segmentPoints.Count - 1) //last
            {
                Vector3 p0 = segmentPoints[i - 1];
                Vector3 p1 = segmentPoints[i];
                Vector3 tangent = (p1 - p0);
                Vector3 q0 = p1 - scale * tangent;

                controlPoints.Add(q0);
                controlPoints.Add(p1);
            }
            else
            {
                Vector3 p0 = segmentPoints[i - 1];
                Vector3 p1 = segmentPoints[i];
                Vector3 p2 = segmentPoints[i + 1];
                Vector3 tangent = (p2 - p0).normalized;
                Vector3 q0 = p1 - scale * tangent * (p1 - p0).magnitude;
                Vector3 q1 = p1 + scale * tangent * (p2 - p1).magnitude;

                controlPoints.Add(q0);
                controlPoints.Add(p1);
                controlPoints.Add(q1);
            }
        }

        curveCount = (controlPoints.Count - 1) / 3;
    }

    public List<Vector3> GetDrawingPoints()
    {
        List<Vector3> drawingPoints = new List<Vector3>();

        for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
        {
            List<Vector3> bezierCurveDrawingPoints = FindDrawingPoints(curveIndex);

            if (curveIndex != 0)
                //remove the fist point, as it coincides with the last point of the previous Bezier curve.
                bezierCurveDrawingPoints.RemoveAt(0);

            drawingPoints.AddRange(bezierCurveDrawingPoints);
        }

        return drawingPoints;
    }

    private List<Vector3> FindDrawingPoints(int curveIndex)
    {
        List<Vector3> pointList = new List<Vector3>();

        Vector3 left = CalculateBezierPoint(curveIndex, 0);
        Vector3 right = CalculateBezierPoint(curveIndex, 1);

        pointList.Add(left);
        pointList.Add(right);

        FindDrawingPoints(curveIndex, 0, 1, pointList, 1);

        return pointList;
    }

    private int FindDrawingPoints(int curveIndex, float t0, float t1,
        List<Vector3> pointList, int insertionIndex)
    {
        Vector3 left = CalculateBezierPoint(curveIndex, t0);
        Vector3 right = CalculateBezierPoint(curveIndex, t1);

        if ((left - right).sqrMagnitude < MinimumSqrDistance)
            return 0;

        float tMid = (t0 + t1) / 2;
        Vector3 mid = CalculateBezierPoint(curveIndex, tMid);

        Vector3 leftDirection = (left - mid).normalized;
        Vector3 rightDirection = (right - mid).normalized;

        if (Vector3.Dot(leftDirection, rightDirection) > DivisionThreshold || Mathf.Abs(tMid - 0.5f) < 0.0001f)
        {
            int pointsAddedCount = 0;

            pointsAddedCount += FindDrawingPoints(curveIndex, t0, tMid, pointList, insertionIndex);
            pointList.Insert(insertionIndex + pointsAddedCount, mid);
            pointsAddedCount++;
            pointsAddedCount += FindDrawingPoints(curveIndex, tMid, t1, pointList, insertionIndex + pointsAddedCount);

            return pointsAddedCount;
        }

        return 0;
    }

    public Vector3 CalculateBezierPoint(int curveIndex, float t)
    {
        int nodeIndex = curveIndex * 3;
        Vector3 p0 = controlPoints[nodeIndex];
        Vector3 p1 = controlPoints[nodeIndex + 1];
        Vector3 p2 = controlPoints[nodeIndex + 2];
        Vector3 p3 = controlPoints[nodeIndex + 3];

        return CalculateBezierPoint(t, p0, p1, p2, p3);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; //first term

        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;
    }

#endregion
}