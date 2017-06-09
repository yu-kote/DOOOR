using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIHearing : MonoBehaviour
{
    /// <summary>
    /// 音が聞こえたら何かしらさせる
    /// </summary>
    protected abstract void HearingAction();

    protected AISoundManager _aiSoundManager;
    protected NodeManager _nodeManager;
    protected AIController _aiController;
    protected RoadPathManager _roadPathManager;

    protected GameObject _targetSound;
    protected List<GameObject> _targetSounds = new List<GameObject>();

    [SerializeField]
    protected float _hearingRange = 1.0f;

    /// <summary>
    /// 音がなった場所から一番近いノード
    /// </summary>
    protected Node _hearNode;

    protected void Setup()
    {
        var field = GameObject.Find("Field");
        _aiSoundManager = field.GetComponent<AISoundManager>();
        _nodeManager = field.GetComponent<NodeManager>();
        _aiController = GetComponent<AIController>();
        _roadPathManager = field.GetComponent<RoadPathManager>();
    }

    /// <summary>
    /// 音が消えたら検索から除外する
    /// </summary>
    protected void TargetSoundRemove()
    {
        // 音が消えたら検索から除外する
        foreach (var target in _targetSounds)
        {
            if (target == null)
            {
                _targetSounds.Remove(target);
                return;
            }
        }

        foreach (var target in _targetSounds)
        {
            if (target == null)
                continue;

            var p = target.gameObject.transform.position;
            var r = target.GetComponent<AISound>().Range / 2;

            // 聞こえる範囲に差をつける
            r += _hearingRange;
            if (Utility.PointToSphere(transform.position, p, r))
                continue;
            _targetSounds.Remove(target);
            break;
        }
    }

    protected void Hearing()
    {
        foreach (var sound in _aiSoundManager.AISounds)
        {
            if (sound.Value == null || sound.Key == null)
                continue;
            if (sound.Value.GetComponent<AISound>() == null)
                continue;

            // 自分が発している音には反応しないようにする
            if (sound.Key == gameObject)
                continue;
            var p = sound.Value.gameObject.transform.position;
            var r = sound.Value.GetComponent<AISound>().Range / 2;

            // 聞こえる範囲に差をつける
            r += _hearingRange;

            if (Utility.PointToSphere(transform.position, p, r))
            {
                // 音を複数聞いた場合最後に聞いた音に反応する
                if (_targetSounds.Contains(sound.Value))
                    continue;
                _targetSounds.Add(sound.Value);

                // 音がなった位置から最も近いノードをもらう
                var node = _nodeManager.NearNode(p);
                // 音の位置が今いるノードだったらはじく
                if (node == null || node == _aiController.CurrentNode)
                    continue;
                _hearNode = node;

                HearingAction();
                break;
            }
        }
    }

    private void OnDestroy()
    {
        _targetSounds.Clear();
    }
}
