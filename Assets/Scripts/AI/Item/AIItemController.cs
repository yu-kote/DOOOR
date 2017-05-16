using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        var acquired_item = itemstatus.AcquiredItem(ItemType.KEY);
        if (acquired_item == ItemType.NONE)
            acquired_item = itemstatus.AcquiredItem(ItemType.LASTKEY);

        if (acquired_item == ItemType.NONE)
            return;

        this.AcquireItem(acquired_item);
    }
}
