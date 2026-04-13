using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum DayCycleStage
{
    None, Day, Sunset, Night, Sunrise
}

[Serializable]
public class DayCycleStageInfo
{
    public DayCycleStage Stage;
    public Color cameraColor;
    public float lightYRotation;
    public float lightIntensity;
}

public class DayCycle : MonoBehaviour
{
    [SerializeField] Light globalLight;
    [SerializeField] Camera mainCamera;

    [SerializeField] ParticleSystem dayVFX;
    [SerializeField] ParticleSystem nightVFX;
    [Header("Cycle properties")]
    [SerializeField] float oneDayDuration = 360f;
    public float stageProgress = 0f;
    float stageDuration => oneDayDuration / stages.Count;

    [SerializeField] List<DayCycleStageInfo> stages = new List<DayCycleStageInfo>();

    Coroutine cycleRoutine;
    public DayCycleStage startStage = DayCycleStage.Day;
    public DayCycleStageInfo PreviousStage;
    public DayCycleStageInfo CurrentStage;


    void Start()
    {
        SwitchToNextStage(startStage);
        cycleRoutine = StartCoroutine(CycleRoutine());
    }

    IEnumerator CycleRoutine()
    {
        float stageTimer = 0f;

        while (true)
        {
            stageTimer += Time.fixedDeltaTime;
            stageProgress = stageTimer / stageDuration;

            globalLight.intensity = Mathf.Lerp(PreviousStage.lightIntensity, CurrentStage.lightIntensity, stageProgress);

            float previousRotation = PreviousStage.lightYRotation;
            float targetRotation = CurrentStage.lightYRotation;
            if (targetRotation < PreviousStage.lightYRotation)
            {
                targetRotation += 360f;
            }

            float newRotationY = Mathf.Lerp(previousRotation, targetRotation, stageProgress);

            Vector3 newRotation = globalLight.transform.eulerAngles;
            newRotation.y = newRotationY;
            globalLight.transform.eulerAngles = newRotation;

            mainCamera.backgroundColor = Color.Lerp(PreviousStage.cameraColor, CurrentStage.cameraColor, stageProgress);

            if (stageTimer >= stageDuration)
            {
                SwitchToNextStage();
                stageTimer = 0;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void SwitchToNextStage(DayCycleStage stage = DayCycleStage.None)
    {
        if(stage == DayCycleStage.None)
        {
            PreviousStage = CurrentStage;

            switch (CurrentStage.Stage)
            {
                case DayCycleStage.Day:
                    CurrentStage = CurrentStage = stages.First((x) => x.Stage == DayCycleStage.Sunset);
                    break;
                case DayCycleStage.Sunset:
                    CurrentStage = CurrentStage = stages.First((x) => x.Stage == DayCycleStage.Night);
                    dayVFX.Stop();
                    nightVFX.Play();
                    break;
                case DayCycleStage.Night:
                    CurrentStage = CurrentStage = stages.First((x) => x.Stage == DayCycleStage.Sunrise);
                    break;
                case DayCycleStage.Sunrise:
                    CurrentStage = CurrentStage = stages.First((x) => x.Stage == DayCycleStage.Day);
                    dayVFX.Play();
                    nightVFX.Stop();
                    break;
            }
        }
        else
        {
            CurrentStage = stages.First((x)=> x.Stage == stage);
            int index = stages.IndexOf(CurrentStage);
            if(index == 0)
            {
                index = stages.Count - 1;
            }
            else
            {
                index -= 1;
            }
            PreviousStage = stages[index];

            switch (CurrentStage.Stage)
            {
                case DayCycleStage.Day:
                    dayVFX.Play();
                    nightVFX.Stop();
                    break;
                case DayCycleStage.Sunset:
                    dayVFX.Stop();
                    nightVFX.Play();
                    break;
                case DayCycleStage.Night:
                    dayVFX.Stop();
                    nightVFX.Play();
                    break;
                case DayCycleStage.Sunrise:
                    dayVFX.Play();
                    nightVFX.Stop();
                    break;
            }
        }

    }
}