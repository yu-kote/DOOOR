using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeBase : MonoBehaviour
{
    protected GameObject _attribute;
    public GameObject Attribute
    {
        get { return _attribute; }
        set { _attribute = value; }
    }

    protected Vector3 _rotateAngle = Vector3.zero;
    public Vector3 RotateAngle
    {
        get { return _rotateAngle; }
        set { _rotateAngle = value; }
    }
    
    protected bool _isInstanceAttribute = true;
    public bool IsInstanceAttribute
    {
        get { return _isInstanceAttribute; }
        set { _isInstanceAttribute = value; }
    }
    
    protected void CreateAttribute(string prefab_name)
    {
        _attribute = Resources.Load<GameObject>("Prefabs/Map/Attribute/" + prefab_name);
        _attribute = Instantiate(_attribute, transform);

        // ノードの子に設定するとき、
        // Unityが自動的にposとrotateを修正してしまうので、
        // 親のposとrotateに合わせるために初期化する

        _attribute.transform.localPosition = Vector3.zero;
        _attribute.transform.localEulerAngles = _rotateAngle;
    }

    private void OnDestroy()
    {
        if (_attribute)
            Destroy(_attribute);
    }
}
