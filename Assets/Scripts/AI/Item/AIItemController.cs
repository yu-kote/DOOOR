using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIItemController : MonoBehaviour
{
    private List<ItemType> _haveItems;
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
    /// アイテムを獲得する
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

    }
}
