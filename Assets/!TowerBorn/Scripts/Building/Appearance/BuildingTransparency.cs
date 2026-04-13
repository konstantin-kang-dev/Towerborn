using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshRendererMaterialData
{
    public MeshRenderer meshRenderer;
    public Material[] originalMaterials;

    public MeshRendererMaterialData(MeshRenderer meshRenderer)
    {
        this.meshRenderer = meshRenderer;
        // Используем sharedMaterials для избежания создания копий материалов
        this.originalMaterials = new Material[meshRenderer.sharedMaterials.Length];
        System.Array.Copy(meshRenderer.sharedMaterials, this.originalMaterials, meshRenderer.sharedMaterials.Length);
    }
}

public class BuildingTransparency : MonoBehaviour
{
    [Header("Настройки прозрачности")]
    [SerializeField] private Material transparentMaterial;

    [Header("Настройки поиска")]
    [SerializeField] private bool includeInactive = true;
    [SerializeField] private bool searchInChildren = true;

    private List<MeshRendererMaterialData> rendererData = new List<MeshRendererMaterialData>();
    private bool isTransparent = false;

    private Material[] tempTransparentMaterials;

    void Awake()
    {
        CollectMeshRenderers();
    }

    public void CollectMeshRenderers()
    {
        rendererData.Clear();

        MeshRenderer[] meshRenderers;

        if (searchInChildren)
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>(includeInactive);
        }
        else
        {
            var singleRenderer = GetComponent<MeshRenderer>();
            meshRenderers = singleRenderer != null ? new MeshRenderer[] { singleRenderer } : new MeshRenderer[0];
        }

        foreach (var meshRenderer in meshRenderers)
        {
            if (IsValidMeshRenderer(meshRenderer))
            {
                rendererData.Add(new MeshRendererMaterialData(meshRenderer));
            }
        }

        //Debug.Log($"Собрано {rendererData.Count} MeshRenderer'ов для постройки {gameObject.name}");
    }

    private bool IsValidMeshRenderer(MeshRenderer meshRenderer)
    {
        return meshRenderer != null &&
               meshRenderer.sharedMaterials != null &&
               meshRenderer.sharedMaterials.Length > 0;
    }

    public void MakeTransparent()
    {
        if (isTransparent || transparentMaterial == null)
        {
            return;
        }

        foreach (var data in rendererData)
        {
            if (data.meshRenderer == null) continue;

            ApplyTransparentMaterial(data);
        }

        isTransparent = true;
    }

    private void ApplyTransparentMaterial(MeshRendererMaterialData data)
    {
        int materialCount = data.originalMaterials.Length;

        if (tempTransparentMaterials == null || tempTransparentMaterials.Length != materialCount)
        {
            tempTransparentMaterials = new Material[materialCount];
        }

        for (int i = 0; i < materialCount; i++)
        {
            tempTransparentMaterials[i] = transparentMaterial;
        }

        data.meshRenderer.materials = tempTransparentMaterials;
    }

    public void RestoreOriginalMaterials()
    {
        if (!isTransparent)
        {
            return;
        }

        foreach (var data in rendererData)
        {
            if (data.meshRenderer != null)
            {
                data.meshRenderer.materials = data.originalMaterials;
            }
        }

        isTransparent = false;
    }

    public void ToggleTransparency()
    {
        if (isTransparent)
        {
            RestoreOriginalMaterials();
        }
        else
        {
            MakeTransparent();
        }
    }

    public void SetTransparentMaterial(Material newTransparentMaterial)
    {
        if (newTransparentMaterial == transparentMaterial)
        {
            return;
        }

        bool wasTransparent = isTransparent;

        if (isTransparent)
        {
            RestoreOriginalMaterials();
        }

        transparentMaterial = newTransparentMaterial;

        if (wasTransparent && transparentMaterial != null)
        {
            MakeTransparent();
        }
    }
    public bool IsTransparent => isTransparent;

    public void RefreshMeshRenderers()
    {
        bool wasTransparent = isTransparent;

        if (isTransparent)
        {
            RestoreOriginalMaterials();
        }

        CollectMeshRenderers();

        if (wasTransparent && transparentMaterial != null)
        {
            MakeTransparent();
        }
    }

    public void AddMeshRenderer(MeshRenderer meshRenderer)
    {
        if (!IsValidMeshRenderer(meshRenderer))
        {
            return;
        }

        foreach (var data in rendererData)
        {
            if (data.meshRenderer == meshRenderer)
            {
                return;
            }
        }

        var newData = new MeshRendererMaterialData(meshRenderer);
        rendererData.Add(newData);

        if (isTransparent && transparentMaterial != null)
        {
            ApplyTransparentMaterial(newData);
        }
    }

    public void RemoveMeshRenderer(MeshRenderer meshRenderer)
    {
        for (int i = rendererData.Count - 1; i >= 0; i--)
        {
            if (rendererData[i].meshRenderer == meshRenderer)
            {
                // Восстанавливаем материалы перед удалением, если объект прозрачный
                if (isTransparent && rendererData[i].meshRenderer != null)
                {
                    rendererData[i].meshRenderer.materials = rendererData[i].originalMaterials;
                }

                rendererData.RemoveAt(i);
                return;
            }
        }
    }

    public int GetMeshRenderersCount() => rendererData.Count;

    public bool HasValidTransparentMaterial() => transparentMaterial != null;

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            CollectMeshRenderers();
        }
    }

    void OnDestroy()
    {
        if (isTransparent)
        {
            RestoreOriginalMaterials();
        }

        rendererData.Clear();
        tempTransparentMaterials = null;
    }

#if UNITY_EDITOR
    [ContextMenu("Сделать прозрачным")]
    private void ContextMakeTransparent()
    {
        MakeTransparent();
    }

    [ContextMenu("Восстановить материалы")]
    private void ContextRestoreOriginal()
    {
        RestoreOriginalMaterials();
    }

    [ContextMenu("Обновить MeshRenderer'ы")]
    private void ContextRefreshRenderers()
    {
        RefreshMeshRenderers();
    }
#endif
}