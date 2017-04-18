using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public abstract class AIBasicsMovement : MonoBehaviour
{
    public abstract void MoveSetup();
    protected abstract void NextNodeSearch();

    protected Node _currentNode;
    protected Node _nextNode = null;
    protected NodeController _nodeController;

    private float _speed = 0;
    public float Speed { get { return _speed; } set { _speed = value; } }

    private bool _canMove = true;
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }

    // デバッグ用にSerializeField
    [SerializeField]
    private Vector3 _moveDirection;
    [SerializeField]
    private Vector3 _moveLength;

    private IDisposable _updateDisposable;

    private void Awake()
    {
        StartCoroutine(StartSearch());
        StartCoroutine(MoveUpdate());
    }

    private IEnumerator StartSearch()
    {
        yield return null;
        NextNodeSearch();
    }

    private IEnumerator MoveUpdate()
    {
        yield return null;

        _updateDisposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                // 次の移動先が決まったら
                if (_nextNode != null && _nextNode != _currentNode)
                {
                    // 進む方向を決めるためベクトルを出す
                    var distance = _nextNode.transform.position - gameObject.transform.position + HeightCorrection();
                    _moveLength = new Vector3(Mathf.Abs(distance.x), Mathf.Abs(distance.z), Mathf.Abs(distance.z));

                    // 値が小さいほど速度の調整がしやすいので0.01fをかける
                    _moveDirection = distance * 0.01f * _speed;

                    // 移動先が決まった時に何かするときの関数
                    NextNodeDecided();

                    // 今いるノードを更新する
                    _currentNode = _nextNode;
                }
                Move();
            }).AddTo(this);
    }

    void Move()
    {
        if (_canMove == false) return;
        transform.position += _moveDirection;
        _moveLength -= Vector3Abs(_moveDirection);

        if (MoveComplete())
        {
            transform.position = _currentNode.transform.position + HeightCorrection();

            _moveDirection = Vector3.zero;
            _moveLength = Vector3.zero;
            NextNodeSearch();
        }
    }

    protected void MoveReset()
    {
        _nextNode = null;
        _moveDirection = Vector3.zero;
        _moveLength = Vector3.zero;
    }

    Vector3 HeightCorrection()
    {
        return new Vector3(0, transform.localScale.y, 0);
    }

    Vector3 Vector3Abs(Vector3 value)
    {
        return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.z), Mathf.Abs(value.z));
    }

    public bool MoveComplete()
    {
        return _moveLength.x <= 0 &&
            _moveLength.y <= 0 &&
            _moveLength.z <= 0;
    }

    public void AddFootPrint(Node node)
    {
        var foot_print = node.GetComponent<FootPrint>();
        foot_print.StepIn(gameObject);
    }

    public void LeaveFootPrint(Node node)
    {
        var foot_print = node.GetComponent<FootPrint>();
        foot_print.StepOut(gameObject);
    }

    protected void NextNodeDecided()
    {
        AddFootPrint(_nextNode);
        LeaveFootPrint(_currentNode);
    }

    private void OnDisable()
    {
        if (_updateDisposable != null)
            _updateDisposable.Dispose();
    }
}
