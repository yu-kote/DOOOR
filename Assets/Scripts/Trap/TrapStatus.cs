using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//罠の種類
//1ビット単位で種類を判別
public enum TrapType
{
	NONE = 0,
	PITFALLS = 1 << 0,	//落とし穴
	DUMMY = 1 << 1,		//殺人鬼のダミー
	SOUND = 1 << 2,		//音
	ROPE = 1 << 3,		//ロープ
	CARPET = 1 << 4,	//足音が聞こえなくなるカーペット
	MAX
}

public class TrapStatus : MonoBehaviour
{
	[SerializeField]
	private uint _canSetTrapStatus = 0;
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
