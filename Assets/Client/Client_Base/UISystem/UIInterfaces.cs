
using UnityEngine;

/// <summary>
/// UISystem对外数据接口
/// </summary>
public interface IUIStorage
{
    void RegisterUIDic(string key, UIBehavior uiBehavior);
    bool TryGetFromUIDic(string key, out UIBehavior uiBehavior);
    void UnRegisterUIDic(string key);

    void RegisterPrefabDic(string key, GameObject prefab);
    bool TryGetFromPrefabDic(string key, out GameObject prefab);
    void UnRegisterPrefabDic(string key);

    void PushUIStack(UIBehavior uiBehavior);
    UIBehavior PopUIStack();
    UIBehavior PeekUIStack();
}