using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimSound : MonoBehaviour
{
    private VictimAnimation _victimAnimation;
    private VictimAnimationStatus _currentAnimStatus;

    void Start()
    {
        _victimAnimation = GetComponent<VictimAnimation>();
        name = GetComponent<MyNumber>().Name;
    }

    void Update()
    {
        if (_currentAnimStatus == _victimAnimation.AnimStatus)
            return;
        _currentAnimStatus = _victimAnimation.AnimStatus;

        SoundManager.Instance.StopSE(gameObject);
        switch (_currentAnimStatus)
        {
            case VictimAnimationStatus.IDOL:
                break;
            case VictimAnimationStatus.WALK:
                Walk();
                break;
            case VictimAnimationStatus.RUN:
                Run();
                Shout();
                break;
            case VictimAnimationStatus.DEAD:
                Shout();
                break;
            case VictimAnimationStatus.OPEN_DOOR:
                break;
            case VictimAnimationStatus.STAGGER:
                break;
            case VictimAnimationStatus.CRISIS:
                break;
            case VictimAnimationStatus.USE_ITEM:
                break;
        }
    }

    void Walk()
    {
        if (name == "Woman")
            SoundManager.Instance.PlaySE("bijyoaruki", gameObject, true);
        if (name == "TallMan")
            SoundManager.Instance.PlaySE("noppoaruki", gameObject, true);
        if (name == "Fat")
            SoundManager.Instance.PlaySE("debuaruki", gameObject, true);
    }

    void Run()
    {
        if (name == "Woman")
            SoundManager.Instance.PlaySE("bijyohasiri", gameObject, true);
        if (name == "TallMan")
            SoundManager.Instance.PlaySE("noppohasiri", gameObject, true);
        if (name == "Fat")
            SoundManager.Instance.PlaySE("debuhasiri", gameObject, true);
    }

    void Shout()
    {
        if (name == "Woman")
            SoundManager.Instance.PlaySE("bijyohimei", gameObject);
        if (name == "TallMan")
            SoundManager.Instance.PlaySE("noppohimei", gameObject);
        if (name == "Fat")
            SoundManager.Instance.PlaySE("debuhimei", gameObject);
    }

}
