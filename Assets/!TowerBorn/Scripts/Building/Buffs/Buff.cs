using UnityEditor;
using UnityEngine;

public enum BuffType
{
    AtkSpeed, Damage, UltimateCooldownReduce
}
public class Buff
{
    public string senderId;
    public BuffType type;
    public float value;
    public float duration;
    public float timer = 0;

    public Buff Clone()
    {
        return new Buff()
        {
            senderId = senderId,
            type = type,
            value = value,
            duration = duration,
            timer = timer,
        };
    }
}