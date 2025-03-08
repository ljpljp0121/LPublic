using UnityEngine;

public class WeaponController : MonoBehaviour, IComponent, IRequire<SkillPlayerCom>
{
    public string WeaponName;
    private Collider weaponCollider;
    private LayerMask attackLayer;

    public void SetDependency(SkillPlayerCom dependency) => attackLayer = dependency.ColliderLayer;

    public void Init()
    {
        weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
    }

    public void StartDetection()
    {
        weaponCollider.enabled = true;
    }

    public void EndDetection()
    {
        weaponCollider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if ((attackLayer & 1 << other.gameObject.layer) > 0)
        {
            Debug.Log("击中物体");
        }
    }
}