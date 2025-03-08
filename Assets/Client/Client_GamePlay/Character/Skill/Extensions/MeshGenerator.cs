using UnityEngine;

/// <summary>
/// 网格生成器
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// 生成扇形网格
    /// </summary>
    public static Mesh GenarteFanMesh(float insideRadius, float outsideRadius, float height, float angle)
    {
        Mesh fanmesh = new Mesh();
        Vector3 centerPos = Vector3.zero;
        Vector3 direction = Vector3.forward;
        Vector3 rightDir = Quaternion.AngleAxis(angle / 2, Vector3.up) * direction;
        float deltaAngle = 2.5f;
        int rects = (int)(angle / deltaAngle);
        int lines = rects + 1;
        Vector3[] vertexs = new Vector3[2 * lines * 2];
        int[] triangles = new int[6 * rects * 4 + 6 + 12];

        //底面
        for (int i = 0; i < lines; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(-deltaAngle * i, Vector3.up) * rightDir;
            Vector3 minPos = centerPos + dir * insideRadius;
            Vector3 maxPos = centerPos + dir * outsideRadius;

            vertexs[i * 2] = minPos;
            vertexs[i * 2 + 1] = maxPos;

            //处理三角面 1 2 0 | 1 3 2
            if (i < lines - 1)
            {
                triangles[i * 6] = 2 * i + 1;
                triangles[i * 6 + 1] = 2 * (i + 1);
                triangles[i * 6 + 2] = 2 * i;

                triangles[i * 6 + 3] = 2 * i + 1;
                triangles[i * 6 + 4] = 2 * (i + 1) + 1;
                triangles[i * 6 + 5] = 2 * (i + 1);
            }
        }

        //顶面
        for (int i = lines; i < 2 * lines; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(-deltaAngle * (i - lines), Vector3.up) * rightDir;
            Vector3 minPos = centerPos + dir * insideRadius;
            Vector3 maxPos = centerPos + dir * outsideRadius;
            minPos.y += height;
            maxPos.y += height;

            vertexs[i * 2] = minPos;
            vertexs[i * 2 + 1] = maxPos;

            //处理三角面 0 2 1 | 2 3 1
            if (i < 2 * lines - 1)
            {
                triangles[i * 6] = 2 * i;
                triangles[i * 6 + 1] = 2 * (i + 1);
                triangles[i * 6 + 2] = 2 * i + 1;

                triangles[i * 6 + 3] = 2 * (i + 1);
                triangles[i * 6 + 4] = 2 * (i + 1) + 1;
                triangles[i * 6 + 5] = 2 * i + 1;
            }
        }

        //右面
        //0 2 1 | 2 3 1
        int start = 2 * lines - 1;
        triangles[start * 6 + 0] = 0;
        triangles[start * 6 + 1] = 2 * lines;
        triangles[start * 6 + 2] = 1;
        triangles[start * 6 + 3] = 2 * lines;
        triangles[start * 6 + 4] = 2 * lines + 1;
        triangles[start * 6 + 5] = 1;


        //左面
        //1 2 0 | 1 3 2 
        triangles[start * 6 + 6] = 2 * (lines - 1) + 1;
        triangles[start * 6 + 7] = 2 * (2 * lines - 1);
        triangles[start * 6 + 8] = 2 * (lines - 1);
        triangles[start * 6 + 9] = 2 * (lines - 1) + 1;
        triangles[start * 6 + 10] = 2 * (2 * lines - 1) + 1;
        triangles[start * 6 + 11] = 2 * (2 * lines - 1);

        //后面
        //0 2 1 | 2 3 1
        start += 2;
        for (int i = 0; i < rects; i++)
        {
            int index = start + i;
            triangles[index * 6 + 0] = i * 2 + 0;
            triangles[index * 6 + 1] = i * 2 + 2;
            triangles[index * 6 + 2] = 2 * lines + i * 2;
            triangles[index * 6 + 3] = i * 2 + 2;
            triangles[index * 6 + 4] = 2 * (lines + 1) + i * 2;
            triangles[index * 6 + 5] = 2 * lines + i * 2;
        }

        //前面
        //1 2 0 | 1 3 2 
        start += rects;
        for (int i = 0; i < rects; i++)
        {
            int index = start + i;
            triangles[index * 6 + 0] = 2 * lines + i * 2 + 1;
            triangles[index * 6 + 1] = i * 2 + 2 + 1;
            triangles[index * 6 + 2] = i * 2 + 1;
            triangles[index * 6 + 3] = 2 * lines + i * 2 + 1;
            triangles[index * 6 + 4] = 2 * (lines + 1) + i * 2 + 1;
            triangles[index * 6 + 5] = i * 2 + 2 + 1;
        }

        fanmesh.vertices = vertexs;
        fanmesh.triangles = triangles;
        fanmesh.RecalculateNormals();
        return fanmesh;
    }

    
}
