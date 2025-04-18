using System;
using UnityEngine;

public class ChronosDomain : MonoBehaviour
{
    [SerializeField]
    protected float timeScaleValue = 1f;

    protected TimeScaleGroup domainTimeGroup;

    public float TimeScaleValue => timeScaleValue;
    public TimeScaleGroup DomainTimeGroup => domainTimeGroup;

    public void SetTimeScaleValue(float value) => timeScaleValue = value;

    protected virtual void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning("ChronosDomain's Collider was not set as Trigger,please check it");
        }

        var col2D = GetComponent<Collider2D>();
        if (col2D != null && !col2D.isTrigger)
        {
            col2D.isTrigger = true;
            Debug.LogWarning("ChronosDomain's Collider was not set as Trigger,please check it");
        }

        domainTimeGroup = new TimeScaleGroup($"{this.gameObject.name}_ChronosDomain", timeScaleValue);
    }

    protected virtual void OnDisable()
    {
        if (domainTimeGroup == null) return;
        domainTimeGroup.UnRegisterAll();
    }

    protected virtual void OnDestroy()
    {
        if (domainTimeGroup == null) return;

        domainTimeGroup.Dispose();
        domainTimeGroup = null;
    }


    protected virtual void OnEnter(IChronosComponent chronosComponent) => domainTimeGroup.Register(chronosComponent);
    protected virtual void OnExit(IChronosComponent chronosComponent) => domainTimeGroup.UnRegister(chronosComponent);
}
