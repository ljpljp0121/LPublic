using cfg.Skill;
using UnityEngine;

#if UNITY_EDITOR

public static class SkillGizmosTool
{
    public static void DrawCollider(SkillColliderEvent colliderEvent, SkillPlayerCom skillPlayer)
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        Matrix4x4 rotateAndPositionMat = new Matrix4x4();
        Transform modelTransform = skillPlayer.ModelTransform == null ? skillPlayer.transform : skillPlayer.ModelTransform;
        switch (colliderEvent.ColliderData)
        {
            case WeaponCollider weaponCollider:
                if (!string.IsNullOrEmpty(weaponCollider.WeaponName))
                    break;
        }
    }
}


#endif

