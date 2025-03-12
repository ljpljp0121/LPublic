
public class MoveComponent : IComponent, IUpdatable, IEnabled
{
    public bool IsEnable { get; set; }

    public void Init()
    {
        IsEnable = true;
    }

    public void OnUpdate()
    {

    }


}
