using cfg.Skill;
using UnityEngine;

/// <summary>
/// 技能攻击检测工具类
/// 用于运行时检测范围
/// </summary>
public static class SkillColliderTool
{
    public static Collider[] detectionResult = new Collider[20];

    /// <summary>
    /// 形状范围检测
    /// </summary>
    public static Collider[] ShapeDetection(Transform modelTrans, ColliderBase data, LayerMask layer)
    {
        switch (data)
        {
            case BoxSkillCollider boxCollider:
                return BoxDetection(modelTrans, boxCollider, layer);
            case CircleSkillCollider circleCollider:
                return SphereDetection(modelTrans, circleCollider, layer);
            case FanSkillCollider fanCollider:
                return FanDetection(modelTrans, fanCollider, layer);
        }
        return null;
    }

    /// <summary>
    /// 盒型范围检测
    /// </summary>
    public static Collider[] BoxDetection(Transform modelTrans, BoxSkillCollider data, LayerMask layer)
    {
        CleanDetectionResults();
        Physics.OverlapBoxNonAlloc(modelTrans.TransformPoint(data.Position), data.Scale / 2, detectionResult,
            modelTrans.rotation * Quaternion.Euler(data.Rotation), layer);
        return detectionResult;
    }

    /// <summary>
    /// 球形范围检测
    /// </summary>
    public static Collider[] SphereDetection(Transform modelTrans, CircleSkillCollider data, LayerMask layer)
    {
        CleanDetectionResults();
        Physics.OverlapSphereNonAlloc(modelTrans.TransformPoint(data.Position), data.Radius, detectionResult, layer);
        return detectionResult;
    }

    /// <summary>
    /// 扇形范围检测
    /// </summary>
    public static Collider[] FanDetection(Transform modelTrans, FanSkillCollider data, LayerMask layer)
    {
        CleanDetectionResults();
        Vector3 fanPosition = modelTrans.TransformPoint(data.Position);
        Vector3 size = new Vector3();
        size.x = data.OutsideRadius * 2;
        size.z = size.x;
        size.y = data.Height;
        Physics.OverlapBoxNonAlloc(fanPosition, size / 2, detectionResult,
            modelTrans.rotation * Quaternion.Euler(data.Rotation), layer);

        //过滤无效检测
        Vector3 fanForward = modelTrans.rotation * Quaternion.Euler(data.Rotation) * Vector3.forward;
        for (int i = 0; i < detectionResult.Length; i++)
        {
            if (detectionResult[i] == null) break;
            //过滤内半径内，外半径外
            Vector3 point = detectionResult[i].ClosestPoint(modelTrans.position);
            float distance = Vector3.Distance(point, modelTrans.position);
            bool remove = distance < data.InsideRadius || distance > data.OutsideRadius;
            //过滤不在角度范围内的
            if (!remove)
            {
                Vector3 dir = point - fanPosition;
                float angle = Vector3.Angle(fanForward, dir);
                remove = angle > data.Angle * 0.5f;
            }
            if (remove)
            {
                detectionResult[i] = null;
            }
        }
        return detectionResult;
    }

    /// <summary>
    /// 清空检测结果
    /// 因为范围检测使用NonAlloc，需要自己传数组，所以每次结束记得清空数组
    /// </summary>
    private static void CleanDetectionResults()
    {
        for (int i = 0; i < detectionResult.Length; i++)
        {
            detectionResult[i] = null;
        }
    }
}