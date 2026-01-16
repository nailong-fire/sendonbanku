using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject inventoryUI;   // Canvas/InventoryUI
    public Transform cardGrid;       // Canvas/InventoryUI/CardGrid

    [Header("Prefab")]
    public InventoryCardItem itemPrefab; // InventoryCardItem.prefab

    [Header("Data")]
    public CardDatabaseSO cardDatabase;

    private bool isOpen;
    private readonly List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        if (inventoryUI != null) inventoryUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        isOpen = !isOpen;

        if (inventoryUI != null)
            inventoryUI.SetActive(isOpen);

        if (isOpen) Refresh();
        else Clear();
    }

    void Refresh()
    {
        Clear();

        if (cardDatabase == null)
        {
            Debug.LogError("[InventoryUI] cardDatabase 未绑定");
            return;
        }
        if (cardGrid == null)
        {
            Debug.LogError("[InventoryUI] cardGrid 未绑定");
            return;
        }
        if (itemPrefab == null)
        {
            Debug.LogError("[InventoryUI] itemPrefab 未绑定");
            return;
        }

        // ✅ 按你的 CardDatabaseSO：用 playerOwnedCardIds
        var ids = cardDatabase.playerOwnedCardIds;
        for (int i = 0; i < ids.Count; i++)
        {
            var so = cardDatabase.GetCardById(ids[i]);
            if (so == null) continue;

            var item = Instantiate(itemPrefab, cardGrid);
            item.Setup(so);

            spawned.Add(item.gameObject);
        }
    }

    void Clear()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if (spawned[i] != null) Destroy(spawned[i]);
        }
        spawned.Clear();
    }
}
