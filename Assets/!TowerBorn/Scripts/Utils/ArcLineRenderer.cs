using UnityEngine;

public class ArcLineRenderer : MonoBehaviour
{
    [Header("Arc Settings")]
    [SerializeField] private float arcHeight = 3f;
    [SerializeField] private int minSegments = 10;
    [SerializeField] private int maxSegments = 50;
    [SerializeField] private float segmentsPerUnit = 5f;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void CreateArc(Vector3 startPoint, Vector3 endPoint)
    {
        CreateArc(startPoint, endPoint, arcHeight);
    }

    public void CreateArc(Vector3 startPoint, Vector3 endPoint, float height)
    {
        lineRenderer.enabled = true;
        int segmentCount = CalculateSegmentCount(startPoint, endPoint);

        lineRenderer.positionCount = segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 point = CalculateParabolicPoint(startPoint, endPoint, height, t);
            lineRenderer.SetPosition(i, point);
        }
    }

    private int CalculateSegmentCount(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        int calculatedSegments = Mathf.RoundToInt(distance * segmentsPerUnit);
        return Mathf.Clamp(calculatedSegments, minSegments, maxSegments);
    }

    private Vector3 CalculateParabolicPoint(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 linearPoint = Vector3.Lerp(start, end, t);
        float parabolicHeight = height * 4 * t * (1 - t);
        return linearPoint + Vector3.up * parabolicHeight;
    }

    public void HideArc()
    {
        lineRenderer.enabled = false;
    }
}