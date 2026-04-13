using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    float damageDealed = 0;
    [SerializeField] TextMeshProUGUI damageDealedTMP;

    int enemiesKilled = 0;
    [SerializeField] TextMeshProUGUI enemiesKilledTMP;

    int secondsInGame = 0;
    [SerializeField] TextMeshProUGUI secondsInGameTMP;

    private void Awake()
    {
        damageDealedTMP.text = ProjectUtils.FormatNumber(damageDealed);
        enemiesKilledTMP.text = ProjectUtils.FormatNumber(enemiesKilled);
        secondsInGameTMP.text = ProjectUtils.FormatTime(secondsInGame);

        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartTimer()
    {
        GameManager.Instance.OnTimerUpdate += HandleTimerUpdate;
    }
    public void StopTimer()
    {
        GameManager.Instance.OnTimerUpdate -= HandleTimerUpdate;
    }

    public void HandleTimerUpdate()
    {
        secondsInGame += 1;
        secondsInGameTMP.text = ProjectUtils.FormatTime(secondsInGame);
    }

    public void AddKill(int amount = 1)
    {
        enemiesKilled += amount;
        enemiesKilledTMP.text = ProjectUtils.FormatNumber(enemiesKilled);
    }

    public void AddDamageDealed(float damage)
    {
        damageDealed += damage;
        damageDealedTMP.text = ProjectUtils.FormatNumber(damageDealed);
    }

    public void ResetStats()
    {
        enemiesKilled = 0;
        enemiesKilledTMP.text = ProjectUtils.FormatNumber(enemiesKilled);

        secondsInGame = 0;
        secondsInGameTMP.text = ProjectUtils.FormatTime(secondsInGame);

        damageDealed = 0;
        damageDealedTMP.text = ProjectUtils.FormatNumber(damageDealed);
    }
}
