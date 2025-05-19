using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderToyManager : MonoBehaviour
{
    public Shader ShaderToy;

    private Material mat;
    public Material Mat
    {
        get
        {
            if (ShaderToy == null)
            {
                Debug.LogError("ShaderToy is null");
                return null;
            }

            if (!ShaderToy.isSupported)
            {
                Debug.LogError("ShaderToy is not supported");
                return null;
            }

            if (mat == null)
            {
                Material newMat = new Material(ShaderToy);
                newMat.hideFlags = HideFlags.HideAndDontSave;
                mat = newMat;
                return mat;
            }
            else
            {
                return mat;
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Mat);
    }
}