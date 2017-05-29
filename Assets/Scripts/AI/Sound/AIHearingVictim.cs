using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        //    return;

        TargetSoundRemove();
        Hearing();
    }

    protected override void HearingAction()
    {
        if (GetComponent<AISearchMove>())
            Destroy(GetComponent<AISearchMove>());
        if (GetComponent<AITargetMove>())
            Destroy(GetComponent<AITargetMove>());
        if (GetComponents<AIRunAway>() != null)
            for (int i = 0; i < GetComponents<AIRunAway>().Count(); i++)
                Destroy(GetComponents<AIRunAway>()[i]);

        var target_mover = gameObject.AddComponent<AIRunAway>();
        target_mover.SetTargetNode(_hearNode);
    }
}
