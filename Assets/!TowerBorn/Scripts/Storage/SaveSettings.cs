using UnityEngine;

[CreateAssetMenu(fileName = "SaveSettings", menuName = "Game/Save Settings")]
public class SaveSettings : ScriptableObject
{
    [SerializeField] private bool saveSystemEnabled = true;

    public bool SaveSystemEnabled => saveSystemEnabled;
}