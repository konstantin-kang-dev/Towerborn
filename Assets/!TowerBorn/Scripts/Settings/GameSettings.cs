using TowerBorn.SaveSystem;
using UnityEditor;
using UnityEngine;

public class GameSettings
{

    public static void SetSFXValue(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        SavesManager.SettingsSave.sfxVolume = value;
        SavesManager.SaveSettings();
    }
    public static void SetMusicValue(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        SavesManager.SettingsSave.musicVolume = value;
        SavesManager.SaveSettings();
    }

    public static void SetGraphicsPreset(int value)
    {
        GraphicsPreset preset = (GraphicsPreset)value;

        GraphicsManager.Instance.SetGraphicsQuality(preset);

        SavesManager.SettingsSave.graphicsPreset = preset;
        SavesManager.SaveSettings();
    }
    public static void SetFpsPreset(int value)
    {
        FpsPreset preset = (FpsPreset)value;

        GraphicsManager.Instance.SetFPSLimit(preset);

        SavesManager.SettingsSave.fpsPreset = preset;
        SavesManager.SaveSettings();
    }

    public static void ApplySave()
    {
        SettingsSave save = SavesManager.SettingsSave;
        SetSFXValue(save.sfxVolume);

        SetMusicValue(save.musicVolume);

        SetGraphicsPreset((int)save.graphicsPreset);

        SetFpsPreset((int)save.fpsPreset);
    }
}