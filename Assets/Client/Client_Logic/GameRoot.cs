using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameRoot : MonoBehaviour
{
    private void Awake()
    {
        ClientAttributes.ExecuteInitOnLoadMethods();
        AssetSystem.LoadAsset<GameObject>("Prefab/Image");
    }

    [InitOnLoad]
    private static void Init()
    {
        Debug.Log("GameRoot Init");
    }
}