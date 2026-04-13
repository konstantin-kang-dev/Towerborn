using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuffsController
{
    public Dictionary<string, Buff> Buffs = new();

    public void Init()
    {
        Buffs = new Dictionary<string, Buff>();
    }
    public void AddBuff(Buff buff)
    {
        /*
        Debug.Log($"Buff Info:");
        Debug.Log($"SenderID: {buff.senderId}");
        Debug.Log($"Type: {buff.type}");
        Debug.Log($"Duration: {buff.duration}");
        Debug.Log($"Value: {buff.value}");
        Debug.Log($"Total buffs: {Buffs.Count}");
        */

        if (Buffs.ContainsKey(buff.senderId))
        {
            Buffs.Remove(buff.senderId);
        }
        buff.timer = 0;
        Buffs.Add(buff.senderId, buff);
    }
    public void RemoveBuff(Buff buff)
    {
        /*
        Debug.Log($"Buff Info:");
        Debug.Log($"SenderID: {buff.senderId}");
        Debug.Log($"Type: {buff.type}");
        Debug.Log($"Duration: {buff.duration}");
        Debug.Log($"Value: {buff.value}");
        Debug.Log($"Total buffs: {Buffs.Count}");
        */

        if (Buffs.ContainsKey(buff.senderId))
        {
            Buffs.Remove(buff.senderId);
        }
    }

    public void UpdateTimer()
    {
        foreach (var buffBlock in new Dictionary<string, Buff>(Buffs))
        {
            buffBlock.Value.timer += Time.deltaTime;

            if (buffBlock.Value.timer >= buffBlock.Value.duration)
            {
                Buffs.Remove(buffBlock.Key);
            }
        }

    }

    public float CalculateBuffValue(BuffType type)
    {
        float result = 0f;

        result += Buffs.Sum(pair =>
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

        return result;
    }
}