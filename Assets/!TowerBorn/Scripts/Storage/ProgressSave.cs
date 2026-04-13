
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ProgressSave
{
    public DateTime lastExitDate;

    public int playerLevel = 1;
    public int playerExp = 0;
    public bool isFirstLaunch = true;
    
    public List<BuildingSave> researchList = new List<BuildingSave>();

    public ProgressSave() { }

}