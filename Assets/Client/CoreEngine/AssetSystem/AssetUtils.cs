
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;

public static class AssetUtils
{
    #region 音频相关

    public static void LoadAudio(this string audioName, Action<AudioClip> onLoaded)
    {
        AssetSystem.LoadAsset<AudioClip>($"Audio/{audioName}", onLoaded);
    }

    public static AudioClip LoadAudio(this string audioName)
    {
        return AssetSystem.LoadAsset<AudioClip>($"Audio/{audioName}");
    }

    #endregion

    #region 材质相关

    public static void LoadMaterial(this string matName, Action<Material> onLoaded)
    {
        AssetSystem.LoadAsset<Material>($"Materials/{matName}", onLoaded);
    }

    public static Material LoadMaterial(this string matName)
    {
        return AssetSystem.LoadAsset<Material>($"Materials/{matName}");
    }

    #endregion

    #region 图片相关

    public static Sprite LoadSpriteByTexture(this string path)
    {
        return AssetSystem.LoadAsset<Sprite>($"Texture/{path}");
    }

    public static void LoadSpriteByTexture(this string path, Action<Sprite> onLoaded)
    {
        AssetSystem.LoadAsset<Sprite>($"Texture/{path}", onLoaded);
    }

    public static async Task<Sprite> LoadSpriteAsyncByTexture(string spritePath)
    {
        var sr = await AssetSystem.LoadAssetAsync<Sprite>($"Texture/{spritePath}");
        return sr;
    }

    public static void LoadTexture(this string path, Action<Texture2D> onLoaded)
    {
        LoadSpriteByTexture(path, (sp) =>
        {
            if (sp == null)
            {
                LogSystem.Error($"Texture not found!!! path: {path}");
                return;
            }

            Texture2D tex = sp.texture;
            onLoaded(tex);
        });
    }

    public static SpriteAtlas LoadAtlas(this string atlasName)
    {
        if (atlasName != "0" && !string.IsNullOrEmpty(atlasName))
        {
            var atlas = AssetSystem.LoadAsset<SpriteAtlas>($"Atlas/{atlasName}");
            if (atlas == null)
            {
                Debug.LogError($"atlas {atlasName} is not found");
            }

            return atlas;
        }
        return null;
    }

    public static void LoadAtlas(this string atlasName, Action<SpriteAtlas> onLoaded, bool unload = true)
    {
        if (atlasName != "0" && !string.IsNullOrEmpty(atlasName))
        {
            AssetSystem.LoadAsset<SpriteAtlas>($"Atlas/{atlasName}", (atlas) =>
            {
                if (atlas == null)
                {
                    Debug.LogError($"atlas {atlasName} is not found");
                    return;
                }
                onLoaded?.Invoke(atlas);
            });
        }
    }

    public static Sprite LoadSpriteByAtlas(this string spriteName, string atlasName)
    {
        SpriteAtlas atlas = spriteName.LoadAtlas();
        if (atlas == null)
        {
            Debug.LogError($"atlas {atlasName} is not found, sprite: {spriteName}");
            return null;
        }
        Sprite sr = atlas.GetSprite(spriteName);
        if (sr == null)
            Debug.LogError($"sprite: {spriteName} not in atlas {atlasName}");
        return sr;
    }

    public static void LoadSpriteByAtlas(this string spriteName, string atlasName, Action<Sprite> onLoaded)
    {
        spriteName.LoadAtlas((prefab) =>
        {
            SpriteAtlas atlas = GameObject.Instantiate(prefab) as SpriteAtlas;

            if (atlas == null)
            {
                Debug.LogError($"atlas {atlasName} is not found, sprite: {spriteName}");
                onLoaded(null);
                return;
            }

            Sprite sr = atlas.GetSprite(spriteName);
            if (sr == null)
                Debug.LogError($"sprite: {spriteName} not in atlas {atlasName}");
            onLoaded(sr);
        });
    }

    public static Sprite LoadSprite(this string url)
    {
        if (string.IsNullOrEmpty(url))
            return null;
        Sprite sr = null;
        if (IsTexture(url))
        {
            sr = LoadSpriteByTexture(url);
        }
        else
        {
            int idx = url.LastIndexOf("/");
            string atlasName = url.Substring(0, idx).Replace("/", "_");
            string spriteName = url.Substring(idx + 1);
            sr = LoadSpriteByAtlas(spriteName, atlasName);
        }
        return sr;
    }

    public static void LoadSprite(this string url, Action<Sprite> onLoaded)
    {
        if (string.IsNullOrEmpty(url))
            return;
        if (IsTexture(url))
        {
            LoadSpriteByTexture(url, (tex) =>
            {
                onLoaded?.Invoke(tex);
            });
        }
        else
        {
            int idx = url.LastIndexOf("/");
            string atlasName = url.Substring(0, idx).Replace("/", "_");
            string spriteName = url.Substring(idx + 1);
            LoadSpriteByAtlas(spriteName, atlasName, onLoaded);
        }
    }

    #endregion

    #region Image相关

    public static void LoadSpriteByTexture(this Image image, string texturePath, Action onLoaded = null)
    {
        image.enabled = false;
        LoadSpriteByTexture(texturePath, sp =>
        {
            image.sprite = sp;
            image.enabled = true;
            onLoaded?.Invoke();
        });
    }

    public static async Task LoadSpriteByTextureAsync(this Image image, string spritePath)
    {
        image.enabled = false;
        var sr = await LoadSpriteAsyncByTexture(spritePath);
        image.sprite = sr;
        image.enabled = true;
    }

    public static void LoadSpriteByAtlas(this Image image, string spriteName, string atlasName, Action callback = null)
    {
        image.enabled = false;
        LoadSpriteByAtlas(spriteName, atlasName, (sprite) =>
        {
            if (sprite == null)
            {
                image.sprite = null;
            }
            else
            {
                image.sprite = sprite;
            }

            image.enabled = true;
            callback?.Invoke();
        });
    }

    public static Task LoadSpriteByAtlasAsync(this Image image, string spriteName, string atlasName)
    {
        var tcs = new TaskCompletionSource<bool>();
        image.enabled = false;
        LoadSpriteByAtlas(spriteName, atlasName, (sprite) =>
        {
            image.sprite = sprite;
            tcs.SetResult(true);
            image.enabled = true;
        });
        return tcs.Task;
    }

    public static void LoadSprite(this Image image, string url, Action callback = null)
    {
        if (string.IsNullOrEmpty(url))
            return;
        if (IsTexture(url))
        {
            LoadSpriteByTexture(image, url, callback);
        }
        else
        {
            int idx = url.LastIndexOf("/");
            string atlasName = url.Substring(0, idx).Replace("/", "_");
            string spriteName = url.Substring(idx + 1);
            LoadSpriteByAtlas(image, spriteName, atlasName, callback);
        }
    }

    public static async Task LoadSpriteAsync(this Image image, string url)
    {
        if (string.IsNullOrEmpty(url))
            return;
        if (IsTexture(url))
        {
            image.enabled = false;
            image.sprite = await AssetSystem.LoadAssetAsync<Sprite>($"Texture/{url}");
            image.enabled = true;
        }
        else
        {
            int idx = url.LastIndexOf("/");
            string atlasName = url.Substring(0, idx).Replace("/", "_");
            string spriteName = url.Substring(idx + 1);
            await LoadSpriteByAtlasAsync(image, spriteName, atlasName);
        }
    }

    public static void LoadTexture(this RawImage rawImage, string texturePath, Action onLoaded = null)
    {
        rawImage.enabled = false;
        texturePath.LoadTexture(tex =>
        {
            rawImage.texture = tex;
            rawImage.enabled = true;
            onLoaded?.Invoke();
        });
    }

    public static void LoadSprite(this RawImage image, string url, Action callback = null)
    {
        if (IsTexture(url))
        {
            LoadTexture(image, url, callback);
        }
        else
        {
            int idx = url.LastIndexOf("/");
            string atlasName = url.Substring(0, idx).Replace("/", "_");
            string spriteName = url.Substring(idx + 1);
            Debug.Log($"atlas: {atlasName},sprite: {spriteName}");
            image.enabled = false;
            LoadSpriteByAtlas(spriteName, atlasName, (sprite) =>
            {
                if (sprite == null)
                {
                    image.texture = null;
                }
                else
                {
                    image.texture = sprite.texture;
                    image.enabled = true;
                }

                callback?.Invoke();
            });
        }
    }

    public static void LoadSprite(this SpriteRenderer renderer, string url, Action callback = null)
    {
        LoadSprite(url, sp =>
        {
            if (renderer != null)
            {
                renderer.sprite = sp;
            }
            callback?.Invoke();
        });
    }

    public static void LoadSprite(this Graphic graphic, string url, Action callback = null)
    {
        if (graphic is Image image)
        {
            image.LoadSprite(url, callback);
        }
        else if (graphic is RawImage rawImage)
        {
            rawImage.LoadSprite(url,callback);
        }
    }

    private static bool IsTexture(string url)
    {

        string path1 = $"Assets/Bundle/Texture/{url}.png";
        string path2 = $"Assets/Bundle/Texture/{url}.jpg";
        return YooAssets.CheckLocationValid(path1) || YooAssets.CheckLocationValid(path2);
    }

    #endregion
}
