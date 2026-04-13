using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CloudGenerator : MonoBehaviour
{
    [Header("Настройки облаков")]
    public GameObject cloudPrefab;
    public int cloudCount = 10;
    public Vector2 scaleRangeX = new Vector2(5, 15);
    public Vector2 scaleRangeZ = new Vector2(5, 15);

    [Header("Границы облаков")]
    public BoxCollider boundaryBox;

    [Header("Настройки движения")]
    public bool enableMovement = true;
    public Vector3 cloudSpeed = new Vector3(1f, 0f, 0f);
    public float speedVariation = 0.1f;

    private List<Transform> clouds = new List<Transform>();
    private List<Vector3> velocities = new List<Vector3>();
    private Bounds bounds;

    void Start()
    {
        if (boundaryBox == null)
        {
            boundaryBox = GetComponent<BoxCollider>();
            if (boundaryBox == null)
            {
                boundaryBox = gameObject.AddComponent<BoxCollider>();
                boundaryBox.size = new Vector3(200, 100, 200);
                boundaryBox.isTrigger = true;
            }
        }

        UpdateBounds();
        GenerateClouds();
    }

    void OnDisable()
    {
        ClearClouds();
    }

    void UpdateBounds()
    {
        if (boundaryBox != null)
        {
            bounds = new Bounds(
                boundaryBox.transform.position + boundaryBox.center,
                Vector3.Scale(boundaryBox.size, boundaryBox.transform.lossyScale)
            );
        }
    }

    public void GenerateClouds()
    {
        ClearClouds();
        UpdateBounds();

        if (cloudPrefab == null)
        {
            Debug.LogError("Префаб облака не задан!");
            return;
        }

        for (int i = 0; i < cloudCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            Quaternion rotation = Quaternion.Euler(
                Random.Range(-25f, 25f),
                Random.Range(0f, 360f),
                Random.Range(-25f, 25f)
            );

            Vector3 scale = new Vector3(
                Random.Range(scaleRangeX.x, scaleRangeX.y),
                1f,
                Random.Range(scaleRangeZ.x, scaleRangeZ.y)
            );

            GameObject cloud = Instantiate(cloudPrefab, position, rotation, transform);
            cloud.name = "Cloud_" + i;
            cloud.transform.localScale = scale;

            clouds.Add(cloud.transform);

            float variation = Random.Range(1f - speedVariation, 1f + speedVariation);
            velocities.Add(cloudSpeed * variation);
        }
    }

    void Update()
    {
        if (Application.isPlaying && enableMovement)
        {
            for (int i = 0; i < clouds.Count; i++)
            {
                if (clouds[i] != null)
                {
                    clouds[i].position += velocities[i] * Time.deltaTime;

                    Vector3 pos = clouds[i].position;

                    if (velocities[i].x > 0 && pos.x > bounds.max.x)
                        pos.x = bounds.min.x;
                    else if (velocities[i].x < 0 && pos.x < bounds.min.x)
                        pos.x = bounds.max.x;

                    if (velocities[i].y > 0 && pos.y > bounds.max.y)
                        pos.y = bounds.min.y;
                    else if (velocities[i].y < 0 && pos.y < bounds.min.y)
                        pos.y = bounds.max.y;

                    if (velocities[i].z > 0 && pos.z > bounds.max.z)
                        pos.z = bounds.min.z;
                    else if (velocities[i].z < 0 && pos.z < bounds.min.z)
                        pos.z = bounds.max.z;

                    clouds[i].position = pos;
                }
            }
        }
    }

    public void ClearClouds()
    {
        foreach (var cloud in clouds)
        {
            if (cloud != null)
            {
                if (Application.isPlaying)
                    Destroy(cloud.gameObject);
                else
                    DestroyImmediate(cloud.gameObject);
            }
        }

        clouds.Clear();
        velocities.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Cloud_"))
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}