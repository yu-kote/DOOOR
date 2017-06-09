using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapList : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _trapObjects = new List<GameObject>();

    private Dictionary<TrapType, GameObject> _trapList
        = new Dictionary<TrapType, GameObject>();

    void Awake()
    {
        SetTrapList();
        if (_trapList.Count == 0)
            Debug.Log("_trapListがセットされてません");

    }

    void SetTrapList()
    {
        int num = 1;
        for (int i = 0; i < (int)_trapObjects.Count; i++)
        {
            _trapList.Add((TrapType)num, _trapObjects[i]);
            num = num << 1;
        }
    }

    public GameObject GetTrapObject(TrapType type)
    {
        if (type == TrapType.NONE)
            Debug.Log("type is NONE");

        return _trapList[type];
    }

    private void OnDestroy()
    {
        _trapObjects.Clear();
    }
}
