using TMPro;
using UnityEngine;

using UnityEngine;

public class SystemProfiler : MonoBehaviour
{
    TextMeshProUGUI tmp;
    private float updateInterval = 0.5f;

    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private float fps;
    private float avgFps;

    private float cpuUsage;
    private float gpuUsage;
    private float ramUsage;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeleft <= 0.0)
        {
            fps = accum / frames;

            if (avgFps == 0)
                avgFps = fps;
            else
                avgFps = Mathf.Lerp(avgFps, fps, 0.1f);

            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;

            tmp.text = $"FPS {Mathf.RoundToInt(fps)}";
            
        }
    }

}
