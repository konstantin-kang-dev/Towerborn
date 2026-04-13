using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DebuffsManager
{
    BaseUnit owner;
    public Dictionary<string, Debuff> Debuffs = new();

    public void Init(BaseUnit owner)
    {
        this.owner = owner;
        Debuffs = new Dictionary<string, Debuff>();
    }
    public void AddDebuff(Debuff debuff)
    {
        /*
        Debug.Log($"Buff Info:");
        Debug.Log($"SenderID: {debuff.senderId}");
        Debug.Log($"Type: {debuff.type}");
        Debug.Log($"Duration: {debuff.duration}");
        Debug.Log($"Value: {debuff.value}");
        Debug.Log($"Total buffs: {Debuffs.Count}");
        */

        if (Debuffs.ContainsKey(debuff.senderId))
        {
            Debuffs.Remove(debuff.senderId);
        }
        debuff.timer = 0;
        Debuffs.Add(debuff.senderId, debuff);
    }

    public void Clear()
    {
        Debuffs.Clear();
    }

    public void UpdateTimer(float deltaTime)
    {
        foreach (var buffBlock in new Dictionary<string, Debuff>(Debuffs))
        {
            Debuff debuff = buffBlock.Value;
            debuff.timer += deltaTime;

            int secondsPassed = Mathf.FloorToInt(debuff.timer);
            int previousSecondsPassed = debuff.secondsPassed;
           
            if (secondsPassed > previousSecondsPassed)
            {
                //Debug.Log($"{owner.GetInstanceID()} Debuff OnSecondsUpdate! secondsPassed: {secondsPassed}");
                debuff.TriggerOnSecondsUpdate();
            }

            debuff.secondsPassed = secondsPassed;

            if (debuff.timer >= debuff.duration)
            {
                Debuffs.Remove(buffBlock.Key);
            }
        }
    }

    public float CalculateDebuffValue(DebuffType type)
    {
        float result = 0f;

        switch (type)
        {
            case DebuffType.Slowing:
                List<Debuff> slowingDebuffs = Debuffs.Values.Where((x) => x.type == DebuffType.Slowing).ToList();
                result = slowingDebuffs.Count > 0 ? slowingDebuffs[0].value : 0f;
                break;
            default:
                result += Debuffs.Sum(pair =>
                {
                    if (pair.Value.type == type)
                    {
                        return pair.Value.value;
                    }
                    else
                    {
                        return 0;
                    }
                });
                break;
        }


        return result;
    }

    public bool IsDebuffedBy(DebuffType type)
    {
        bool result = false;

        result = Debuffs.Values.Any((x) => x.type == type);

        return result;
    }
}