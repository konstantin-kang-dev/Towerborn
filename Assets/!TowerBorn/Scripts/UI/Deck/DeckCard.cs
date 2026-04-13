using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Transform DeckCardCell;
    BuildingConfig BuildingConfig;
    public int DeckKey = 0;
    Building Building;

    [Header("UI properties")]
    [SerializeField] Transform wrapper;
    [SerializeField] TextMeshProUGUI titleTMP;
    [SerializeField] Image mainSprite;
    [SerializeField] TextMeshProUGUI priceTMP;

    [Header("Positions and transitions")]
    public Vector3 initialPosition;
    public Vector3 initialScale;
    float distanceToDissapear = 250f;

    public Action<DeckCard> OnDragStart;
    public Action<DeckCard> OnDragEnd;
    public Action<DeckCard> OnExecute;

    [Header("States")]
    public bool IsActive = false;
    public bool IsEnteredGameArea = false;
    public bool IsDragging = false;
    public bool IsAnimating = false;
    public bool IsInitialized = false;
    void Awake()
    {

    }

    public void SetData(BuildingConfig config, int deckKey, Transform deckCardCell)
    {
        BuildingConfig = config;
        DeckKey = deckKey;
        DeckCardCell = deckCardCell;

        titleTMP.text = config.DisplayName;
        mainSprite.sprite = config.CardSprite;
        priceTMP.text = ProjectUtils.FormatNumber(config.BuildPrice);
    }

    public void Init()
    {
        UpdateCard();

        Vector3 startScale = new Vector3(0.75f, 0.75f, 0.75f);
        wrapper
            .DOScale(Vector3.one, 0.25f)
            .From(startScale)
            .SetEase(Ease.OutBack, 10f);

        wrapper
            .DOLocalMoveY(0f, 0.25f)
            .From(100f)
            .SetEase(Ease.InQuad, 2f);

        IsInitialized = true;
    }

    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsActive) return;
        if (!IsInitialized) return;
        if (IsAnimating) return;
        if (DeckUI.Instance.IsHoldingCard()) return;

        initialPosition = transform.position;
        initialScale = wrapper.localScale;

        DeckCardCell.SetAsLastSibling();

        IsDragging = true;

        OnDragStart?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!IsDragging) return;

        transform.position = eventData.position;

        float initialHeight = initialPosition.y;
        float currentHeight = transform.position.y;

        currentHeight = Mathf.Max(currentHeight, initialHeight);

        float distance = currentHeight - initialHeight;
        float scale = (distanceToDissapear - distance) / distanceToDissapear;
        scale = Mathf.Clamp(scale, 0f, 1f);
        
        if(scale <= 0f) HandleEnterGameArea();
        if(scale >= 0.2f) HandleLeaveGameArea();

        if(Building != null)
        {
            BuildingPlacer.Instance.UpdateBuildingPos(eventData.position);
        }

        wrapper.localScale = new Vector3(scale, scale, scale);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDragging) return;

        if (IsEnteredGameArea)
        {
            ExecuteCard();
        }
        else
        {
            Sequence sequence = DOTween.Sequence();

            Tween moveAnim = transform.DOLocalMove(Vector3.zero, 0.25f);

            Tween scaleAnim = wrapper.DOScale(Vector3.one, 0.25f);

            sequence.Append(moveAnim).Join(scaleAnim);
        }
        IsDragging = false;

        OnDragEnd?.Invoke(this);
    }

    void HandleEnterGameArea()
    {
        if (IsEnteredGameArea) return;
        IsEnteredGameArea = true;

        Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(transform.position);

        Building = BuildingsManager.Instance.SpawnBuilding(BuildingConfig, touchWorldPosition);
        
        BuildingSelectionManager.Instance.SelectBuilding(Building);
        BuildingPlacer.Instance.StartMovingBuilding(Building);
    }

    void HandleLeaveGameArea()
    {
        if (!IsEnteredGameArea) return;
        IsEnteredGameArea = false;
        GameGrid.Instance.MarkCellAsFree(Building.PlacementCell);

        BuildingSelectionManager.Instance.DeselectBuilding();
        //BuildingPlacer.Instance.EndMovingBuilding();

        Destroy(Building.gameObject);
        Building = null;
    }

    void ExecuteCard()
    {
        IsEnteredGameArea = false;
        //Debug.Log($"ExecuteCard");

        if(BuildingSelectionManager.Instance.MergeableBuilding == null)
        {
            BuildingSelectionManager.Instance.DeselectBuilding();
        }

        if (Building != null)
        {
            Building.Init();
        }

        //BuildingPlacer.Instance.EndMovingBuilding();

        Building = null;

        ResourcesManager.SpendResource(ResourceName.Gold, BuildingConfig.BuildPrice);

        OnExecute?.Invoke(this);
    }

    public void UpdateCard()
    {
        IsActive = ResourcesManager.CheckResourceForEnough(ResourceName.Gold, BuildingConfig.BuildPrice);

        priceTMP.color = IsActive ? Color.white : Color.red;
    }

    public void DestroyCard()
    {
        HandleLeaveGameArea();

        wrapper
            .DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
