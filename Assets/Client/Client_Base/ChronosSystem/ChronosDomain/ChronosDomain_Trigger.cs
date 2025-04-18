
using UnityEngine;

public class ChronosDomain_Trigger : ChronosDomain
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var chronosComponent = other.GetComponent<IChronosComponent>();
        if (chronosComponent == null) return;
        OnEnter(chronosComponent);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        var chronosComponent = other.GetComponent<IChronosComponent>();
        if (chronosComponent == null) return;
        OnExit(chronosComponent);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        var chronosComponent = other.GetComponent<IChronosComponent>();
        if (chronosComponent == null) return;
        OnEnter(chronosComponent);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.isTrigger) return;
        var chronosComponent = other.GetComponent<IChronosComponent>();
        if (chronosComponent == null) return;
        OnExit(chronosComponent);
    }
}
