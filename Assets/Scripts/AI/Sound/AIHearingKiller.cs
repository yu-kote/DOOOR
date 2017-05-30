using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIHearingKiller : AIHearing
{
    void Start()
    {
        Setup();
    }

    void Update()
    {
        //var target_moves = GetComponents<AITargetMove>();
        //if (target_moves.Count() > 1)
        //    Destroy(GetComponent<AITargetMove>());

        // 追っている時は聞こえなくする
        if (GetComponent<AIChace>() != null)
            return;

        if (GetComponent<AISearchMove>() ||
            GetComponent<AITargetMove>() ||
            _aiController.MoveMode == AIController.MoveEmotion.REACT_SOUND)
        {
            TargetSoundRemove();
            Hearing();
        }
    }

    protected override void HearingAction()
    {
        if (GetComponent<AISearchMove>())
            Destroy(GetComponent<AISearchMove>());
        if (GetComponent<AITargetMove>())
            Destroy(GetComponent<AITargetMove>());
        _roadPathManager.RoadGuideReset(gameObject);

        var target_mover = gameObject.AddComponent<AITargetMove>();
        target_mover.SetTargetNode(_hearNode);

        _aiController.MoveMode = AIController.MoveEmotion.REACT_SOUND;
    }
}
