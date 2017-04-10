using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapType
{
	PITFALLS,
	NONE
}

public class TrapStatus : MonoBehaviour
{
	private TrapType _type = TrapType.NONE;
	public TrapType Type
	{
		get { return _type; }
		set { _type = value; }
	}
	private bool _isSpawn = false;
	public bool IsSpawn
	{
		get { return _isSpawn; }
		set { _isSpawn = value; }
	}
}
