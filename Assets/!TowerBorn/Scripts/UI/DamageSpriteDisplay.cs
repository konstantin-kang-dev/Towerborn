using UnityEngine;

[System.Serializable]
public class DamageSpriteDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] digitRenderers;
    [SerializeField] private int maxDigits = 6;

    private Sprite[] numberSprites;
    private Sprite dotSprite, minusSprite, kSprite, mSprite;
    private Color originalColor;

    private void Awake()
    {
        // Создаем рендереры для цифр если их нет
        if (digitRenderers == null || digitRenderers.Length == 0)
        {
            CreateDigitRenderers();
        }

        transform.eulerAngles = new Vector3(45, 45, 0);
    }

    private void CreateDigitRenderers()
    {
        digitRenderers = new SpriteRenderer[maxDigits];

        for (int i = 0; i < maxDigits; i++)
        {
            GameObject digitObj = new GameObject($"Digit_{i}");
            digitObj.transform.SetParent(transform);
            digitObj.transform.localPosition = new Vector3(i * 0.3f, 0, 0); // Расстояние между цифрами

            digitRenderers[i] = digitObj.AddComponent<SpriteRenderer>();
            digitRenderers[i].sortingLayerName = "UI"; // Установите нужный слой
            digitRenderers[i].sortingOrder = 100;
        }
    }

    public void Initialize(Sprite[] numbers, Sprite dot, Sprite minus, Sprite k, Sprite m)
    {
        numberSprites = numbers;
        dotSprite = dot;
        minusSprite = minus;
        kSprite = k;
        mSprite = m;
    }

    public void SetDamage(float damage, Color color)
    {
        originalColor = color;

        // Форматируем число
        string damageText = ProjectUtils.FormatNumber(damage);

        // Центрируем текст
        float totalWidth = damageText.Length * 0.3f;
        float startX = -totalWidth * 0.5f;

        // Отображаем каждый символ
        for (int i = 0; i < digitRenderers.Length; i++)
        {
            if (i < damageText.Length)
            {
                char character = damageText[i];
                Sprite spriteToUse = GetSpriteForCharacter(character);

                digitRenderers[i].sprite = spriteToUse;
                digitRenderers[i].color = color;
                digitRenderers[i].transform.localPosition = new Vector3(startX + i * 0.3f, 0, 0);
                digitRenderers[i].gameObject.SetActive(true);
            }
            else
            {
                digitRenderers[i].gameObject.SetActive(false);
            }
        }
    }

    private Sprite GetSpriteForCharacter(char character)
    {
        switch (character)
        {
            case '0': return numberSprites[0];
            case '1': return numberSprites[1];
            case '2': return numberSprites[2];
            case '3': return numberSprites[3];
            case '4': return numberSprites[4];
            case '5': return numberSprites[5];
            case '6': return numberSprites[6];
            case '7': return numberSprites[7];
            case '8': return numberSprites[8];
            case '9': return numberSprites[9];
            case '.': return dotSprite;
            case '-': return minusSprite;
            case 'K': return kSprite;
            case 'M': return mSprite;
            default: return numberSprites[0]; // Fallback
        }
    }

    public void SetAlpha(float alpha)
    {
        Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        foreach (var renderer in digitRenderers)
        {
            if (renderer.gameObject.activeInHierarchy)
            {
                renderer.color = newColor;
            }
        }
    }
}