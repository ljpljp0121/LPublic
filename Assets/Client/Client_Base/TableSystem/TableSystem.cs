using cfg;
using SimpleJSON;
using UnityEngine;

public static class TableSystem
{
    public static Tables Table;

    [InitOnLoad]
    public static void Init()
    {
        Table = new Tables(LoadTable);
        Debug.Log("表初始化成功");
    }

    private static JSONNode LoadTable(string tableName)
    {
        var textAsset = AssetSystem.LoadAsset<TextAsset>($"TableData/{tableName}.json");
        return JSON.Parse(textAsset.text);
    }
}