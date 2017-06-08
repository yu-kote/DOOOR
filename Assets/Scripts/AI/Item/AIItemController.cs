using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AIItemController : MonoBehaviour
{
    private List<ItemType> _haveItems = new List<ItemType>();
    public List<ItemType> HaveItems { get { return _haveItems; } set { _haveItems = value; } }

    [SerializeField]
    private ItemType[] _debugItemList;

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            _debugItemList[i] = ItemType.NONE;
        }
    }

    /// <summary>
    /// 獲得
    /// </summary>
    void AcquireItem(ItemType item)
    {
        _haveItems.Add(item);

        // デバッグ用に何を持っているか表示
        for (int i = 0; i < 5; i++)
        {
            if (_debugItemList[i] == ItemType.NONE)
            {
                _debugItemList[i] = item;
                break;
            }
        }
    }

    /// <summary>
    /// 引数のアイテムがあるかどうか
    /// </summary>
    public bool HaveItemCheck(ItemType type)
    {
        return _haveItems.Contains(type);
    }

    /// <summary>
    /// アイテムを使う
    /// </summary>
    public void UseItem(ItemType type)
    {
        var ai_controller = GetComponent<AIController>();
        GetComponent<VictimAnimation>().AnimStatus = VictimAnimationStatus.USE_ITEM;
        ai_controller.StopMovement(0.5f, () => GetComponent<VictimAnimation>().AnimStatus = VictimAnimationStatus.IDOL);
        _haveItems.Remove(type);
    }

    void Update()
    {
        ItemSearch();
    }


    void ItemSearch()
    {
        var ai_controller = GetComponent<AIController>();
        var node = ai_controller.CurrentNode;

        if (node == null)
            return;

        var itemstatus = node.GetComponent<ItemStatus>();
        if (itemstatus == null)
            return;

        var setting_items = itemstatus.GetItem();
        if (setting_items == ItemType.NONE)
            return;
        Func<ItemType, bool> Check =
            (type) => ((uint)setting_items & (uint)type) > 0;


        for (uint type = (uint)ItemType.TYENSO << 1; type > 0; type = type >> 1)
        {
            if (Check((ItemType)type))
            {
                itemstatus.AcquiredItem((ItemType)type);
                this.AcquireItem((ItemType)type);
            }
        }
    }
}
