using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBehaviorCom : MonoBehaviour, IComponent, IRequire<IEnumerable<ISkillComponent>>,
    IRequire<AnimationCom>
{
    private List<ISkillComponent> skillComponents;
    private CharacterController characterController;
    private AnimationCom animCom;

    public void SetDependency(AnimationCom dependency) => animCom = dependency;

    public void SetDependency(IEnumerable<ISkillComponent> dependency) =>
        skillComponents = dependency.ToList();

    public void Init()
    {
        characterController = GetComponent<CharacterController>();
    }


    public void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        animCom.transform.rotation *= deltaRotation;
        characterController.Move(deltaPosition);
    }
}