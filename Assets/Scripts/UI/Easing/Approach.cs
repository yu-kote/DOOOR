using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Approach : MonoBehaviour
{
    [SerializeField]
    private GameObject _target;
    public GameObject Target
    {
        get { return _target; }
        set { _target = value; }
    }

    [SerializeField]
    public float _speed = 0.05f;

    // ずらす値
    private Vector3 _offsetPos;
    public Vector3 OffsetPos
    {
        get { return _offsetPos; }
        set { _offsetPos = value; }
    }


    void Start()
    {
    }

    void Update()
    {
        var pos = transform.position;
        var t_pos = _target.transform.position;
        var distance = t_pos - pos - _offsetPos;
        pos += (distance * _speed);

        transform.position = pos;
    }
}
