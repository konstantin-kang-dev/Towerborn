using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public enum GraphicsPreset
{
    Low, High
}
public enum FpsPreset
{
    Low, High
}
public class GraphicsManager : MonoBehaviour
{
    public static GraphicsManager Instance;

    [Header("URP Quality Assets")]
    [SerializeField] private UniversalRenderPipelineAsset lowQualityAsset;
    [SerializeField] private UniversalRenderPipelineAsset highQualityAsset;

    private void Awake()
    {
        Instance = this;
    }
    public void SetGraphicsQuality(GraphicsPreset preset)
    {
        switch (preset)
        {
            case GraphicsPreset.Low:
                QualitySettings.renderPipeline = lowQualityAsset;
                GraphicsSettings.defaultRenderPipeline = lowQualityAsset;
                break;
            case GraphicsPreset.High:
                QualitySettings.renderPipeline = highQualityAsset;
                GraphicsSettings.defaultRenderPipeline = highQualityAsset;
                break;
        }

        Debug.Log($"Graphics set to: {preset} Quality");
    }

    public void SetFPSLimit(FpsPreset preset)
    {
        switch (preset)
        {
            case FpsPreset.Low:
                Application.targetFrameRate = 30;
                break;
            case FpsPreset.High:
                Application.targetFrameRate = 60;
                break;
        }
    }
}