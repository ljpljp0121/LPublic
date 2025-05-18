using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancing : MonoBehaviour
{
    public GameObject Prefab;
    public int Count;
    private int Range = 30;

    void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            Vector2 pos = Random.insideUnitCircle * Range;
            Instantiate(Prefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }
    }
}