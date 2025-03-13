using UnityEngine;

public class WeaponController : MonoBehaviour, IComponent
{
    public string WeaponName;
    public LayerMask AttackLayer;
    private Collider weaponCollider;

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
        if ((AttackLayer & 1 << other.gameObject.layer) > 0)
        {
            Debug.Log("击中物体");
        }
    }
}