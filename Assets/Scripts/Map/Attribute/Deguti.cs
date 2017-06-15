using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deguti : AttributeBase
{
    public Door.DoorStatus _doorStatus = Door.DoorStatus.CLOSE;
    private Animator animLeft = null;
    private Animator animRight = null;
    private float _statusLockTime = 0.0f;
    public float StatusLockTime
    {
        get { return _statusLockTime; }
        set { _statusLockTime = value; }
    }

    private Material[] _materials = new Material[3];
    private GameObject _light;

    void Awake()
    {
        _materials[0] = Resources.Load<Material>("Prefabs/BG/Materials/ExitDoorLeft");
        _materials[1] = Resources.Load<Material>("Prefabs/BG/Materials/ExitDoorRight");
        _materials[2] = Resources.Load<Material>("Prefabs/BG/Materials/ExitDoorFrame");

        _light = Resources.Load<GameObject>("Prefabs/BG/PointLight");
        _light = Instantiate(_light, transform);
    }

    void Start()
    {
        CreateAttribute("Deguti");
        _attribute.transform.localEulerAngles = Vector3.up * 90;
        _attribute.transform.localPosition = Vector3.forward * -5;
        animLeft = _attribute.transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        animRight = _attribute.transform.GetChild(1).GetChild(0).GetComponent<Animator>();

        _light.transform.localPosition = new Vector3(0, 12.5f, -10);
        _light.GetComponent<Light>().range = 15;
    }

    void Update()
    {
        _statusLockTime = Mathf.Max(0.0f, _statusLockTime - Time.deltaTime);

        if (animLeft.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol") &&
            animLeft.GetBool("IsOpen"))
            animLeft.SetBool("IsOpen", false);

        if (animLeft.GetCurrentAnimatorStateInfo(0).IsName("CloseIdol") &&
            animLeft.GetBool("IsClose"))
            animLeft.SetBool("IsClose", false);

        if (animRight.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol") &&
            animRight.GetBool("IsOpen"))
            animRight.SetBool("IsOpen", false);

        if (animRight.GetCurrentAnimatorStateInfo(0).IsName("CloseIdol") &&
            animRight.GetBool("IsClose"))
            animRight.SetBool("IsClose", false);
    }

    public bool StartOpening()
    {
        if (_statusLockTime > 0.0f)
            return false;
        if (_doorStatus != Door.DoorStatus.CLOSE)
            return false;
        if (!animLeft.GetCurrentAnimatorStateInfo(0).IsName("CloseIdol"))
            return false;

        _doorStatus = Door.DoorStatus.OPEN;
        animLeft.SetBool("IsOpen", true);
        animRight.SetBool("IsOpen", true);

        for (int i = 0; i < _materials.Count(); i++)
            _materials[i].color = new Color(1, 1, 1, 1);

        return true;
    }

    public bool StartClosing()
    {
        if (_statusLockTime > 0.0f)
            return false;
        if (_doorStatus != Door.DoorStatus.OPEN)
            return false;
        if (!animLeft.GetCurrentAnimatorStateInfo(0).IsName("OpenIdol"))
            return false;

        _doorStatus = Door.DoorStatus.CLOSE;
        animLeft.SetBool("IsClose", true);
        animRight.SetBool("IsClose", true);

        for (int i = 0; i < _materials.Count(); i++)
            _materials[i].color = new Color(1, 1, 1, 0.3f);

        return true;
    }

    public bool IsDoorLock()
    {
        return _statusLockTime > 0.0f;
    }

    public void LockDoorStatus(float statusLockTime)
    {
        if (_statusLockTime > 0.0f)
            return;

        _statusLockTime = statusLockTime;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _materials.Count(); i++)
            _materials[i].color = new Color(1, 1, 1, 1);
    }
}
