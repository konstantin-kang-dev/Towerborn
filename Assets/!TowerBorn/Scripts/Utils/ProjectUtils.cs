using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ProjectUtils : ScriptableObject
{
    public static bool HasReachedTarget(Vector3 currentPosition, Vector3 targetPosition, float threshold = 0.1f)
    {
        return Vector3.Distance(currentPosition, targetPosition) <= threshold;
    }
    public static int GetSecondsPassed(DateTime from, DateTime now)
    {
        double res = 0;
        TimeSpan diff = now - from;
        res = diff.TotalSeconds;
        return (int)res;
    }
    public static string FormatNumber(float number)
    {
        int intValue = Mathf.RoundToInt(number);

        string numStr = intValue.ToString();

        bool isNegative = false;
        if (numStr.StartsWith("-"))
        {
            isNegative = true;
            numStr = numStr.Substring(1);
        }

        StringBuilder result = new StringBuilder();

        for (int i = numStr.Length - 1, count = 0; i >= 0; i--, count++)
        {
            if (count > 0 && count % 3 == 0)
            {
                result.Insert(0, ".");
            }

            result.Insert(0, numStr[i]);
        }

        return isNegative ? "-" + result.ToString() : result.ToString();
    }

    public static string FormatTime(float totalSeconds)
    {
        int seconds = Mathf.FloorToInt(totalSeconds);

        int hours = seconds / 3600;
        seconds %= 3600;
        int minutes = seconds / 60;
        seconds %= 60;

        System.Text.StringBuilder result = new System.Text.StringBuilder();

        if (hours > 0)
        {
            result.Append(hours).Append("h ");
        }

        if (minutes > 0)
        {
            result.Append(minutes).Append("m ");
        }

        if (seconds > 0 || (hours == 0 && minutes == 0))
        {
            result.Append(seconds).Append("s");
        }

        return result.ToString();
    }

    public static Vector3 RandomizePositionXZ(Vector3 position, float minOffset, float maxOffset)
    {
        Vector3 offset = Vector3.zero;

        offset.x = UnityEngine.Random.Range(minOffset, maxOffset);
        offset.z = UnityEngine.Random.Range(minOffset, maxOffset);

        return position + offset;
    }

    public static void Vibrate()
    {

    }

    public static int[] DivideIntoParts(int value, int parts)
    {
        if (parts <= 0)
            throw new ArgumentException("Количество частей должно быть положительным числом", nameof(parts));

        if (parts == 1)
            return new int[] { value };

        int[] result = new int[parts];

        int baseValue = value / parts;

        int remainder = value % parts;

        for (int i = 0; i < parts; i++)
        {
            result[i] = baseValue;
        }

        for (int i = 0; i < remainder; i++)
        {
            result[i]++;
        }

        return result;
    }

    public static Vector3 Parabola(Vector3 startPos, Vector3 targetPos, float height, float t)
    {
        t = Mathf.Clamp01(t);

        Vector3 linearPos = Vector3.Lerp(startPos, targetPos, t);

        float heightOffset = 4f * height * t * (1f - t);

        return new Vector3(linearPos.x, linearPos.y + heightOffset, linearPos.z);
    }

    public static int RandomIntExcept(int min, int max, int except)
    {
        int result = UnityEngine.Random.Range(min, max);

        if (result == except)
        {
            if (result == max - 1)
            {
                result -= 1;
            }
            else if (result == min)
            {
                result += 1;
            }
            else
            {
                result += 1;
            }
        }

        return result;
    }
    public static float CalculatePercent(float baseValue, float percent)
    {
        return (baseValue / 100f) * percent;
    }
}