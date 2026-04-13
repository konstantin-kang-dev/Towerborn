using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Progression", menuName = "GameData/ProgressionConfig")]
public class ProgressionConfig : ScriptableObject
{
    public List<ProgressionStatInfo> progressionStatInfos = new List<ProgressionStatInfo>();
}