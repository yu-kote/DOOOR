using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectRange : MonoBehaviour
{
    // 四角の選択範囲
    [SerializeField]
    private GameObject _selectRange;

    void Start()
    {
        _selectRange = Instantiate(_selectRange);
        _selectRange.transform.SetParent(transform);

        var collider = GetComponent<BoxCollider>();

        _selectRange.transform.localScale = collider.size;
    }

    private void OnDisable()
    {
        Destroy(_selectRange);
    }

    void Update()
    {
        var collider = GetComponent<BoxCollider>();

        _selectRange.transform.position = transform.position;
        _selectRange.transform.position += collider.center;
        _selectRange.transform.position += new Vector3(5.0f * _selectRange.transform.localScale.x, 0, 0);

    }
}
