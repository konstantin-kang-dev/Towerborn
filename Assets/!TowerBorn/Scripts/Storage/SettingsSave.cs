using System;
using UnityEditor;
using UnityEngine;


[Serializable]
public class SettingsSave
{
    public float sfxVolume = 0.5f;
    public float musicVolume = 0.5f;

    public GraphicsPreset graphicsPreset = GraphicsPreset.High;
    public FpsPreset fpsPreset = FpsPreset.High;

    public SettingsSave()
    {

    }
}