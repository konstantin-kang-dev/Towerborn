using UnityEditor;
using UnityEngine;

public class WavesManagerTester : MonoBehaviour
{
    public void SkipWaves(int count = 1)
    {
        WavesManager.Instance.SkipWaves(count);
    }
    public void SetWave(int waveNumber = 1)
    {
        WavesManager.Instance.SetWave(waveNumber);
    }
}
