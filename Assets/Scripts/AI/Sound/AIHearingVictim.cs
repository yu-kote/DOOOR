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
        var run_aways = GetComponents<AIRunAway>();
        if (run_aways.Count() > 1)
            Destroy(GetComponent<AIRunAway>());


        // 逃げてる時は聞こえなくする
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
        if (GetComponent<AIRunAway>())
            Destroy(GetComponent<AIRunAway>());

        var target_mover = gameObject.AddComponent<AIRunAway>();
        target_mover.SetTargetNode(_hearNode);

        _aiController.MoveMode = AIController.MoveEmotion.REACT_SOUND;
    }
}
