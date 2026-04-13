using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RerollBtn : MonoBehaviour
{
    Button btn;
    [SerializeField] Image rerollIcon;
    [SerializeField] TextMeshProUGUI priceTMP;

    bool isActive = false;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(HandleClick);
    }
    public void HandleClick()
    {
        if (isActive) return;

        if (ResourcesManager.CheckResourceForEnough(ResourceName.Gems, DeckUI.Instance.RollPrice))
        {
            StartCoroutine(RotationRoutine());
        }
    }

    IEnumerator RotationRoutine()
    {
        isActive = true;
        btn.interactable = false;

        Vector3 targetEulerRotation = new Vector3(0, 0, -1080f);
        Vector3 targetScale = new Vector3(1.25f, 1.25f, 1.25f);

        rerollIcon.transform
            .DORotate(targetEulerRotation, 0.6f, RotateMode.FastBeyond360)
            .From(Vector3.zero)
            .SetEase(Ease.InOutQuad);

        rerollIcon.transform
            .DOScale(targetScale, 0.45f)
            .From(Vector3.one)
            .SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(0.45f);

        rerollIcon.transform
            .DOScale(Vector3.one, 0.15f)
            .SetEase(Ease.OutBack);

        DeckUI.Instance.Reroll();
        UpdatePrice();

        yield return new WaitForSeconds(1.5f);
        btn.interactable = true;
        isActive = false;
    }

    public void UpdatePrice()
    {
        int price = DeckUI.Instance.RollPrice;
        priceTMP.text = price.ToString();

        if(ResourcesManager.CheckResourceForEnough(ResourceName.Gems, price))
        {
            priceTMP.color = Color.white;
        }
        else
        {
            priceTMP.color = Color.red;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(priceTMP.rectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(priceTMP.rectTransform.parent.GetComponent<RectTransform>());
    }
}