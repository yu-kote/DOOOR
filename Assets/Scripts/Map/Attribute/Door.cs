using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Door : AttributeBase
{
    public enum DoorStatus
    {
        OPEN,
        CLOSE
    }

    public DoorStatus _doorStatus = DoorStatus.CLOSE;
    private Animator anim = null;
    private float _statusLockTime = 0.0f;
    public float StatusLockTime
    {
        get { return _statusLockTime; }
        set { _statusLockTime = value; }
    }

    private bool _isReverse = false;

    public GameObject _doorLock = null;

    void Awake()
    {

    }

    void Start()
    {
        CreateAttribute("Door");
        anim = _attribute.transform.GetChild(0).GetComponent<Animator>();
        _attribute.transform.localPosition = new Vector3(0, 1, 0);
    }

    void Update()
    {
        _statusLockTime = Mathf.Max(0.0f, _statusLockTime - Time.deltaTime);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol") &&
            anim.GetBool("IsOpen"))
            anim.SetBool("IsOpen", false);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol") &&
            anim.GetBool("IsReverse"))
            anim.SetBool("IsReverse", false);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("CloseIdol") &&
            anim.GetBool("IsClose"))
            anim.SetBool("IsClose", false);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("OpenIdolReverse") &&
            anim.GetBool("IsReverseClose"))
            anim.SetBool("IsReverseClose", false);

        if (IsDoorLock() == false)
            if (_doorLock != null)
                Destroy(_doorLock);
    }

    public bool StartOpening(bool is_reverse = false)
    {
        if (_statusLockTime > 0.0f)
            return false;
        if (_doorStatus != DoorStatus.CLOSE)
            return false;
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("CloseIdol"))
            return false;

        _doorStatus = DoorStatus.OPEN;

        _isReverse = is_reverse;
        anim.SetBool("IsReverse", is_reverse);
        if (_isReverse == false)
            anim.SetBool("IsOpen", true);

        SoundManager.Instance.PlaySE("doaakeru", gameObject);
        return true;
    }

    public bool StartClosing()
    {
        if (_statusLockTime > 0.0f)
            return false;
        if (_doorStatus != DoorStatus.OPEN)
            return false;
        if (_isReverse == false)
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol"))
                return false;
        if (_isReverse)
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("OpenIdolReverse"))
                return false;

        _doorStatus = DoorStatus.CLOSE;

        anim.SetBool("IsReverse", false);

        anim.SetBool("IsClose", true);
        anim.SetBool("IsReverseClose", true);

        SoundManager.Instance.PlaySE("doasimeru", gameObject);
        return true;
    }

    public bool IsDoorLock()
    {
        return _statusLockTime > 0.0f;
    }

    public bool DoorLock(GameObject lock_object)
    {
        _doorLock = lock_object;
        return true;
    }

    public bool LockDoorStatus(float statusLockTime)
    {
        bool can_lock = true;
        if (_statusLockTime > 0.0f)
            can_lock = false;

        if (_doorStatus == DoorStatus.OPEN)
            return false;
        _statusLockTime = statusLockTime;
        return can_lock;
    }
}
