using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    [SerializeField] float smoothTime = 0.2f;
    [SerializeField] float zoomSpeed = 0.01f;
    [SerializeField] MeshRenderer boundsRenderer;

    public float inertiaDamping = 0.9f;
    public float elasticity = 0.75f;
    public float elasticDamping = 0.85f;

    public Vector3 velocity = Vector3.zero;
    public Vector3 boundaries;


    float elasticDistance = 1.5f;

    private Vector3 lastTargetPos;
    public Vector3 targetPos;

    public float defaultZoom = 10f;
    public float currentZoom = 0f;

    Camera cam;
    bool isDragging = false;

    void Awake()
    {
        Instance = this;
        targetPos = transform.position;
        cam = GetComponentInChildren<Camera>();
        boundaries = boundsRenderer.bounds.extents;


        currentZoom = defaultZoom;
    }

    void Update()
    {

        ApplyIntertia();
        ApplyElasticBoundaries();

        transform.position = Vector3.Lerp(transform.position, targetPos, 15f * Time.unscaledDeltaTime);

#if UNITY_EDITOR
        //float zoomValue = Input.GetAxis("Mouse ScrollWheel") * 500f;
        //SetZoomValue(-zoomValue);
#endif
    }
    void ApplyIntertia()
    {
        if (!isDragging)
        {
            targetPos -= velocity * Time.unscaledDeltaTime;
            velocity *= inertiaDamping;
            if (velocity.magnitude < 0.01f) velocity = Vector3.zero;
        }
    }
    void ApplyElasticBoundaries()
    {
        Vector3 force = Vector3.zero;

        Vector3 boundsCenter = boundsRenderer.bounds.center;

        if (targetPos.x < boundsCenter.x - boundaries.x - elasticDistance)
            force.x = (boundsCenter.x - boundaries.x - elasticDistance - targetPos.x) * elasticity;
        else if (targetPos.x > boundsCenter.x + boundaries.x + elasticDistance)
            force.x = (boundsCenter.x + boundaries.x + elasticDistance - targetPos.x) * elasticity;

        if (targetPos.z < boundsCenter.z - boundaries.z - elasticDistance)
            force.z = (boundsCenter.z - boundaries.z - elasticDistance - targetPos.z) * elasticity;
        else if (targetPos.z > boundsCenter.z + boundaries.z + elasticDistance)
            force.z = (boundsCenter.z + boundaries.z + elasticDistance - targetPos.z) * elasticity;

        if (force.magnitude > 0)
        {
            velocity -= force;
            velocity *= elasticDamping;
        }
    }
    public void SetTargetPos(Vector2 touchDist)
    {
        //Debug.Log($"MoveCam touchDist: {touchDist}");
        isDragging = true;

        float zoomSens = defaultZoom / currentZoom;
        float xPos = (touchDist.x / 190f / zoomSens);
        float zPos = (touchDist.y / 100f / zoomSens);

        Vector3 moveDirection = transform.right * xPos + transform.forward * zPos;

        moveDirection.y = 0;
        targetPos -= moveDirection;

        velocity = moveDirection / Time.unscaledDeltaTime;
    }

    public void StopDragging()
    {
        isDragging = false;
    }

    public void SetZoomValue(float zoom)
    {
        if (zoom == 0) return;
        currentZoom = Mathf.Clamp(cam.orthographicSize + zoom * zoomSpeed * 0.2f, 3f, 15f);
        cam.orthographicSize = currentZoom;
    }
}
