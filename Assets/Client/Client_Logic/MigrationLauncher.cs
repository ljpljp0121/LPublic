using UnityEngine;

public static class MigrationLauncher
{
    [InitOnLoad]
    public static void InitMigrationAction()
    {
        // MigrationManager.RegisterMigration<TestData>
        // (1, data =>
        // {
        //     Debug.Log($"正在迁移{data.GetType()}从v1到v2");
        //     Debug.Log($"迁移前数据：{JsonUtility.ToJson(data)}");
        //     data.Stature = data.Height;
        //     data.Height = 0;
        //     Debug.Log($"迁移后数据：{JsonUtility.ToJson(data)}");
        // });
    }
}