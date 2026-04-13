using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingAudioController : MonoBehaviour
{
    Building building;
    [SerializeField] EnhancedAudioController attackAudioController;
    [SerializeField] float attackAudioDelay = 0f;

    public void Init(Building building)
    {
        this.building = building;
        AudioManager.Instance.OnSfxVolumeChange += UpdateVolume;
    }

    public void PlayAttackSFX()
    {
        attackAudioController.Play(attackAudioDelay, building.BuildingConfig.Id + "_Attack");
    }

    void UpdateVolume(float volume)
    {
        attackAudioController.SetVolume(volume);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.OnSfxVolumeChange -= UpdateVolume;
    }
}