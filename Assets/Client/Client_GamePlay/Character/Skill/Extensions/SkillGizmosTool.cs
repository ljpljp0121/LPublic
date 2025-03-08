using cfg.Skill;
using UnityEngine;

#if UNITY_EDITOR

public static class SkillGizmosTool
{
    public static void DrawCollider(SkillColliderEvent colliderEvent, SkillPlayerCom skillPlayer)
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        Matrix4x4 rotateAndPositionMat = new Matrix4x4();
        Transform modelTransform =
            skillPlayer.ModelTransform == null ? skillPlayer.transform : skillPlayer.ModelTransform;
        switch (colliderEvent.ColliderData)
        {
            case WeaponCollider weaponCollider:
                if (!string.IsNullOrEmpty(weaponCollider.WeaponName)
                    && skillPlayer.WeaponDic.TryGetValue(weaponCollider.WeaponName, out var weapon))
                {
                    Collider collider = weapon.GetComponent<Collider>();
                    rotateAndPositionMat.SetTRS(collider.transform.position, collider.transform.rotation,
                        collider.transform.localScale);
                    Gizmos.matrix = rotateAndPositionMat;
                    if (collider is BoxCollider)
                    {
                        BoxCollider boxCollider = (BoxCollider)collider;
                        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                    }
                    else if (collider is SphereCollider)
                    {
                        SphereCollider sphereCollider = (SphereCollider)collider;
                        Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
                    }
                }
                break;
            case BoxSkillCollider boxSkillCollider:
                Vector3 boxPos = modelTransform.TransformPoint(boxSkillCollider.Position);
                Quaternion boxRot = modelTransform.rotation * Quaternion.Euler(boxSkillCollider.Rotation);
                rotateAndPositionMat.SetTRS(boxPos, boxRot, Vector3.one);
                Gizmos.matrix = rotateAndPositionMat;
                Gizmos.DrawCube(Vector3.zero, boxSkillCollider.Scale);
                break;
            case CircleSkillCollider circleSkillCollider:
                Vector3 spherePos = modelTransform.TransformPoint(circleSkillCollider.Position);
                Gizmos.DrawSphere(spherePos, circleSkillCollider.Radius);
                break;
            case FanSkillCollider fanSkillCollider:
                Vector3 fanPos = modelTransform.TransformPoint(fanSkillCollider.Position);
                Quaternion fanRot = modelTransform.rotation * Quaternion.Euler(fanSkillCollider.Rotation);
                Mesh mesh = MeshGenerator.GenarteFanMesh(fanSkillCollider.InsideRadius,
                    fanSkillCollider.OutsideRadius, fanSkillCollider.Height, fanSkillCollider.Angle);
                Gizmos.DrawMesh(mesh, fanPos, fanRot);
                break;
        }
    }
}


#endif