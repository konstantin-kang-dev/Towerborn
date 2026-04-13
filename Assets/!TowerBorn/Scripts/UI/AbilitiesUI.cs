
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesUI : MonoBehaviour
{
    [SerializeField] GameObject abilityPrefab;

    void Start()
    {
        
    }

    public void Generate(List<AbilityConfig> abilities)
    {
        Clear();
        foreach (AbilityConfig abilityConfig in abilities)
        {
            GameObject abilityGO = Instantiate(abilityPrefab, transform);

            AbilityUI abilityUI = abilityGO.GetComponent<AbilityUI>();
            abilityUI.Init(abilityConfig);

        }
    }

    public void Clear()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
