using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public abstract class AIBasicsMovement : MonoBehaviour
{
    // ノードを強制移動させたときに再度移動の検索を開始させる関数
    public abstract void MoveSetup();
    // このクラスを継承するクラスが一番最初に次に進むノードを選ぶ関数
    protected abstract void StartNextNodeSearch();
    // 次に進むノードを選ぶ関数
    protected abstract void NextNodeSearch();

    protected Node _currentNode;
    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }
    protected Node _nextNode;
    public Node NextNode { get { return _nextNode; } set { _nextNode = value; } }
    protected Node _prevNode;
    public Node PrevNode { get { return _prevNode; } set { _prevNode = value; } }

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
        _speed = GetComponent<AIController>().DefaultSpeed;
        yield return null;
        StartNextNodeSearch();
        NextNodeMoveUpdate();
    }

    private IEnumerator MoveUpdate()
    {
        yield return null;

        _updateDisposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                NextNodeMoveUpdate();
                Move();
            }).AddTo(gameObject);
    }

    protected void NextNodeMoveUpdate()
    {
        // ドアが閉まっているかどうか
        if (IsDoorLock(_nextNode))
            return;

        // 次の移動先が決まったら
        if (_nextNode == null || _nextNode == _currentNode)
            return;
        // 進む方向を決めるためベクトルを出す
        var target = _nextNode.transform.position + HeightCorrection();
        var distance = target - gameObject.transform.position;

        _moveLength = Vector3Abs(distance);

        // 値が小さいほど速度の調整がしやすいので0.03fをかける
        _moveDirection = distance.normalized * _speed * 0.03f;

        // 移動先が決まった時にそのノードに足跡をつける
        NextNodeDecided();

        // 今いるノードを更新する
        _prevNode = _currentNode;
        _currentNode = _nextNode;
    }

    // ベクトルの長さを球状に補間する
    Vector3 Vector3MoveDistance(Vector3 distance)
    {
        var xy_axis_angle = Mathf.Atan2(distance.y, distance.x);
        xy_axis_angle = ToDegrees(xy_axis_angle);
        var zy_axis_angle = Mathf.Atan2(distance.y, distance.z);
        zy_axis_angle = ToDegrees(zy_axis_angle);

        float rad_pi = Mathf.PI / 180.0f;

        var vx = Mathf.Cos(xy_axis_angle * rad_pi) * Mathf.Abs(distance.x);
        var vy = Mathf.Sin(xy_axis_angle * rad_pi) * Mathf.Abs(distance.y);
        var vz = Mathf.Cos(zy_axis_angle * rad_pi) * Mathf.Abs(distance.z);

        return new Vector3(vx, vy, vz);
    }

    float ToDegrees(float x)
    {
        return x * 57.295779513082321f;
    }

    /// <summary>
    /// 実際に動かす場所
    /// </summary>
    void Move()
    {
        if (_canMove == false) return;
        transform.Translate(_moveDirection);
        _moveLength -= Vector3Abs(_moveDirection);

        if (MoveComplete())
        {
            transform.Translate(_moveLength);
            _moveDirection = Vector3.zero;
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
        return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
    }

    public bool MoveComplete()
    {
        return _moveLength.x <= 0.001f &&
            _moveLength.y <= 0.001f &&
            _moveLength.z <= 0.001f;
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

    protected bool IsDoorLock(Node node)
    {
        if (node == null)
            return true;
        var door = node.GetComponent<Door>();
        if (door)
            if (door._doorStatus == Door.DoorStatus.CLOSE)
                if (door.IsDoorLock())
                {
                    //Debug.Log("通れません");
                    return true;
                }
        return false;
    }

    private void OnDisable()
    {
        if (_updateDisposable != null)
            _updateDisposable.Dispose();
    }
}
