using System.Collections.Generic;
using System.IO;
using DG.Tweening.Plugins;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasTool : OdinEditorWindow
{
    [Title("图集工具")]
    [InfoBox("路径是Assets开头的路径")]
    [SerializeField]
    private string SpritesFolderPath = "Assets/GameRes/SpriteBundle";

    [SerializeField]
    private string AtlasFolderPath = "Assets/Bundle/Atlas";

    [InfoBox("会将SpriteFolder里的图片按文件夹打成图集,每个文件夹一个图集")]
    [Button("打图集", ButtonHeight = 30)]
    private void MakeAtlas()
    {
        var folders = GetSpriteFolders();
        if (folders == null || folders.Count == 0) return;
        RePackAtlases(AtlasFolderPath, ref folders);

        CreateAtlases(AtlasFolderPath, folders);
    }

    private List<string> GetSpriteFolders()
    {
        List<string> folders = new List<string>();

        var paths = Directory.GetDirectories(SpritesFolderPath, "*", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            string[] guids = Directory.GetDirectories(path.Replace("\\","/"), "*", SearchOption.TopDirectoryOnly);
            if (guids.Length != 0) continue;
            folders.Add(path.Replace("\\", "/"));
        }
        return folders;
    }

    /// <summary>
    /// 检查图集是否已经生成，生成的话就删掉
    /// </summary>
    private void RePackAtlases(string atlasFolderPath, ref List<string> spritesFolderPaths)
    {
        var existAtlasGuids = AssetDatabase.FindAssets(".spriteatlas", new string[] { atlasFolderPath });
        if (existAtlasGuids != null && existAtlasGuids.Length > 0)
        {
            var existAtlasPaths = new string[existAtlasGuids.Length];
            var existAtlases = new SpriteAtlas[existAtlasGuids.Length];
            for (int i = 0; i < existAtlasGuids.Length; i++)
            {
                existAtlasPaths[i] = AssetDatabase.GUIDToAssetPath(existAtlasGuids[i]);
                string atlasName = existAtlasPaths[i].Replace(SpritesFolderPath + "/", "").Replace(".spriteatlas", "");
                string imgsPath = atlasName.IndexOf("_") >= 0 ? atlasName.Replace("_", "/") : atlasName;
                Debug.Log($"atlasPath: {existAtlasPaths[i]}, imgsPath: {imgsPath}");

                existAtlases[i] = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(existAtlasPaths[i]);
                bool isSameFolder = false;
                foreach (var folder in spritesFolderPaths)
                {
                    isSameFolder = folder.IndexOf(imgsPath) >= 0;
                    Debug.Log($"folder: {folder} , isSameFolder : {isSameFolder}");
                    if (isSameFolder)
                    {
                        spritesFolderPaths.Remove(folder);
                        break;
                    }
                }
                UnityEditor.EditorUtility.DisplayProgressBar("RepackAtlases", $"atlasPath : {existAtlasPaths[i]}, imgsPath : {imgsPath} , bDeleted : {isSameFolder}", i * 1f / existAtlasGuids.Length);
            }
            SpriteAtlasUtility.PackAtlases(existAtlases, EditorUserBuildSettings.activeBuildTarget);
            UnityEditor.EditorUtility.ClearProgressBar();
        }
    }

    private void CreateAtlases(string atlasFolderPath, List<string> folders)
    {
        int idx = 0;
        foreach (var folder in folders)
        {
            UnityEditor.EditorUtility.DisplayProgressBar("CreateAtlases", $"atlasPath: {folder}", idx++ * 1f / folders.Count);
            CreateAtlas(atlasFolderPath, folder);
        }
        UnityEditor.EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 创建图集
    /// </summary>
    private void CreateAtlas(string atlasFolderPath, string spritesFolderPath)
    {
        string atlasName = spritesFolderPath.Replace(SpritesFolderPath + "/", "").Replace("/", "_");
        Debug.Log($"CreateAtlas : atlasPath: {spritesFolderPath} , atlasName : {atlasName}");
        string path = $"{atlasFolderPath}/{atlasName}.spriteatlas";
        SpriteAtlas atlas = new SpriteAtlas();

        SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings();
        packingSettings.padding = 4;
        atlas.SetPackingSettings(packingSettings);

        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings();
        textureSettings.readable = true;
        textureSettings.sRGB = true;
        atlas.SetTextureSettings(textureSettings);

        TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
        platformSettings.maxTextureSize = 2048;
        atlas.SetPlatformSettings(platformSettings);

        AssetDatabase.CreateAsset(atlas, path);

        var folderAsset = AssetDatabase.LoadMainAssetAtPath(spritesFolderPath);
        atlas.Add(new UnityEngine.Object[] { folderAsset });

        SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.SaveAssets();
    }
}
