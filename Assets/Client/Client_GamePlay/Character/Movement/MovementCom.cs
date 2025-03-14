
using DG.Tweening.Core.Easing;
using UnityEngine;
public class MovementCom : MonoBehaviour, IComponent, IEnabled, IRequire<AnimationCom>
{
    private AnimationCom animCom;
    private CharacterController characterController;

    private bool isWalk = false;

    public bool IsEnable { get; set; }
    public void SetDependency(AnimationCom dependency) => animCom = dependency;

    public void Init()
    {
        IsEnable = true;
        characterController = GetComponent<CharacterController>();
        animCom.SetRootMotionAction(OnRootMotion);
        InputManager.Instance.MoveEvent += OnMove;
        InputManager.Instance.FlashEvent += OnFlash;
    }

    public void UnInit()
    {
        animCom.ClearRootMotionAction();
        InputManager.Instance.MoveEvent -= OnMove;
        InputManager.Instance.FlashEvent -= OnFlash;
    }

    public void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        animCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
    }

    private void OnMove(Vector2 direction)
    {
        if (!IsEnable)
            return;
        Vector3 input = new Vector3(direction.x, 0, direction.y);
        if (direction is { x: 0, y: 0 })
        {
            animCom.PlaySingleAnimation(default);
        }
        if (!isWalk)
        {
            animCom.PlaySingleAnimation(default);
        }
        else
        {
            animCom.PlaySingleAnimation(default);
        }
    }

    private void OnFlash()
    {
        Debug.Log("闪避");
    }
}
