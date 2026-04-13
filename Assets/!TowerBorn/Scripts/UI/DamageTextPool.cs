using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DamageTextPool : MonoBehaviour
{
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private int initialPoolSize = 50;
    [SerializeField] private float displayTime = 1f;
    [SerializeField] private Vector3 randomOffset = new Vector3(0.5f, 0.5f, 0);
    [SerializeField] private int expansionSize = 10; // Сколько объектов добавлять при расширении пула

    private List<TextMeshPro> textPool = new List<TextMeshPro>();
    private Queue<int> availableIndices = new Queue<int>();

    private static DamageTextPool instance;
    public static DamageTextPool Instance { get { return instance; } }

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
        int startIndex = textPool.Count;
        for (int i = 0; i < size; i++)
        {
            GameObject textObj = Instantiate(damageTextPrefab, transform);
            TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();

            if (tmp == null)
            {
                Debug.LogError("Префаб текста урона должен содержать компонент TextMeshPro!");
                continue;
            }

            textObj.SetActive(false);
            textPool.Add(tmp);
            availableIndices.Enqueue(startIndex + i);
        }
    }

    private void ExpandPool()
    {
        Debug.Log($"Расширяем пул текстов урона на {expansionSize} объектов");
        InitializePool(expansionSize);
    }

    public void ShowDamageText(Vector3 position, float damageAmount, Color color)
    {
        if (availableIndices.Count == 0)
        {
            ExpandPool();
        }

        int index = availableIndices.Dequeue();
        TextMeshPro tmp = textPool[index];

        Vector3 randomPos = new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z)
        );

        tmp.transform.position = position + randomPos;
        tmp.text = ProjectUtils.FormatNumber(damageAmount);
        tmp.color = color;
        tmp.gameObject.SetActive(true);

        StartCoroutine(HideTextAfterDelay(index, tmp));
    }

    private IEnumerator HideTextAfterDelay(int index, TextMeshPro tmp)
    {
        float elapsedTime = 0f;
        Vector3 startPos = tmp.transform.position;
        Color startColor = tmp.color;

        while (elapsedTime < displayTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / displayTime;

            tmp.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 0.5f, normalizedTime);

            if (normalizedTime > 0.5f)
            {
                tmp.color = new Color(startColor.r, startColor.g, startColor.b,
                    Mathf.Lerp(1f, 0f, (normalizedTime - 0.5f) * 2f));
            }

            yield return null;
        }

        tmp.gameObject.SetActive(false);
        availableIndices.Enqueue(index);
    }
}