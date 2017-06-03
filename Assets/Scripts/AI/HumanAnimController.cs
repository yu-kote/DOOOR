using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimController : MonoBehaviour
{
    [SerializeField]
    private GameObject _human;

    private Script_SpriteStudio_Root _root;

    private AIController _aiController;
    private AIController.AnimationStatus _currentAnimStatus;

    void Start()
    {
        _aiController = GetComponent<AIController>();

        _human = Instantiate(_human, transform);
        _human.transform.localPosition = new Vector3(0, 1.35f, 0);
        _human.transform.localRotation = Quaternion.identity;

        _root = _human.transform.FindChild("chara_noppo")
            .gameObject.GetComponent<Script_SpriteStudio_Root>();
    }

    void Update()
    {
        AnimationStatusUpdate();
        Rotation();
    }

    void AnimationStatusUpdate()
    {
        if (_currentAnimStatus == _aiController.AnimStatus)
            return;

        _currentAnimStatus = _aiController.AnimStatus;
        _root.AnimationPlay((int)_currentAnimStatus);
    }

    // キャラを進む方向に回転させる
    void Rotation()
    {
        var direction = _aiController.GetMovement().MoveDirection;
        if (direction == Vector3.zero)
            return;
        var euler_angle = Quaternion.FromToRotation(new Vector3(0, 0, 1), direction.normalized).eulerAngles;
        euler_angle = new Vector3(0, euler_angle.y + 90.0f, 0);

        var rotation = _human.transform.rotation;
        rotation.eulerAngles = euler_angle;
        _human.transform.rotation = rotation;
    }

}
