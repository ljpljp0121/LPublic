public class NoneUpdateStateMachine : StateMachine
{
    public override void Update() { }

    public override void FixedUpdate() { }

    public override void LateUpdate() { }

    public virtual void Tick()
    {
        CurrStateObj?.Update();
    }
}