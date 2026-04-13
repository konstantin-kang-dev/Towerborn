using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageSpritePool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private int initialPoolSize = 50;
    [SerializeField] private int expansionSize = 10;

    [Header("Display Settings")]
    [SerializeField] private float displayTime = 1f;
    [SerializeField] private Vector3 randomOffset = new Vector3(0.5f, 0.5f, 0);
    [SerializeField] private float moveUpDistance = 0.5f;

    [Header("Number Sprites")]
    [SerializeField] private Sprite[] numberSprites = new Sprite[10]; // 0-9
    [SerializeField] private Sprite dotSprite;
    [SerializeField] private Sprite minusSprite;
    [SerializeField] private Sprite kSprite; // для тысяч (K)
    [SerializeField] private Sprite mSprite; // для миллионов (M)

    private List<DamageSpriteDisplay> damagePool = new List<DamageSpriteDisplay>();
    private Queue<int> availableIndices = new Queue<int>();

    private static DamageSpritePool instance;
    public static DamageSpritePool Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializePool(initialPoolSize);
    }

    private void InitializePool(int size)
    {
        int startIndex = damagePool.Count;

        for (int i = 0; i < size; i++)
        {
            GameObject damageObj = Instantiate(damagePrefab, transform);
            DamageSpriteDisplay display = damageObj.GetComponent<DamageSpriteDisplay>();

            if (display == null)
            {
                Debug.LogError("Префаб урона должен содержать компонент DamageNumberDisplay!");
                continue;
            }

            // Инициализируем спрайты цифр
            display.Initialize(numberSprites, dotSprite, minusSprite, kSprite, mSprite);

            damageObj.SetActive(false);
            damagePool.Add(display);
            availableIndices.Enqueue(startIndex + i);
        }
    }

    private void ExpandPool()
    {
        Debug.Log($"Расширяем пул урона на {expansionSize} объектов");
        InitializePool(expansionSize);
    }

    public void ShowDamageText(Vector3 position, float damageAmount, Color color)
    {
        if (availableIndices.Count == 0)
        {
            ExpandPool();
        }

        int index = availableIndices.Dequeue();
        DamageSpriteDisplay display = damagePool[index];

        // Случайная позиция
        Vector3 randomPos = new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z)
        );

        display.transform.position = position + randomPos;
        display.SetDamage(damageAmount, color);
        display.gameObject.SetActive(true);

        StartCoroutine(AnimateDamageText(index, display));
    }

    private IEnumerator AnimateDamageText(int index, DamageSpriteDisplay display)
    {
        float elapsedTime = 0f;
        Vector3 startPos = display.transform.position;

        while (elapsedTime < displayTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / displayTime;

            // Движение вверх
            display.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * moveUpDistance, normalizedTime);

            // Исчезновение во второй половине анимации
            if (normalizedTime > 0.5f)
            {
                float alpha = Mathf.Lerp(1f, 0f, (normalizedTime - 0.5f) * 2f);
                display.SetAlpha(alpha);
            }

            yield return null;
        }

        display.gameObject.SetActive(false);
        display.SetAlpha(1f); // Сбрасываем альфу для следующего использования
        availableIndices.Enqueue(index);
    }
}