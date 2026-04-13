using TMPro;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance;
    public int startLivesAmount = 20;
    public int currentLivesAmount = 0;

    [SerializeField] Animator livesAnimator;
    [SerializeField] ParticleSystem livesSpendVFX;
    [SerializeField] TextMeshProUGUI livesTMP;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    public void ResetLives()
    {
        currentLivesAmount = startLivesAmount;
        livesAnimator.SetTrigger("Action");
        livesTMP.text = currentLivesAmount.ToString();
    }
    public void SetLives(int amount)
    {
        currentLivesAmount = amount;
        livesAnimator.SetTrigger("Action");
        livesTMP.text = currentLivesAmount.ToString();
    }
    public void SpendLive()
    {
        currentLivesAmount--;
        livesAnimator.SetTrigger("Action");
        livesTMP.text = currentLivesAmount.ToString();
        livesSpendVFX.Play();
        if (currentLivesAmount <= 0)
        {
            GameManager.Instance.EndGame();
            WavesManager.Instance.LoseWave();
        }
    }
    void Update()
    {
        
    }
}
