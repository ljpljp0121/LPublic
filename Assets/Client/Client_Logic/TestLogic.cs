using System;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    [SerializeField] private Transform root;

    private void Start()
    {
        GameObject obj = AssetSystem.LoadAsset<GameObject>("Prefab/Image");
        Instantiate(obj, root);
    }
}