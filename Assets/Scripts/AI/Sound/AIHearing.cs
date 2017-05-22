using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearing : MonoBehaviour
{
    private AISoundManager _aiSoundManager;
    private NodeManager _nodeManager;
    private AIController _aiController;

    void Start()
    {
        var field = GameObject.Find("Field");
        _aiSoundManager = field.GetComponent<AISoundManager>();
        _nodeManager = field.GetComponent<NodeManager>();
        _aiController = GetComponent<AIController>();
    }

    /// <summary>
    /// 殺人鬼しか耳を持たない
    /// 自分の音は反応しない
    /// 他人の音が殺人鬼だった場合
    /// 音を鳴らした場合、音を鳴らした位置に行く
    /// 探索者を探している場合しか音の判定をしない
    /// 音の方向に向かっている場合他の音は聞かない
    /// </summary>
    void Update()
    {
        if (GetComponent<AIChace>() != null)
            return;
        if (_aiController.MoveMode == AIController.MoveEmotion.HURRY_UP)
            return;

        foreach (var sound in _aiSoundManager.AISounds)
        {
            if (sound.Key == gameObject)
                continue;
            var p = sound.Value.gameObject.transform.position;
            var r = sound.Value.GetComponent<AISound>().Range / 2;

            if (Utility.PointToCircle(transform.position, p, r))
            {
                Debug.Log("hit :" + tag);

                var node = _nodeManager.NearNode(p);
                if (node == null)
                    continue;
                Debug.Log(node);
                _aiController.MoveMode = AIController.MoveEmotion.HURRY_UP;
                StartCoroutine(TargetMoveStart(node));
            }
        }
    }

    private IEnumerator TargetMoveStart(Node node)
    {
        var target_node = node;
        while (true)
        {
            yield return null;
            var mover = _aiController.GetMovement();
            if (mover.MoveComplete() == false)
                continue;

            if (GetComponent<AISearchMove>())
                Destroy(GetComponent<AISearchMove>());
            if (GetComponent<AITargetMove>())
                Destroy(GetComponent<AITargetMove>());

            var target_mover = gameObject.AddComponent<AITargetMove>();
            target_mover.SetTargetNode(target_node);
            _aiController.MoveMode = AIController.MoveEmotion.HURRY_UP;

            break;
        }
    }
}
