#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// 绘制技能网格工具
/// </summary>
public static class SkillGizmosTool
{
    /// <summary>
    /// 绘制当前轨道片段需要在场景中呈现的一些事物
    /// 比如攻击检测的范围等
    /// </summary>
    public static void DrawDetection(SkillColliderEvent attackDetectionEvent, SkillEditorPlayer skillPlayer)
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        Matrix4x4 rotateAndPositionMat = new Matrix4x4();
        Transform modelTransform =
            skillPlayer.ModelTransform == null ? skillPlayer.transform : skillPlayer.ModelTransform;
        switch (attackDetectionEvent.SkillColliderType)
        {
            case SkillColliderType.Weapon:
                WeaponCollider weaponDetection = (WeaponCollider)attackDetectionEvent.SkillColliderData;
                if (!string.IsNullOrEmpty(weaponDetection.weaponName)
                    && skillPlayer.WeaponDic.TryGetValue(weaponDetection.weaponName, out GameObject weapon))
                {
                    Collider collider = weapon.GetComponent<Collider>();
                    rotateAndPositionMat.SetTRS(collider.transform.position, collider.transform.rotation, collider.transform.localScale);
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
            case SkillColliderType.Box:
                BoxSkillCollider boxDetection = (BoxSkillCollider)attackDetectionEvent.SkillColliderData;
                Vector3 boxPos = modelTransform.TransformPoint(boxDetection.Position);
                Quaternion boxRot = modelTransform.rotation * Quaternion.Euler(boxDetection.Rotation);
                rotateAndPositionMat.SetTRS(boxPos, boxRot, Vector3.one);
                Gizmos.matrix = rotateAndPositionMat;
                Gizmos.DrawCube(Vector3.zero, boxDetection.Scale);
                break;
            case SkillColliderType.Sphere:
                SphereSkillCollider sphereDetection = (SphereSkillCollider)attackDetectionEvent.SkillColliderData;
                Vector3 spherePos = modelTransform.TransformPoint(sphereDetection.Position);
                Gizmos.DrawSphere(spherePos, sphereDetection.Radius);
                break;
            case SkillColliderType.Fan:
                FanSkillCollider fanDetection = (FanSkillCollider)attackDetectionEvent.SkillColliderData;
                Vector3 fanPos = modelTransform.TransformPoint(fanDetection.Position);
                Quaternion fanRot = modelTransform.rotation * Quaternion.Euler(fanDetection.Rotation);
                Mesh mesh = MeshGenerator.GenerateFanMesh(fanDetection.InsideRadius,
                    fanDetection.OutsideRadius, fanDetection.Height, fanDetection.Angle);
                Gizmos.DrawMesh(mesh, fanPos, fanRot);
                break;
        }

        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;
    }
}

#endif