using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearing : MonoBehaviour
{
    private AISoundManager _aiSoundManager;
    private NodeManager _nodeManager;
    private AIController _aiController;
    private List<Node> _targetNode = new List<Node>();
    //private GameObject _targetSound;

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
        //if (GetComponent<AISearchMove>())

        foreach (var sound in _aiSoundManager.AISounds)
        {
            if (sound.Key == gameObject)
                continue;
            var p = sound.Value.gameObject.transform.position;
            var r = sound.Value.GetComponent<AISound>().Range / 2;

            if (Utility.PointToCircle(transform.position, p, r))
            {
                // 音がなった位置から最も近いノードをもらう
                var node = _nodeManager.NearNode(p);
                // 目指すノードが今の位置だったらやめる
                if (node == null || node == _aiController.CurrentNode)
                    continue;

                if (_targetNode.Contains(_aiController.CurrentNode))
                    continue;

                _targetNode.Add(node);

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
