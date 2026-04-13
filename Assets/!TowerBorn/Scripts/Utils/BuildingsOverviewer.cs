using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingsOverviewer : MonoBehaviour
{
    List<Building> buildings = new List<Building>();
    int currentKey = 0;
    void Awake()
    {
        buildings = GetComponentsInChildren<Building>(true).ToList();

        foreach (var building in buildings)
        {
            BuildingUI buildingUI = building.GetComponentInChildren<BuildingUI>(true);
            if (buildingUI != null)
            {
                buildingUI.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        ShowBuilding(buildings[0]);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            UpdateBuilding();
        }

        if(Input.GetKey(KeyCode.Space))
        {
            TakeScreenshot();
        }
    }

    void UpdateBuilding()
    {
        currentKey += 1;
        if(currentKey >= buildings.Count)
        {
            currentKey = 0;
        }

        foreach (var building in buildings)
        {
            building.gameObject.SetActive(false);
        }

        ShowBuilding(buildings[currentKey]);
    }

    void ShowBuilding(Building baseBuilding)
    {
        baseBuilding.gameObject.SetActive(true);
    }

    void TakeScreenshot()
    {
        string path = Application.persistentDataPath + "/Screenshots/"; 

        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        string fileName = $"screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        string fullPath = path + fileName;

        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log($"Screenshot saved: {fullPath}");
    }
}
