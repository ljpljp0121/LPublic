
using UnityEngine;

/// <summary>
/// UISystem对外数据接口
/// </summary>
public interface IUIStorage
{
    void ChangeOrAddUIDic(string uiName, UIBehavior uiBehavior);
    bool TryGetFromUIDic(string uiName, out UIBehavior uiBehavior);
    void TryRemoveUIDic(string uiName);

    void ChangeOrAddPrefabDic(string uiName, GameObject prefab);
    bool TryGetFromPrefabDic(string uiName, out GameObject prefab);
    void TryRemovePrefabDic(string uiName);

    void PushUIStack(UIBehavior uiBehavior);
    UIBehavior TryPopUIStack();
    UIBehavior TryPeekUIStack();
}