using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public class AIChace : AITargetMove
{
    private int _endNodeDistance = 7;

    private GameObject _targetHuman;
    public GameObject TargetHuman { get { return _targetHuman; } set { _targetHuman = value; } }
    public void SetTargetHuman(GameObject target_node) { _targetHuman = target_node; }

    private bool _isChaceEnd;

    void Start()
    {
        // From RouteSearch
        RouteSearchSetup();
        // From TargetMove
        MoveSetup();

        // 道を可視化してみる
        //StartCoroutine(SearchRoadTestDraw(_currentNode));
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;
        _isChaceEnd = false;

        MoveReset();
    }

    Node ApproachNode()
    {
        if (Search())
            return _currentNode.GetComponent<NodeGuide>().NextNode(gameObject);
        return null;
    }

    bool Chace()
    {
        if (_isChaceEnd)
            return true;
        if (_targetHuman == null)
            return true;

        _targetNode = _targetHuman.GetComponent<AIController>().CurrentNode;

        Node next_node = null;
        next_node = ApproachNode();
        _roadPathManager.RoadGuideReset(gameObject);

        if (SearchCount > _endNodeDistance)
            return true;

        // 移動できるかどうか
        if (next_node == null)
            return false;

        // 壁だったら進めない
        if (next_node.GetComponent<Wall>() != null)
            return false;

        // 階段がロックされていたら通れない
        if (IsStairsLock(next_node))
            return false;

        // ドアだったら探索終わり
        var door = next_node.GetComponent<Door>();
        if (door)
            return true;
        if (_currentNode.GetComponent<Door>())
            return true;

        _nextNode = next_node;
        PrevNodeUpdate();

        return false;
    }

    protected override void NextNodeSearch()
    {
        _isChaceEnd = Chace();
    }

    void Update()
    {
        KillTarget();
        ChaceEnd();
    }

    void ChaceEnd()
    {
        if (_targetMoveEnd) return;

        if (_isChaceEnd)
        {
            _targetMoveEnd = true;

            if (IsDoorAround())
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                {
                    if (_isChaceEnd)
                        SearchMoveStart();
                }).AddTo(gameObject);
                return;
            }
            SearchMoveStart();
        }
    }

    void KillTarget()
    {
        if (CanMove == false)
            return;
        if (tag != "Killer")
            return;
        var humans = _currentNode.GetComponent<FootPrint>().HumansOnNode;
        if (humans.Count < 2) return;

        foreach (var human in humans)
        {
            if (human == null) continue;
            if (human.tag != "Victim") continue;
            if (_currentNode.GetComponent<Stairs>() &&
                human.GetComponent<AIController>().GetMovement().MoveComplete() == false)
                continue;
            // 対象が階段上にいる場合は殺せなくなる
            var target_node = human.GetComponent<AIController>().CurrentNode;
            if (target_node)
                if (target_node.GetComponent<Stairs>())
                    continue;
            // 追っている目標じゃなかったら殺さない
            if (_targetHuman != human)
                continue;

            var item_controller = human.GetComponent<AIItemController>();
            // 迎撃するアイテムを持っていたら迎撃される
            if (item_controller.HaveItemCheck(ItemType.GUN))
            {
                item_controller.UseItem(ItemType.GUN, gameObject);
                SoundManager.Instance.PlaySE("handogan", human.gameObject);

                // 攻撃されたたときのアニメーションに切り替える
                GetComponent<KillerAnimation>().AnimStatus = KillerAnimationStatus.HIT;
                GetComponent<AIController>()
                    .StopMovement(1.5f, () => GetComponent<KillerAnimation>().AnimStatus = KillerAnimationStatus.IDOL);
                break;
            }
            else if (item_controller.HaveItemCheck(ItemType.TYENSO))
            {
                item_controller.UseItem(ItemType.TYENSO, gameObject);
                SoundManager.Instance.PlaySE("tye-nso-", human.gameObject);

                // 攻撃されたたときのアニメーションに切り替える
                GetComponent<KillerAnimation>().AnimStatus = KillerAnimationStatus.HIT;
                GetComponent<AIController>()
                    .StopMovement(1.5f, () => GetComponent<KillerAnimation>().AnimStatus = KillerAnimationStatus.IDOL);
                break;
            }
            _targetHuman = null;

            human.GetComponent<AIController>().StopMovement(2);
            GetComponent<KillerAnimation>().AnimStatus = KillerAnimationStatus.IDOL;
            GetComponent<AIController>()
                .StopMovement(0.1f, () =>
                {
                    GetComponent<KillerAnimation>().AnimStatus = KillerAnimationStatus.IDOL;

                    human.GetComponent<AIController>().BeKilled();
                    GetComponent<KillerAnimation>().KillAnimation();

                    SoundManager.Instance.PlaySE("korosu");
                });
            break;
        }
    }

    // 周囲にドアがあるかどうか
    bool IsDoorAround()
    {
        foreach (var node in _currentNode.LinkNodes)
        {
            if (tag == "Killer" && node.GetComponent<Door>())
                return true;
        }
        return false;
    }
}
