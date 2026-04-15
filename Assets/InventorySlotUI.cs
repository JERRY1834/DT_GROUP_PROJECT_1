using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包物品槽UI - 代表背包中的一个物品
/// 挂载到物品槽预制体上
/// 预制体应包含: Image(图标) + Button(点击) + Text(物品名)
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    private Item item;
    private InventoryUIManager uiManager;
    private Button button;
    private Image icon;
    private Text itemName;

    private void Awake()
    {
        button = GetComponent<Button>();
        icon = GetComponent<Image>();
        itemName = GetComponentInChildren<Text>();
    }

    /// <summary>
    /// 初始化槽位
    /// </summary>
    public void Initialize(Item itemData, InventoryUIManager manager)
    {
        item = itemData;
        uiManager = manager;

        // 设置图标
        if (icon != null && item.Icon != null)
        {
            icon.sprite = item.Icon;
        }

        // 设置文字
        if (itemName != null)
        {
            itemName.text = item.ItemName;
        }

        // 按钮点击事件
        if (button != null)
        {
            button.onClick.AddListener(OnSlotClicked);
        }
    }

    /// <summary>
    /// 点击槽位 - 检出对应的实体到玩家面前
    /// </summary>
    private void OnSlotClicked()
    {
        // 从映射管理器检出物品对应的实体
        InventoryItemEntityManager entityMgr = InventoryItemEntityManager.Instance;
        if (entityMgr == null)
        {
            Debug.LogError("[InventorySlotUI] 致命错误：无法获取 InventoryItemEntityManager 实例");
            return;
        }

        GameObject entity = entityMgr.CheckoutItem(item);
        if (entity == null)
        {
            Debug.LogWarning($"[InventorySlotUI] 无法检出物品（可能未正确注册）: {item.ItemName}");
            return;
        }

        // 为该实体设置回调，以便后续的成功/失败交互
        SetupEntityCallbacks(entity, item);

        // 关闭背包 UI，让玩家可以交互
        uiManager.Close();

        Debug.Log($"[InventorySlotUI] 已检出物品: {item.ItemName}");
    }

    /// <summary>
    /// 为实体设置交互回调
    /// </summary>
    private void SetupEntityCallbacks(GameObject entity, Item item)
    {
        // 如果是钟表零件
        ClockPart clockPart = entity.GetComponent<ClockPart>();
        if (clockPart != null)
        {
            // 成功安装 → 删除物品
            clockPart.SetOnInstalledCallback(() =>
            {
                InventoryItemEntityManager entityMgr = InventoryItemEntityManager.Instance;
                if (entityMgr != null)
                {
                    entityMgr.RemoveItem(item);
                    Debug.Log($"[InventorySlotUI] 物品成功交互，已从背包移除: {item.ItemName}");
                }
            });

            // 返回/失败 → 隐藏实体，保留背包物品
            clockPart.SetOnReturnedCallback(() =>
            {
                InventoryItemEntityManager entityMgr = InventoryItemEntityManager.Instance;
                if (entityMgr != null)
                {
                    entityMgr.ReturnItem(item);
                    Debug.Log($"[InventorySlotUI] 物品返回背包: {item.ItemName}");
                }
            });
        }
        else
        {
            // 普通物品或其他类型 - 可在此添加其他交互逻辑
            Debug.Log($"[InventorySlotUI] 检出物品（非特殊交互）: {item.ItemName}");
        }
    }

    public Item GetItem() => item;

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnSlotClicked);
        }
    }
}
