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
    /// 点击槽位 - 直接使用物品
    /// </summary>
    private void OnSlotClicked()
    {
        ItemConsumer mechanism = uiManager.GetCurrentMechanism();
        
        if (mechanism != null)
        {
            // 有打开背包的机关存在，尝试使用物品
            mechanism.UseItem(item);
        }
        else
        {
            // 没有机关在使用，只是查看物品
            Debug.Log($"查看物品: {item.ItemName}");
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
