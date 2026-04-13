using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomButtonSelector : MonoBehaviour
{
    [SerializeField] Transform roller;
    [SerializeField] TextMeshProUGUI rollerTMP;

    [SerializeField] List<Button> buttons;

    public int currentValue;

    public event Action<int> OnValueChanged;

    void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Button button = buttons[i];

            int key = i;
            button.onClick.AddListener(() =>
            {
                SetValue(key);
            });
        }
    }

    public void SetValue(int value, bool snap = false)
    {
        Debug.Log($"Set value: {value}");
        currentValue = value;

        OnValueChanged?.Invoke(value);

        TriggerRoll(buttons[currentValue], snap);
    }

    void TriggerRoll(Button targetButton, bool snap)
    {
        TextMeshProUGUI targetButtonTMP = targetButton.GetComponentInChildren<TextMeshProUGUI>();
        rollerTMP.text = targetButtonTMP.text;

        if (snap || !gameObject.activeInHierarchy)
        {
            roller.position = targetButton.transform.position;
        }
        else
        {
            Tween moveAnim = roller
                .DOMove(targetButton.transform.position, 0.2f)
                .SetEase(Ease.OutBack);

            AudioManager.Instance.PlaySFX(SfxType.UISwitch);
        }
            
    }

}
