using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIBeware : MonoBehaviour
{
    private RoadPathManager _roadPathManager;

    [SerializeField]
    private int _searchLimit = 5;
    public int SearchLimit { get { return _searchLimit; } set { _searchLimit = value; } }

    private int _searchCount;
    //private bool _isFindHuman = false;
    private GameObject _targetHuman;

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();

        StartCoroutine(Search());
    }

    private IEnumerator Search()
    {
        while (true)
        {
            yield return null;

            // 普通の移動をしている場合しか周囲を見ない
            if (GetComponent<AISearchMove>() == null)
                continue;

            // 標的が見つかっているかどうか
            if (_targetHuman == null)
            {
                // 探す
                var find_humans = SearchHuman(GetComponent<AIController>().CurrentNode);
                // 探した跡を消す
                _roadPathManager.RoadPathReset(gameObject);
                // 見つけたかどうかと、先頭があるかどうか
                if (find_humans != null && find_humans.First() != null)
                {
                    var find_human = find_humans.First();
                    // 見つけた場合は標的にする
                    _targetHuman = find_human;
                }
            }

            // ノード間の移動が終わっているかどうか(これがないと角で曲がるとき貫通する)
            //if (GetComponent<AIController>().GetMovement().MoveComplete() == false)
            //continue;
            // 標的が見つかっているかどうか
            if (_targetHuman == null)
                continue;
            if (CanMove(_targetHuman) == false)
                continue;

            // 殺人鬼の場合
            if (gameObject.tag == "Killer")
            {
                // 目標地点を目指している途中ならはじく
                if (GetComponent<AITargetMove>() != null)
                    continue;

                var mover = gameObject.AddComponent<AITargetMove>();

                // どこを目指すかを教える
                mover.SetTargetNode(_targetHuman.GetComponent<AIController>().CurrentNode);
                mover.Speed = GetComponent<AIController>().HurryUpSpeed;

                // 普通の移動をしていたら普通の移動をやめる
                if (GetComponent<AISearchMove>())
                    Destroy(GetComponent<AISearchMove>());
            }
            // 犠牲者の場合
            if (gameObject.tag == "Victim")
            {
                // 逃げている最中だったらはじく
                if (GetComponent<AIRunAway>() != null)
                    continue;

                var mover = gameObject.AddComponent<AIRunAway>();

                // どいつから逃げなければいけないかを教える
                mover.SetTargetNode(_targetHuman);
                mover.Speed = GetComponent<AIController>().HurryUpSpeed;

                // 普通の移動をしていたらやめる
                if (GetComponent<AISearchMove>())
                    Destroy(GetComponent<AISearchMove>());

            }
            _targetHuman = null;
        }
    }

    List<GameObject> SearchHuman(Node current_node)
    {
        _searchCount++;
        if (_searchCount > _searchLimit)
        {
            _searchCount = 0;
            return null;
        }

        var humans = current_node.gameObject.GetComponent<FootPrint>().HumansOnNode;
        if (!humans.Contains(gameObject) &&
            humans.Count > 0)
        {
            foreach (var human in humans)
            {
                if (human == null) continue;
                if (gameObject.tag == "Victim")
                    if (human.tag == "Victim")
                        continue;
                return humans;
            }
        }

        var loadpath = current_node.gameObject.GetComponent<RoadPath>();

        foreach (var node in current_node.LinkNodes)
        {
            // 検索済みは飛ばし
            if (node.gameObject.GetComponent<RoadPath>().PathCheck(gameObject) == true)
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
                var stairs_humans = node.gameObject.GetComponent<FootPrint>().HumansOnNode;
                if (!stairs_humans.Contains(gameObject) &&
                     stairs_humans.Count > 0)
                    return stairs_humans;
                continue;
            }

            loadpath.Add(gameObject, node);


            var found_human = SearchHuman(node);
            if (found_human != null)
                return found_human;

            // 角の場合は終了
            if (node.gameObject.GetComponent<Corner>())
                return null;
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

    void Update()
    {

    }
}
