using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerBorn.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DeckAndResearchCardInfo
{
    public BuildingTier BuildingTier;
    public GameObject prefab;
}

public class ResearchUI : MonoBehaviour
{
    public static ResearchUI Instance { get; private set; }

    [SerializeField] SlidePanelTweener _slidePanelTweener;

    [SerializeField] Button _openBtn;
    [SerializeField] Button _closeBtn;

    [SerializeField] Transform _cardsParent;
    [SerializeField] List<DeckAndResearchCardInfo> _cardPrefabs = new List<DeckAndResearchCardInfo>();

    List<ResearchCard> _researchCards = new List<ResearchCard>();
    Dictionary<Transform, CanvasGroup> _cardsDict = new Dictionary<Transform, CanvasGroup>();

    Coroutine animationCardsRoutine;
    void Awake()
    {
        _openBtn.onClick.AddListener(() => SetVisibility(true));
        _closeBtn.onClick.AddListener(() => SetVisibility(false));


        Instance = this;
    }

    void Update()
    {

    }

    public void Init()
    {
        ResourcesManager.OnResourceUpdated += UpdateUI;

        LoadCards();
        SetVisibility(false);
    }

    void LoadCards()
    {
        for (int i = 0; i < _cardsParent.childCount; i++)
        {
            Destroy(_cardsParent.GetChild(i).gameObject);
        }

        List<BuildingConfig> configs = BuildingsDB.Instance.AllConfigs.Values.ToList();

        configs = configs.ToList();
        foreach (BuildingConfig config in configs)
        {
            DeckAndResearchCardInfo cardPrefabInfo = _cardPrefabs.First((x)=> x.BuildingTier == config.BuildingTier);
            ResearchCard newCard = Instantiate(cardPrefabInfo.prefab, _cardsParent).GetComponent<ResearchCard>();
            newCard.Init(config);
            _researchCards.Add(newCard);
        }

    }

    void SortCards()
    {
        var children = _cardsParent.Cast<Transform>()
                .OrderBy(t => t.GetComponent<ResearchCard>().GetPriority())
                .ToArray();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }

        _cardsDict.Clear();
        for (int i = 0; i < children.Length; i++)
        {
            Transform cardTransform = children[i];
            CanvasGroup cardCanvasGroup = cardTransform.GetComponent<CanvasGroup>();

            _cardsDict[cardTransform] = cardCanvasGroup;
        }
    }

    void AnimateCards()
    {
        foreach (var cardBlock in _cardsDict)
        {
            Transform cardTransform = cardBlock.Key;
            CanvasGroup cardCanvasGroup = cardBlock.Value;

            cardCanvasGroup.alpha = 0f;
        }

        animationCardsRoutine = StartCoroutine(AnimationCardsRoutine());
    }

    IEnumerator AnimationCardsRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var cardBlock in _cardsDict)
        {
            Transform cardTransform = cardBlock.Key;
            CanvasGroup cardCanvasGroup = cardBlock.Value;

            Vector2 fromPos = cardTransform.localPosition - new Vector3(0, 200f);

            Sequence sequence = DOTween.Sequence();

            Tween moveAnim = cardTransform
                .DOLocalMove(cardTransform.localPosition, 0.25f)
                .From(fromPos)
                .SetEase(Ease.OutBack, 2f);

            Tween fadeAnim = cardCanvasGroup
                .DOFade(1f, 0.25f)
                .From(0f);

            sequence.Append(fadeAnim).Join(moveAnim);

            yield return new WaitForSeconds(0.05f);
        }
    }


    public void UnlockCards(List<BuildingConfig> configs)
    {
        foreach (var config in configs)
        {
            ResearchCard researchCard = _researchCards.First((x) => x.Config.Id == config.Id);

            researchCard.SetUnlockedState(true);
        }
        Save();
    }

    public void UpdateUI(Resource resource)
    {
        foreach (var card in _researchCards)
        {
            card.UpdateCard();
        }
    }

    public void SetVisibility(bool state)
    {
        if (state)
        {
            _slidePanelTweener.Show();
            SortCards();
            AnimateCards();
            AudioManager.Instance.PlaySFX(SfxType.UISlideClickOpen);
        }
        else
        {
            _slidePanelTweener.Hide();
            AudioManager.Instance.PlaySFX(SfxType.UISlideClickClose);

            if (animationCardsRoutine != null)
            {
                StopCoroutine(animationCardsRoutine);
                animationCardsRoutine = null;
            }
        }
    }

    public void Save()
    {
        List<BuildingSave> saves = new List<BuildingSave>();

        foreach (var card in _researchCards)
        {
            BuildingSave save = new BuildingSave()
            {
                id = card.Config.Id,
                isUnlocked = card.isUnlocked,
            };
            saves.Add(save);
        }

        SavesManager.ProgressSave.researchList = saves;
    }

    public void ApplySave()
    {
        foreach (var buildingSave in SavesManager.ProgressSave.researchList)
        {
            ResearchCard researchCard = _researchCards.First((x) => x.Config.Id == buildingSave.id);

            if(researchCard == null)
            {
                Debug.LogError($"Not found card in database. Id: {buildingSave.id}");
                continue;
            }
            researchCard.SetUnlockedState(buildingSave.isUnlocked);
        }
    }

}
