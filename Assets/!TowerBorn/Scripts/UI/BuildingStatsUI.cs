using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CombatStatIconInfo
{
    public CombatStatType statType;
    public Sprite icon;
}
public class BuildingStatsUI : MonoBehaviour
{
    [SerializeField] List<CombatStatIconInfo> combatIcons = new List<CombatStatIconInfo>();
    [SerializeField] GameObject statUIPrefab;
    
    public void Init(CombatStats combatStats)
    {
        DestroyAllChildren();

        foreach(CombatStat combatStat in combatStats.statsList)
        {
            if (combatStat.statValue == 0) continue;
            GameObject statUIGO = Instantiate(statUIPrefab, transform);
            BuildingStatUI statUI = statUIGO.GetComponent<BuildingStatUI>();

            CombatStatIconInfo combatIcon = combatIcons.First((x)=> x.statType == combatStat.statType);
            string postFix = string.Empty;
            switch (combatStat.statType)
            {
                case CombatStatType.AtkInterval:
                    postFix = "s";
                    break;
            }
            statUI.Init(combatIcon, combatStat.statValue.ToString() + postFix);
        }
    }

    void DestroyAllChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
