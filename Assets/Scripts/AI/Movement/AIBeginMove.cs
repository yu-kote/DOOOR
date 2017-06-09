using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class AIBeginMove : MonoBehaviour
{
    float _startMoveTime;

    public void Awake()
    {
        var ai_controller = GetComponent<AIController>();
        ai_controller.GetMovement().CanMove = false;
    }

    public void BeginMoveStart()
    {
        _startMoveTime = 2.0f;

        GetComponent<AIController>().StopMovement(_startMoveTime);

        StartCoroutine(EasePosition());

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
        {
            GetComponent<AIBeware>().IsBeware = true;
        }).AddTo(gameObject);
    }

    private IEnumerator EasePosition()
    {
        yield return null;
        var start_pos = GetComponent<AIController>().GetMovement().CurrentNode.transform.position;
        start_pos += new Vector3(0, transform.localScale.y, 0);

        EasingInitiator.Add(gameObject, start_pos,
                            _startMoveTime, EaseType.Linear);
    }
}
