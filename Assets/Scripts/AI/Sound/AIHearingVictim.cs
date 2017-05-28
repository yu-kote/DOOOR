using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearingVictim : AIHearing
{
    void Start()
    {
        Setup();
    }

    void Update()
    {
        // 逃げてる時は聞こえなくする
        //if (GetComponent<AIRunAway>() != null)
        //return;

        TargetSoundRemove();
        Hearing();
    }

    protected override void HearingAction()
    {
        if (GetComponent<AISearchMove>())
            Destroy(GetComponent<AISearchMove>());
        if (GetComponent<AITargetMove>())
            Destroy(GetComponent<AITargetMove>());
        if(GetComponent<AIRunAway>())
            Destroy(GetComponent<AIRunAway>());
        
        var target_mover = gameObject.AddComponent<AIRunAway>();
        target_mover.SetTargetNode(_hearNode);
    }
}
