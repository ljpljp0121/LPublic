#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SkillGizmosTool1
{
    /// <summary>
    /// 绘制当前轨道片段需要在场景中呈现的一些事物
    /// 比如攻击检测的范围等
    /// </summary>
    public static void DraweDetection(SkillAttackDetectionEvent attackDetectionEvent, SkillPlayer skill_Player)
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        Matrix4x4 rotateAndPositionMat = new Matrix4x4();
        Transform modelTransform = skill_Player.ModelTransform == null ? skill_Player.transform : skill_Player.ModelTransform;
        switch (attackDetectionEvent.AttackDetectionType)
        {
            case AttackDetectionType.Weapon:
                WeaponAttackDetection weaponDetection = (WeaponAttackDetection)attackDetectionEvent.AttackDetectionData;
                if (!string.IsNullOrEmpty(weaponDetection.weaponName)
                    && skill_Player.WeaponDic.TryGetValue(weaponDetection.weaponName, out WeaponController weapon))
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
            case AttackDetectionType.Box:
                BoxAttackDetection boxDetection = (BoxAttackDetection)attackDetectionEvent.AttackDetectionData;
                Vector3 boxPos = modelTransform.TransformPoint(boxDetection.Position);
                Quaternion boxRot = modelTransform.rotation * Quaternion.Euler(boxDetection.Rotation);
                rotateAndPositionMat.SetTRS(boxPos, boxRot, Vector3.one);
                Gizmos.matrix = rotateAndPositionMat;
                Gizmos.DrawCube(Vector3.zero, boxDetection.Scale);
                break;
            case AttackDetectionType.Sphere:
                SphereAttackDetection sphereDetection = (SphereAttackDetection)attackDetectionEvent.AttackDetectionData;
                Vector3 spherePos = modelTransform.TransformPoint(sphereDetection.Position);
                Gizmos.DrawSphere(spherePos, sphereDetection.Radius);
                break;
            case AttackDetectionType.Fan:
                FanAttackDetection fanDetection = (FanAttackDetection)attackDetectionEvent.AttackDetectionData;
                Vector3 fanPos = modelTransform.TransformPoint(fanDetection.Position);
                Quaternion fanRot = modelTransform.rotation * Quaternion.Euler(fanDetection.Rotation);
                Mesh mesh = MeshGenerator.GenarteFanMesh(fanDetection.InsideRadius,
                         fanDetection.OutsideRadius, fanDetection.Height, fanDetection.Angle);
                Gizmos.DrawMesh(mesh, fanPos, fanRot);
                break;
        }

        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;
    }
}

#endif