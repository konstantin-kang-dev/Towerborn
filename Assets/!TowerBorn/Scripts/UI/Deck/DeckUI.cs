
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    public static DeckUI Instance;

    [SerializeField] List<Transform> _deckCardsCells = new List<Transform>();
    [SerializeField] List<DeckAndResearchCardInfo> _cardPrefabs = new List<DeckAndResearchCardInfo>();

    [SerializeField] SlidePanelTweener _slidePanelTweener;

    public int RollPrice { get; private set; } = 5;
    [SerializeField] RerollBtn _rerollBtn;
    [SerializeField] CanvasGroup _cardsBlocker;

    DeckCard _holdingCard = null;
    public List<DeckCard> DeckCards { get; private set; } = new List<DeckCard>();

    public bool IsInitialized { get; private set; } = false;
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        
    }

    public void Init()
    {
        ResourcesManager.OnResourceUpdated += UpdateCards;

        IsInitialized = true;
    }

    void ClearCells()
    {
        foreach (var deckCardCell in _deckCardsCells)
        {
            for (int i = 0; i < deckCardCell.childCount; i++)
            {
                Destroy(deckCardCell.GetChild(i).gameObject);
            }
        }
    }

    public void SetVisibility(bool visible)
    {
        if(visible)
        {
            _slidePanelTweener.Show();
            GenerateAllCards();
        }
        else
        {
            _slidePanelTweener.Hide();
        }
    }

    public void ResetDeck(Action OnComplete = null)
    {
        StartCoroutine(ResetRoutine(OnComplete));
    }

    IEnumerator ResetRoutine(Action OnComplete)
    {
        foreach (var deckCard in DeckCards)
        {
            deckCard.DestroyCard();
            yield return new WaitForSeconds(0.07f);
        }
        DeckCards.Clear();
        OnComplete?.Invoke();
    }

    public void GenerateAllCards()
    {
        ClearCells();
        StartCoroutine(AllCardsGenerationRoutine());
    }

    public void Reroll()
    {
        ResourcesManager.SpendResource(ResourceName.Gems, RollPrice);

        RollPrice += ProjectConstants.REROLL_INCREMENT_PRICE;

        ResetDeck(()=> StartCoroutine(AllCardsGenerationRoutine()));
    }

    IEnumerator AllCardsGenerationRoutine()
    {
        yield return new WaitForSeconds(0.15f);
        int cardsToCreate = _deckCardsCells.Count - DeckCards.Count;
        for (int i = 0; i < cardsToCreate; i++)
        {
            GenerateCard();
            yield return new WaitForSeconds(0.12f);
        }
    }

    public void GenerateCard()
    {
        if(IsDeckFull()) return;

        BuildingTier randomTier = DeckGenerator.GetRandomTier();
        BuildingConfig randomConfig = BuildingsDB.Instance.GetRandomUnlockedConfig(randomTier);

        int freeCellKey = GetFreeDeckKey();
        Transform deckCardCell = _deckCardsCells[freeCellKey];

        DeckAndResearchCardInfo cardPrefabInfo = _cardPrefabs.First((x) => x.BuildingTier == randomConfig.BuildingTier);
        GameObject deckCardGO = Instantiate(cardPrefabInfo.prefab, deckCardCell);
        deckCardGO.transform.position = deckCardCell.position;

        DeckCard deckCard = deckCardGO.GetComponent<DeckCard>();
        deckCard.SetData(randomConfig, freeCellKey, deckCardCell);
        DeckCards.Add(deckCard);
        deckCard.Init();
        deckCard.OnExecute += (DeckCard deckCard) =>
        {
            DeckCards.Remove(deckCard);
            Destroy(deckCard.gameObject);
            GenerateCard();
        };

        deckCard.OnDragStart += HandleDeckCardDragStart;
        deckCard.OnDragEnd += HandleDeckCardDragEnd;
    }

    void HandleDeckCardDragStart(DeckCard deckCard)
    {
        _holdingCard = deckCard;
    }
    void HandleDeckCardDragEnd(DeckCard deckCard)
    {
        _holdingCard = null;
    }

    public bool IsHoldingCard()
    {
        return _holdingCard != null;
    }

    public void UpdateCards(Resource resource)
    {
        foreach (var deckCard in DeckCards)
        {
            deckCard.UpdateCard();
        }

        _rerollBtn.UpdatePrice();
    }

    public void SetCardsBlocker(bool value)
    {
        if(value)
        {
            _cardsBlocker.gameObject.SetActive(true);

            _cardsBlocker.DOFade(1f, 0.2f);
            _cardsBlocker.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            _cardsBlocker.DOFade(1f, 0.25f);
            _cardsBlocker.transform
                .DOScale(new Vector3(0.75f, 0.75f, 0.75f), 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(()=> _cardsBlocker.gameObject.SetActive(false));
        }
        
    }

    public bool IsDeckFull()
    {
        return DeckCards.Count >= _deckCardsCells.Count;
    }
    public int GetFreeDeckKey()
    {
        List<int> keys = new List<int>();

        for (int i = 0; i < _deckCardsCells.Count; i++)
        {
            keys.Add(i);
        }

        foreach (var deckCard in DeckCards)
        {
            keys.Remove(deckCard.DeckKey);
        }

        return keys[0];
    }
}
