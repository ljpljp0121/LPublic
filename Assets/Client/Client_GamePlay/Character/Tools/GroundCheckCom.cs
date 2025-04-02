
using GAS.Runtime;
using UnityEngine;

public class GroundCheckCom : GameComponent
{
    [Header("检测参数")]
    [SerializeField] private float GroundCheckDistance = 0.2f;
    [SerializeField] private LayerMask GroundLayer;

    private CharacterController controller;
    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; }

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnDestroy()
    {

    }

    public void Tick()
    {
        RaycastHit hit;
        IsGrounded = Physics.SphereCast(
            transform.position + controller.center,
            controller.radius,
            Vector3.down,
            out hit,
            GroundCheckDistance,
            GroundLayer);
        GroundNormal = IsGrounded ? hit.normal : Vector3.up;
    }
}
