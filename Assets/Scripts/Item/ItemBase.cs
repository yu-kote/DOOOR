using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemID
{
	NONE = 0,				//何もない
	KEY = 1 << 0,           //部屋の鍵
	LASTKEY = 1 << 1,		//玄関(出口)の鍵
	FLASHLIGHT = 1 << 2,	//懐中電灯
	GUN = 1 << 3			//銃
}

public class ItemBase : MonoBehaviour
{
	protected ItemID _itemID = ItemID.NONE;
	protected bool _isAcquired = false;
	public bool IsAcquired
	{
		get { return _isAcquired; }
		set { _isAcquired = value; }
	}

	protected GameObject _item;
	public GameObject Attribute
	{
		get { return _item; }
		set { _item = value; }
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

	protected void CreateItem(string prefab_name)
	{
		_item = Resources.Load<GameObject>("Prefabs/Item/" + prefab_name);
		_item = Instantiate(_item, transform);

		// ノードの子に設定するとき、
		// Unityが自動的にposとrotateを修正してしまうので、
		// 親のposとrotateに合わせるために初期化する

		_item.transform.localPosition = Vector3.zero;
		_item.transform.localEulerAngles = _rotateAngle;

	}

	public ItemID AcquiredItem()
	{
		_isAcquired = true;

		return _itemID;
	}

	void Update()
	{
		if (!_isAcquired)
			return;

		Destroy(gameObject);
	}
}
