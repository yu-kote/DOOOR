using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//罠の種類
//1ビット単位で種類を判別
public enum TrapType
{
	NONE = 0,
	PITFALLS = 1 << 0,	//落とし穴
	SOUND = 1 << 1,		//音
	ROPE = 1 << 2,		//ロープ
	MAX = 1 << 3
}

public class TrapStatus : MonoBehaviour
{
	private uint _canSetTrapStatus = 7;
	public uint CanSetTrapStatus
	{
		get { return _canSetTrapStatus; }
		set { _canSetTrapStatus = value; }
	}
	private bool _isSpawn = false;
	public bool IsSpawn
	{
		get { return _isSpawn; }
		set { _isSpawn = value; }
	}

	public void AddCanSetTrap(TrapType type)
	{
		_canSetTrapStatus = _canSetTrapStatus | (uint)type;
	}

	public bool IsCanSetTrap(TrapType type)
	{
		return ((_canSetTrapStatus & (uint)type) > 0);
	}
}
