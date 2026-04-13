using System;
using UnityEditor;
using UnityEngine;

public enum DebuffType
{
    Slowing, PeriodicDamage
}
public class Debuff
{
    public string senderId;
    public DebuffType type;
    public float value;
    public float duration;
    public float timer = 0;
    public int secondsPassed = 0;

    public event Action OnSecondsUpdate;

    public void TriggerOnSecondsUpdate()
    {
        OnSecondsUpdate?.Invoke();
    }
    public Debuff Clone()
    {
        return new Debuff()
        {
            senderId = senderId,
            type = type,
            value = value,
            duration = duration,
            timer = timer,
            OnSecondsUpdate = OnSecondsUpdate,
        };
    }
}