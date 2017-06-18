using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AIBeginMove : MonoBehaviour
{
    float _startMoveTime;

    bool _isSearchStart;
    public bool IsSearchStart { get { return _isSearchStart; } }

    public void Awake()
    {
        var ai_controller = GetComponent<AIController>();
        ai_controller.GetMovement().CanMove = false;
        _isSearchStart = false;
    }

    public void BeginMoveStart()
    {
        _isSearchStart = true;
        _startMoveTime = 2.0f;

        GetComponent<AIController>().StopMovement(_startMoveTime);
        StartCoroutine(EasePosition());
        CallBack(_startMoveTime, () => GetComponent<AIBeware>().IsBeware = true);
    }

    protected void CallBack(float time, Action action)
    {
        StartCoroutine(CallBackAction(time, action));
    }

    private IEnumerator CallBackAction(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    private IEnumerator EasePosition()
    {
        yield return null;
        var current_node = GetComponent<AIController>().GetMovement().CurrentNode;
        var start_pos = current_node.transform.position;
        start_pos += new Vector3(0, transform.localScale.y, 0);

        EasingInitiator.Add(gameObject, start_pos,
                            _startMoveTime, EaseType.Linear);

        var deguti = current_node.GetComponent<Deguti>();
        if (deguti == null)
            yield break;

        deguti.StartOpening();
        yield return new WaitForSeconds(_startMoveTime);
        deguti.StartClosing();
        Destroy(this);
    }
}
