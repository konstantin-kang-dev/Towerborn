using System.Collections;
using TowerBorn.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI Instance {get; private set;}

    [SerializeField] PopupTweener _popupTweener;

    [SerializeField] Button _bgBtn;

    [SerializeField] Slider _sfxSlider;
    [SerializeField] Slider _musicSlider;

    [SerializeField] CustomButtonSelector _graphicsSelector;
    [SerializeField] CustomButtonSelector _fpsSelector;
    void Awake()
    {
        _popupTweener = GetComponent<PopupTweener>();

        _bgBtn.onClick.AddListener(Hide);

        _sfxSlider.onValueChanged.AddListener((float value) =>
        {
            GameSettings.SetSFXValue(value);
        });

        _musicSlider.onValueChanged.AddListener((float value) =>
        {
            GameSettings.SetMusicValue(value);
        });

        _graphicsSelector.OnValueChanged += (value) =>
        {
            GameSettings.SetGraphicsPreset(value);
        };

        _fpsSelector.OnValueChanged += (value) =>
        {
            GameSettings.SetFpsPreset(value);
        };

        Instance = this;
    }


    void Update()
    {

    }

    public void Show()
    {
        _popupTweener.Show();
        UpdateUI();

        AudioManager.Instance.PlaySFX(SfxType.UISlideOpen);
    }
    public void Hide()
    {
        _popupTweener.Hide();

        AudioManager.Instance.PlaySFX(SfxType.UISlideClose);
    }


    public void UpdateUI()
    {
        SettingsSave save = SavesManager.SettingsSave;
        _sfxSlider.value = save.sfxVolume;
        _musicSlider.value = save.musicVolume;
        _graphicsSelector.SetValue((int)save.graphicsPreset, true);
        _fpsSelector.SetValue((int)save.fpsPreset, true);
    }

}