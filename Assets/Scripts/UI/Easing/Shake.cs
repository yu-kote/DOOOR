using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{

    private Vector3 _startPos;
    public Vector3 StartPos { get { return _startPos; } set { _startPos = value; } }

    [SerializeField]
    private Vector3 _rangeMin = new Vector3(0, 0, 0);
    public Vector3 RangeMin { get { return _rangeMin; } set { _rangeMin = value; } }

    [SerializeField]
    private Vector3 _rangeMax = new Vector3(1, 1, 1);
    public Vector3 RangeMax { get { return _rangeMin; } set { _rangeMin = value; } }

    private bool _isShake;
    public bool IsShake { get { return _isShake; } set { _isShake = value; } }

    public void Start()
    {
        _isShake = true;
        _startPos = transform.position;
    }

    public void Stop()
    {
        _isShake = false;
    }

    public void SetRange(float min, float max)
    {
        SetRange(new Vector3(min, min, min), new Vector3(max, max, max));
    }

    public void SetRange(Vector3 min, Vector3 max)
    {
        _rangeMin = min;
        _rangeMax = max;
    }

    void Update()
    {
        if (_isShake == false)
            return;

        transform.position = _startPos;
        var random_move = new Vector3(Random.Range(_rangeMin.x, _rangeMax.x),
                                      Random.Range(_rangeMin.y, _rangeMax.y),
                                      Random.Range(_rangeMin.z, _rangeMax.z));
        transform.position += random_move;
    }
}
