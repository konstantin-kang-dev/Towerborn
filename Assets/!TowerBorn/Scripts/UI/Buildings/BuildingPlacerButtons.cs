using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacerButtons : MonoBehaviour
{
    [SerializeField] Button button;

    bool isActive = false;

    void Start()
    {
        button.onClick.AddListener(() =>
        {
            BuildingPlacer.Instance.EndMovingBuilding();
        });
    }

    void Update()
    {
        if (!isActive) return;

        SetPosition(BuildingPlacer.Instance.MovingBuilding.transform.position);
    }

    public void SetPosition(Vector3 position)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(position);
        transform.position = pos;
    }

    public void SetVisibility(bool visible)
    {
        gameObject.SetActive(visible);
        isActive = visible;
    }
}