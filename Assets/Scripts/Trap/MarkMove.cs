using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkMove : MonoBehaviour
{
    Vector3 _startPos;
    float _moveCount = 0.0f;

    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        _moveCount += 10 * Time.deltaTime;
        float y = Mathf.Sin(0.2f + 0.3f * _moveCount) * 0.3f;
        gameObject.transform.position = new Vector3(_startPos.x, _startPos.y + y, _startPos.z);
    }
}
