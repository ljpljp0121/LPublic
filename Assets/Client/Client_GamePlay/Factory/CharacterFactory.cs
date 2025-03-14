
using System;
using System.Threading.Tasks;
using cfg.role;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CharacterFactory
{
    public static async Task Build(int roleId, Transform tr = null)
    {
        try
        {
            RoleRes roleRes = TableSystem.GetVOData<TbRoleRes>().Get(roleId);
            GameObject obj = await AssetSystem.LoadAssetAsync<GameObject>(roleRes.Prefab);
            if (tr == null)
            {
                Object.Instantiate(obj);
            }
            else
            {
                Object.Instantiate(obj, tr);
            }
            SetStats(roleRes);
            obj.GetComponent<ComponentSystem>().Initialize();
        }
        catch (Exception e)
        {
            LogSystem.Error($"角色构建失败: {e.Message}");
            throw;
        }
    }

    private static void SetStats(RoleRes roleRes)
    {

    }
}
