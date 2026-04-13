using UnityEditor;
using UnityEngine;

public class BuildingBoundaries : MonoBehaviour
{
    SimpleShowHideAnimator animator;
    [SerializeField] SpriteRenderer atkRangeRenderer;

    private void Awake()
    {
        animator = GetComponent<SimpleShowHideAnimator>();
    }

    public void SetVisibility(bool visible)
    {
        if (visible)
        {
            animator.Show();
        }
        else
        {
            animator.Hide();
        }
    }

    public void SetAtkRangeSize(float range)
    {
        range *= ProjectConstants.ATK_RANGE_MULTIPLIER;
        atkRangeRenderer.size = new Vector2(range, range);
    }
}