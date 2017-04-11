using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeBase : MonoBehaviour
{
    protected GameObject _attribute;

    protected void CreateAttribute(string prefab_name)
    {
        _attribute = Resources.Load<GameObject>("Prefabs/Attribute/" + prefab_name);
        _attribute = Instantiate(_attribute, transform);

        // ノードの子に設定するとき、
        // Unityが自動的にposとrotateを修正してしまうので、
        // 親のposとrotateに合わせるために初期化する

        _attribute.transform.localPosition = Vector3.zero;
        _attribute.transform.localEulerAngles = Vector3.zero;
    }

    private void OnDestroy()
    {
        if (_attribute)
            Destroy(_attribute);
    }

    public bool IsInstanceAttribute()
    {
        return _attribute != null;
    }
}
