using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIBeware : MonoBehaviour
{
    [SerializeField]
    private int _blackoutSearchLimit = 1;
    [SerializeField]
    private int _defaultSearchLimit = 5;
    [SerializeField]
    private int _LongSearchLimit = 10;

    private int _searchLimit = 5;
    public int SearchLimit { get { return _searchLimit; } set { _searchLimit = value; } }

    private RoadPathManager _roadPathManager;
    private int _searchCount;
    private GameObject _targetHuman;

    /// <summary>
    /// 周りを確認するかどうか
    /// </summary>
    private bool _isBeware;
    public bool IsBeware
    {
        get { return _isBeware; }
        set { _isBeware = value; }
    }

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _isBeware = false;
        StartCoroutine(Search());
    }

    private IEnumerator Search()
    {
        while (true)
        {
            yield return null;
            // ゲーム開始直後はロックする
            if (_isBeware == false)
                continue;
            // 懐中電灯を持っている間は探索距離が延びる
            if (tag == "Victim")
                if (GetComponent<AIItemController>().HaveItemCheck(ItemType.FLASHLIGHT))
                    _searchLimit = _defaultSearchLimit;
                else
                    _searchLimit = _LongSearchLimit;

            // 普通の移動をしている場合しか周囲を見ない
            var ai_controller = GetComponent<AIController>();
            if (ai_controller.MoveMode == AIController.MoveEmotion.HURRY_UP)
                continue;

            // 標的が見つかっているかどうか
            if (_targetHuman == null)
            {
                // 向いている方向しか探索しないため、後ろのノードを探索済みにする
                if (ai_controller.PrevNode)
                    ai_controller.PrevNode.GetComponent<NodeGuide>().AddSearch(gameObject);
                // 探す
                var find_humans = SearchHuman(ai_controller.CurrentNode);
                // 探した跡を消す
                _roadPathManager.SearchReset(gameObject);
                // 見つけたかどうかと、先頭があるかどうか
                if (find_humans != null && find_humans.FirstOrDefault())
                {
                    var find_human = find_humans.First();
                    // 見つけた場合は標的にする
                    _targetHuman = find_human;
                }
            }

            // 標的が見つかっているかどうか
            if (_targetHuman == null)
                continue;
            if (CanMove(_targetHuman) == false)
                continue;


            // 殺人鬼の場合
            if (gameObject.tag == "Killer")
            {
                // 普通の移動をしていたら普通の移動をやめる
                if (GetComponent<AISearchMove>())
                    Destroy(GetComponent<AISearchMove>());
                if (GetComponent<AITargetMove>())
                    Destroy(GetComponent<AITargetMove>());

                // どこを目指すかを教える
                var mover = gameObject.AddComponent<AIChace>();
                mover.SetTargetNode(_targetHuman.GetComponent<AIController>().CurrentNode);
                mover.SetTargetHuman(_targetHuman);
            }
            // 犠牲者の場合
            if (gameObject.tag == "Victim")
            {
                // 他のの移動をしていたらやめる
                if (GetComponent<AISearchMove>())
                    Destroy(GetComponent<AISearchMove>());
                if (GetComponent<AITargetMove>())
                    Destroy(GetComponent<AITargetMove>());
                if (GetComponent<AIChace>())
                    Destroy(GetComponent<AIChace>());

                // どいつから逃げなければいけないかを教える
                var mover = gameObject.AddComponent<AIRunAway>();
                mover.SetTargetHuman(_targetHuman);
            }
            // 急ぐ
            ai_controller.MoveMode = AIController.MoveEmotion.HURRY_UP;
            _targetHuman = null;
        }
    }

    // 自分と違う人間を探す
    List<GameObject> SearchHuman(Node current_node)
    {
        _searchCount++;
        if (_searchCount > _searchLimit)
        {
            _searchCount = 0;
            return null;
        }

        if (current_node == null) return null;
        var humans = current_node.gameObject.GetComponent<FootPrint>().HumansOnNode;

        if (!humans.Contains(gameObject) &&
            humans.Count > 0)
        {
            var human = SearchHumanOnNode(humans, "Victim");
            if (human != null)
                return human;
        }

        // 角の場合は終了
        if (current_node.gameObject.GetComponent<Corner>())
            if (current_node != GetComponent<AIController>().CurrentNode)
                return null;

        var loadpath = current_node.gameObject.GetComponent<NodeGuide>();

        foreach (var node in current_node.LinkNodes)
        {
            // 検索済みは飛ばし
            if (node.gameObject.GetComponent<NodeGuide>().SearchCheck(gameObject))
                continue;
            // 壁は探索しない
            if (node.gameObject.GetComponent<Wall>())
                continue;
            // 殺人鬼は扉の向こうを見れない
            if (tag == "Killer")
                if (node.gameObject.GetComponent<Door>())
                    continue;
            // ほかの階は探索しない
            if (current_node.gameObject.GetComponent<Stairs>() &&
                node.gameObject.GetComponent<Stairs>())
            {
                if (tag != "Killer")
                {
                    var stairs_humans = node.gameObject.GetComponent<FootPrint>().HumansOnNode;
                    if (!stairs_humans.Contains(gameObject) &&
                         stairs_humans.Count > 0)
                    {
                        var human = SearchHumanOnNode(stairs_humans, "Victim");
                        if (human != null)
                            return human;
                    }
                }
                continue;
            }

            loadpath.AddSearch(gameObject);

            var found_human = SearchHuman(node);
            if (found_human != null)
                return found_human;
        }
        _searchCount = 0;
        return null;
    }

    bool CanMove(GameObject human)
    {
        if (tag == "Killer")
        {
            var human_node = human.GetComponent<AIController>().CurrentNode;
            if (human_node.GetComponent<Door>() != null)
                return false;
        }
        return true;
    }

    // 犠牲者なら犠牲者以外、殺人鬼なら殺人鬼以外がノードにいるか探す
    List<GameObject> SearchHumanOnNode(List<GameObject> human_on_node, string exclude_tag = "")
    {
        foreach (var human in human_on_node)
        {
            if (human == null) continue;
            if (gameObject.tag == exclude_tag)
                if (human.tag == exclude_tag)
                    continue;
            return human_on_node;
        }
        return null;
    }

    void Update()
    {

    }
}
