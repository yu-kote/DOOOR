using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimController : MonoBehaviour
{
    [SerializeField]
    private GameObject _human;

    private Script_SpriteStudio_Root _root;

    private AIController _aiController;

    private VictimAnimationStatus _currentVictimAnimStatus;
    private KillerAnimationStatus _currentKillerAnimStatus;

    void Start()
    {
        var ai_generator = GameObject.Find("HumanManager").GetComponent<AIGenerator>();
        _aiController = GetComponent<AIController>();

        _human = Instantiate(_human, transform);
        _human.transform.localPosition = new Vector3(0, 1.0f, 0);
        _human.transform.localRotation = Quaternion.identity;

        _root = _human.transform.GetChild(0)
            .gameObject.GetComponent<Script_SpriteStudio_Root>();

        var camera = ai_generator.View3dCamera;
        var prefab = _human.GetComponent<Script_SpriteStudio_ControlPrefab>();
        prefab.InstanceManagerDraw = camera.GetComponent<Script_SpriteStudio_ManagerDraw>();

    }

    void Update()
    {
        if (tag == "Victim")
            VictimAnimationStatusUpdate();
        if (tag == "Killer")
            KillerAnimationStatusUpdate();
        Rotation();
    }

    void VictimAnimationStatusUpdate()
    {
        if (_currentVictimAnimStatus == GetComponent<VictimAnimation>().AnimStatus)
            return;

        _currentVictimAnimStatus = GetComponent<VictimAnimation>().AnimStatus;
        _root.AnimationPlay((int)_currentVictimAnimStatus);
    }

    void KillerAnimationStatusUpdate()
    {
        if (_currentKillerAnimStatus == GetComponent<KillerAnimation>().AnimStatus)
            return;

        _currentKillerAnimStatus = GetComponent<KillerAnimation>().AnimStatus;
        _root.AnimationPlay((int)_currentKillerAnimStatus);
    }

    // キャラを進む方向に回転させる
    void Rotation()
    {
        // 落とし穴に落下してる最中に向きを変えると変な挙動する場合がある
        //if (_currentVictimAnimStatus == VictimAnimationStatus.STAGGER)
        //  return;
        if (_currentVictimAnimStatus == VictimAnimationStatus.OPEN_DOOR)
            return;

        var direction = _aiController.GetMovement().MoveDirection;
        if (direction == Vector3.zero)
            return;
        var euler_angle = Quaternion.FromToRotation(new Vector3(0, 0, 1), direction.normalized).eulerAngles;
        euler_angle = new Vector3(0, euler_angle.y + 90.0f, 0);

        var rotation = _human.transform.rotation;
        if (rotation.eulerAngles == euler_angle)
            return;
        rotation.eulerAngles = euler_angle;
        _human.transform.rotation = rotation;
    }

    public void Rotation(GameObject target)
    {
        var direction = target.transform.position - gameObject.transform.position;
        if (direction == Vector3.zero)
            return;
        var euler_angle = Quaternion.FromToRotation(new Vector3(0, 0, 1), direction.normalized).eulerAngles;
        euler_angle = new Vector3(0, euler_angle.y + 90.0f, 0);

        var rotation = _human.transform.rotation;
        if (rotation.eulerAngles == euler_angle)
            return;
        rotation.eulerAngles = euler_angle;
        _human.transform.rotation = rotation;
    }
}
