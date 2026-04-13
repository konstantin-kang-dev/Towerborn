using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ButtonState
{
    Normal, Pressed, Selected, Disabled
}
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Animator))]
public class SimpleUIButton : MonoBehaviour
{
    public Button Button {  get; private set; }
    ButtonState currentState = ButtonState.Normal;
    Animator animator;
    private void Awake()
    {
        Button = GetComponent<Button>();
        animator = GetComponent<Animator>();

        Button.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SfxType.UIClick);
        });
    }


}
