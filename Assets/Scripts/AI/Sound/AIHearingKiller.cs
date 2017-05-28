using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearingKiller : AIHearing
{
    void Start()
    {
        Setup();
    }

    void Update()
    {
        // 追っている時は聞こえなくする
        if (GetComponent<AIChace>() != null)
            return;

        TargetSoundRemove();
        Hearing();
    }

    protected override void HearingAction()
    {
        if (GetComponent<AISearchMove>())
            Destroy(GetComponent<AISearchMove>());
        if (GetComponent<AITargetMove>())
            Destroy(GetComponent<AITargetMove>());

        var target_mover = gameObject.AddComponent<AITargetMove>();
        target_mover.SetTargetNode(_hearNode);
    }
}
