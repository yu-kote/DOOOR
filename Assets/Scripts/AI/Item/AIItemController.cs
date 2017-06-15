using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AIItemController : MonoBehaviour
{
    private List<ItemType> _haveItems = new List<ItemType>();
    public List<ItemType> HaveItems { get { return _haveItems; } set { _haveItems = value; } }

    // デバッグ用に表示
    [SerializeField]
    int _haveItemLimit;

    [SerializeField]
    private ItemType[] _itemList;

    private HumanList _boardList;

    void Start()
    {
        for (int i = 0; i < _itemList.Count(); i++)
        {
            _itemList[i] = ItemType.NONE;
        }
    }

    /// <summary>
    /// 獲得
    /// </summary>
    void AcquireItem(ItemType item)
    {
        if (item == ItemType.LASTKEY)
            SoundManager.Instance.PlaySE("kaginyuusyu");

        _haveItems.Add(item);

        // UIにゲットしたアイテムを表示する
        var board = _boardList.GetHumanBoard(GetComponent<MyNumber>().Number);
        board.SetItem(_boardList.GetItemSprite(item));
        // UIにアイテムをゲットしたときのアイコンを表示する
        board.SetIcon(_boardList.GetSprite(_boardList.GetIconNameFromBoardIcon(BoardIcon.ITEM_GET)));

        // デバッグ用に何を持っているか表示
        for (int i = 0; i < 5; i++)
        {
            if (_itemList[i] == ItemType.NONE)
            {
                _itemList[i] = item;
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
    public void UseItem(ItemType type, GameObject target = null)
    {
        if (type == ItemType.GUN || type == ItemType.TYENSO)
        {
            if (target)
                GetComponent<HumanAnimController>().Rotation(target.gameObject);
            GetComponent<VictimAnimation>().ChangeAnimation(VictimAnimationStatus.USE_ITEM, 1.5f);
        }

        _haveItems.Remove(type);

        var board = _boardList.GetHumanBoard(GetComponent<MyNumber>().Number);
        board.UseItem(_boardList.GetItemNameFromItemType(type));
    }

    float _itemEffectTime = 0.0f;
    /// <summary>
    /// アイテムを使い、指定時間後消滅させる
    /// </summary>
    public void UseItem(ItemType type, float time)
    {
        StartCoroutine(ItemEffect(type, time));
    }

    private IEnumerator ItemEffect(ItemType type, float time)
    {
        while (true)
        {
            yield return null;
            _itemEffectTime += Time.deltaTime;
            if (_itemEffectTime < time)
                continue;
            _itemEffectTime = 0.0f;
            UseItem(type);
            yield break;
        }
    }

    /// <summary>
    /// アイテムをUIと紐づける
    /// </summary>
    public void SetHumanList(HumanList board)
    {
        _boardList = board;
    }

    void Update()
    {
        // アイテムを探す
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

        // 所持数上限だったらはじく
        if (_haveItems.Count >= _haveItemLimit)
            return;

        var setting_item = itemstatus.GetItem();
        if (setting_item == ItemType.NONE)
            return;

        // すでに所持していたらはじく
        if (HaveItemCheck(setting_item))
            return;

        // そのアイテムを持てるかどうか調べる
        if (CanHaveItem(setting_item) == false)
            return;

        Func<ItemType, bool> Check =
            (type) => ((uint)setting_item & (uint)type) > 0;

        for (uint type = (uint)ItemType.TYENSO << 1; type > 0; type = type >> 1)
        {
            if (Check((ItemType)type))
            {
                itemstatus.AcquiredItem((ItemType)type);
                this.AcquireItem((ItemType)type);
            }
        }

        if (setting_item == ItemType.FLASHLIGHT)
        {
            float light_time = 30;
            UseItem(setting_item, light_time);
            gameObject.AddComponent<FlushLightEffect>().EffectTime = light_time;
        }
    }

    bool CanHaveItem(ItemType type)
    {
        var name = GetComponent<MyNumber>().Name;
        if (type == ItemType.FLASHLIGHT || type == ItemType.LASTKEY)
            return true;
        if (type == ItemType.GUN)
            if (name == "TallMan")
                return true;
        if (type == ItemType.TYENSO)
            if (name == "Fat")
                return true;
        return false;
    }

    private void OnDestroy()
    {
        _haveItems.Clear();
    }


}
