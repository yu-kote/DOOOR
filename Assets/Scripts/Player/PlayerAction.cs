using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
	// プレイヤーがマップに対してアクションを起こす時に押すボタン
	[SerializeField]
	private string _actionButton = "Action";

	public bool IsPushAcitonButton()
	{
		return Input.GetButtonDown(_actionButton);
	}
}
