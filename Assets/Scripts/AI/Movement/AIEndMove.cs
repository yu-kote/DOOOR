using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEndMove : MonoBehaviour
{
    float _endMoveTime;

    void Start()
    {
        _endMoveTime = 2.0f;

        StartCoroutine(EndMove());
    }

    private IEnumerator EndMove()
    {
        yield return new WaitForSeconds(2.0f);
        var exit_node = GetComponent<AIController>().CurrentNode;
        var offset = new Vector3(0, 1, 0);

        EasingInitiator.Add(gameObject, exit_node.transform.position + offset,
                            _endMoveTime, EaseType.Linear);

        yield return new WaitForSeconds(_endMoveTime);

        var end_pos = transform.localPosition + new Vector3(0, 0, -3);
        EasingInitiator.Add(gameObject, end_pos, _endMoveTime, EaseType.Linear);
    }

    private void Update()
    {
        gameObject.transform.eulerAngles = Vector3.zero;
    }
}
