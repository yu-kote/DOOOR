using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearing : MonoBehaviour
{
    private AISoundManager _aiSoundManager;
    private NodeManager _nodeManager;
    private AIController _aiController;
    private GameObject _targetSound;
    private List<GameObject> _targetSounds = new List<GameObject>();


    void Start()
    {
        var field = GameObject.Find("Field");
        _aiSoundManager = field.GetComponent<AISoundManager>();
        _nodeManager = field.GetComponent<NodeManager>();
        _aiController = GetComponent<AIController>();
    }


    void Update()
    {
        if (GetComponent<AIChace>() != null)
            return;

        // 音が消えたら検索から除外する
        foreach (var target in _targetSounds)
        {
            if (target == null)
            {
                _targetSounds.Remove(target);
                return;
            }
        }


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

            if (Utility.PointToSphere(transform.position, p, r))
            {
                // 音を複数聞いた場合最後を目指す
                if (_targetSounds.Contains(sound.Value))
                    continue;
                _targetSounds.Add(sound.Value);

                // 音がなった位置から最も近いノードをもらう
                var node = _nodeManager.NearNode(p);
                // 目指すノードが今の位置だったらやめる
                if (node == null || node == _aiController.CurrentNode)
                    continue;
                
                if (GetComponent<AISearchMove>())
                    Destroy(GetComponent<AISearchMove>());
                if (GetComponent<AITargetMove>())
                    Destroy(GetComponent<AITargetMove>());

                var target_mover = gameObject.AddComponent<AITargetMove>();
                target_mover.SetTargetNode(node);
            }
        }
    }
}
