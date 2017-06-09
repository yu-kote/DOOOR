using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerSound : MonoBehaviour
{
    private KillerAnimation _killerAnimation;
    private KillerAnimationStatus _currentAnimStatus;

    void Start()
    {
        _killerAnimation = GetComponent<KillerAnimation>();
    }

    void Update()
    {
        if (_currentAnimStatus == _killerAnimation.AnimStatus)
            return;
        _currentAnimStatus = _killerAnimation.AnimStatus;

        SoundManager.Instance.StopSE(gameObject);
        switch (_currentAnimStatus)
        {
            case KillerAnimationStatus.IDOL:
                break;
            case KillerAnimationStatus.WALK:
                Walk();
                break;
            case KillerAnimationStatus.RUN:
                Run();
                LookAt();
                break;
        }
    }

    void Walk()
    {
        SoundManager.Instance.PlaySE("satuzinkiaruki", gameObject, true);
    }

    void Run()
    {
        SoundManager.Instance.PlaySE("satuzinnkihasiri", gameObject, true);
    }

    void LookAt()
    {
        SoundManager.Instance.PlaySE("mitukeru", gameObject);
    }
}
