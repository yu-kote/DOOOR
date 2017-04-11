using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class AIMovement : MonoBehaviour
{
    private GameObject _field;
    private NodeController _nodeController;

    private MyNumber _myNumber;

    private Node _currentNode;
    private Node _nextNode = null;

    // デバッグ用にSerializeField
    [SerializeField]
    private Vector3 _moveDirection;
    [SerializeField]
    private Vector3 _moveLength;

    private bool _canMove = true;
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }

    [SerializeField]
    private float _speed = 1;

    public void Start()
    {
        _field = GameObject.Find("Field");
        _nodeController = _field.GetComponent<NodeController>();
        _myNumber = GetComponent<MyNumber>();
        var node_manager = _field.GetComponent<NodeManager>();
        _currentNode = node_manager.SearchOnNodeHuman(gameObject);
        NextNodeSearch();
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

    void Update()
    {
        // Unirx使おうとしたら2フレーム処理されてしまうバグが出たため保留
        //    this.ObserveEveryValueChanged(movement => movement._nextNode != movement._currentNode)
        //.Subscribe(_ =>
        //{
        //    Debug.Log("ノードを移動します");

        //    var distance = _currentNode.transform.position - _nextNode.transform.position;
        //    _moveDistance = distance;
        //    _moveDirection = distance * 0.01f;
        //    LeaveFootPrint();
        //    _currentNode = _nextNode;
        //    AddFootPrint();
        //    _canMove = true;
        //});

        // 次の移動先が決まったら
        if (_nextNode != _currentNode)
        {
            // 進む方向を決めるためベクトルを出す
            var distance = _nextNode.transform.position - _currentNode.transform.position;
            _moveLength = new Vector3(Mathf.Abs(distance.x), Mathf.Abs(distance.z), Mathf.Abs(distance.z));
            _moveDirection = distance * 0.01f * _speed;

            // 今乗っていたノードから立ち去る
            LeaveFootPrint(_currentNode);

            // 次のノードに乗る
            AddFootPrint(_nextNode);

            // 今いるノードを更新する
            _currentNode = _nextNode;
        }
        Move();
    }

    void NextNodeSearch()
    {
        // まだ足跡がついてないノードを探す
        var candidate = CanMoveNode();

        // 周りのノードが全部足跡ついていたら自分の足跡をすべて消して探しなおす
        if (candidate.Count == 0)
        {
            _nodeController.EraseTraces(_myNumber);
            // 今乗っているノードが再検索されないように足跡を付け直す
            _currentNode.GetComponent<FootPrint>().AddTrace(gameObject);
            candidate = CanMoveNode();
        }

        var next_node_num = Random.Range(0, candidate.Count);

        _nextNode = candidate[next_node_num];
    }

    List<Node> CanMoveNode()
    {
        return _currentNode.LinkNodes
            .Where(node => node.GetComponent<FootPrint>().Traces.Contains(_myNumber) == false)
            .Where(node => node.GetComponent<Wall>() == null)
            .ToList();
    }

    void Move()
    {
        if (_canMove == false) return;
        transform.position += _moveDirection;
        _moveLength -= Vector3Abs(_moveDirection);

        if (_moveLength.x <= 0 &&
            _moveLength.y <= 0 &&
            _moveLength.z <= 0)
        {
            transform.position =
                new Vector3(_currentNode.transform.position.x,
                            _currentNode.transform.position.y + transform.localScale.y,
                            _currentNode.transform.position.z);
            NextNodeSearch();
        }
    }

    Vector3 Vector3Abs(Vector3 value)
    {
        return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.z), Mathf.Abs(value.z));
    }
}
