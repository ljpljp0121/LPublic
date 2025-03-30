using UnityEngine;

/// <summary>
/// 移动
/// </summary>
public class EOnInputMove : BaseEvent
{
    public Vector2 MoveDir;

    public EOnInputMove(Vector2 moveDir)
    {
        MoveDir = moveDir;
    }
}

/// <summary>
/// 冲刺
/// </summary>
public class EOnInputFlash : BaseEvent { }

/// <summary>
/// 打开菜单
/// </summary>
public class EOnInputMenu : BaseEvent { }

/// <summary>
/// 打开信息面板
/// </summary>
public class EOnInputInfo : BaseEvent { }

/// <summary>
/// 关闭信息面板
/// </summary>
public class EOnInputInfoCancelled : BaseEvent { }

/// <summary>
/// 普通攻击
/// </summary>
public class EOnInputCommonAttack : BaseEvent { }

/// <summary>
/// 普通技能
/// </summary>
public class EOnInputCommonSkill : BaseEvent { }

/// <summary>
/// 终极技能
/// </summary>
public class EOnInputUltimateSkill : BaseEvent { }

/// <summary>
/// 关闭菜单
/// </summary>
public class EOnInputResume : BaseEvent { }