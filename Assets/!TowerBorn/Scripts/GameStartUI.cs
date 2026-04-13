using UnityEngine;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    public static GameStartUI Instance;
    SimpleShowHideAnimator animator;
    [SerializeField] Button startBtn;
    void Awake()
    {
        animator = GetComponent<SimpleShowHideAnimator>();
        startBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
            animator.Hide();
        });

        Instance = this;
    }

    public void ResetGame()
    {
        animator.Show();
    }

    void Update()
    {
        
    }
}
