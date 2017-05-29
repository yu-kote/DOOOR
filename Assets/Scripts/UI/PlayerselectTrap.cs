using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerselectTrap : MonoBehaviour
{
	private PlayerAction _playerAction = null;
	private TrapType _playerSelectTrap = TrapType.NONE;
	private Text _text = null;

	void Start()
	{
		_playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
		if (_playerAction == null)
			Debug.Log("_playerAciton is not esixt");

		_text = GetComponent<Text>();
		if (_text == null)
			Debug.Log("_text is not esixt");
	}

	void Update()
	{
		if (_playerSelectTrap == _playerAction.SelectTrapType)
			return;

		_playerSelectTrap = _playerAction.SelectTrapType;
		switch (_playerSelectTrap)
		{
			case TrapType.NONE:
				_text.text = "NONE";
				return;
			case TrapType.PITFALLS:
				_text.text = "PITFALLS";
				return;
			case TrapType.SOUND:
				_text.text = "SOUND";
				return;
			case TrapType.ROPE:
				_text.text = "ROPE";
				return;
		}
	}
}
