using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : AttributeBase
{
    // Left or Right
    private string _directionTag;
    public string DirectionTag { get { return _directionTag; } set { _directionTag = value; } }

    private int _cell_x;
    public int CellX { get { return _cell_x; } set { _cell_x = value; } }

    private NodeManager _nodeManager;
    private Node _linkNode;
    public Node LinkNode { get { return _linkNode; } set { _linkNode = value; } }
    
    private float _statusLockTime = 0.0f;
    public float StatusLockTime
    {
        get { return _statusLockTime; }
        set { _statusLockTime = value; }
    }
    public GameObject _stairsLock = null;

    void Start()
    {
        if (_isInstanceAttribute)
        {
            CreateAttribute("Stairs" + _directionTag);

            var offset = 1f;
            _attribute.transform.localPosition += new Vector3(0, 0, -offset);
        }
    }

    public bool IsStairsLock()
    {
        return _statusLockTime > 0.0f;
    }

    public bool StairsLock(GameObject lock_object)
    {
        _stairsLock = lock_object;
        return true;
    }

    public bool LockStairsStatus(float statusLockTime)
    {
        bool can_lock = true;
        if (_statusLockTime > 0.0f)
            can_lock = false;
        _statusLockTime = statusLockTime;
        return can_lock;
    }

    private void Update()
    {
        _statusLockTime = Mathf.Max(0.0f, _statusLockTime - Time.deltaTime);

        if (IsStairsLock() == false)
            if (_stairsLock != null)
                Destroy(_stairsLock);
    }
}
