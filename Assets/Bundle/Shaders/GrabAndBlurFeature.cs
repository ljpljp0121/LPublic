
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class GrabAndBlurFeature : ScriptableRendererFeature
{
    public enum BlurMethod
    {
        //原始版本算法
        Original,
        //原始版本算法使用了DrawMesh接口绘制
        OriginalWithDrawMesh,
        //DualKawase算法
        DualKawase,
        //Grainy 
        Grainy,
    }

    /// <summary>
    /// 所有模糊参数
    /// </summary>
    [Serializable]
    public class GrabAndBlurSetting
    {
        [Header("模糊算法")]
        public BlurMethod curSelectMethod;

        [FormerlySerializedAs("oriBlurShader")]
        [Header("原始模糊算法")]
        //着色器和材质实例
        public Material oriBlurMat;
        [FormerlySerializedAs("oriBlurWithMeshShader")]
        public Material oriBlurWithMeshMat;
        //降采样次数
        [Range(0, 6), Tooltip("[降采样次数]向下采样的次数。此值越大,则采样间隔越大,需要处理的像素点越少,运行速度越快。")]
        public int DownSampleNum = 2;

        //模糊扩散度
        [Range(0.0f, 20.0f), Tooltip("[模糊扩散度]进行高斯模糊时，相邻像素点的间隔。此值越大相邻像素间隔越远，图像越模糊。但过大的值会导致失真。")]
        public float BlurSpreadSize = 3.0f;

        //迭代次数
        [Range(0, 8), Tooltip("[迭代次数]此值越大,则模糊操作的迭代次数越多，模糊效果越好，但消耗越大。")]
        public int BlurIterations = 3;

        [Header("Dual Kawase模糊算法相关设置")]
        public Material dualKawaseBlur;
        [Range(0.0f, 5.0f), Tooltip("[模糊计算半径]计算模糊的半径范围。此值越大,效果越模糊,否则反之。")]
        public float dualKawaseBlurRadius = 0.5f;
        [Range(1, 10), Tooltip("[模糊计算迭代次数]计算模糊的迭代次数。此值越大,效果越模糊,性能消耗越大,但非线性增长。")]
        public int dualKawaseIteration = 6;
        [Range(1, 16), Tooltip("[初始计算降采样比例]计算模糊的初始降采样比例。此值越大,初始采样RT分辨率越低，效果越模糊,性能越好,否则反之。")]
        public float dualKawaseRTDownScaling = 2.0f;

        [Header("Grainy模糊算法相关设置")]
        public Material grainyBlur;
        [Range(0.0f, 50.0f), Tooltip("[模糊计算半径]计算模糊的半径范围。此值越大,效果越模糊,否则反之。")]
        public float grainyBlurRadius;

        [Range(1, 8), Tooltip("[模糊计算迭代次数]计算模糊的迭代次数。此值越大,效果越模糊,性能消耗越大。")]
        public int grainyIteration;

        [Range(1, 10), Tooltip("[初始计算降采样比例]计算模糊的初始降采样比例。此值越大,初始采样RT分辨率越低，效果越模糊,性能越好,否则反之。")]
        public int grainyRTDownScaling;
        [Header("杂项设置")]
        public Material copyColorShader;
        public void InitDefaultSettings()
        {
            // oriBlurMat = Shader.Find("RapidBlurEffect");
            // oriBlurWithMeshMat = Shader.Find("Hidden/AOI/Post/RapidBlurEffectUseMesh");
            // dualKawaseBlur = Shader.Find("Hidden/AOI/Post/DualKawaseBlur");
            // grainyBlur = Shader.Find("Hidden/AOI/Post/GrainyBlur");
            // copyColorShader = Shader.Find("Hidden/AOI/Common/CopyColor");
        }

    }

    public class GrabAndBlurPass : ScriptableRenderPass
    {
        public GrabAndBlurPass()
        {
            InitScreenBlurParams();
        }

        public GrabAndBlurSetting setting;

        public void ReSetVFXBlurSetting(GrabAndBlurSetting overlaySetting)
        {
            //todo: AOIEffect Hide material Inspector
            this.setting.curSelectMethod = overlaySetting.curSelectMethod;
            this.setting.DownSampleNum = overlaySetting.DownSampleNum;
            this.setting.BlurSpreadSize = overlaySetting.BlurSpreadSize;
            this.setting.BlurIterations = overlaySetting.BlurIterations;
            this.setting.dualKawaseBlurRadius = overlaySetting.dualKawaseBlurRadius;
            this.setting.dualKawaseIteration = overlaySetting.dualKawaseIteration;
            this.setting.dualKawaseRTDownScaling = overlaySetting.dualKawaseRTDownScaling;
            this.setting.grainyBlurRadius = overlaySetting.grainyBlurRadius;
            this.setting.grainyIteration = overlaySetting.grainyIteration;
            this.setting.grainyRTDownScaling = overlaySetting.grainyRTDownScaling;
        }

        #region OriginalBlur
        private Material oriBlruMat;
        Material OriBlurMat
        {
            get
            {
                if (oriBlruMat == null)
                {
                    oriBlruMat = new Material(setting.oriBlurMat)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return oriBlruMat;
            }
        }

        private Material oriBlruWithMeshMat;
        Material OriBlurWithMeshMat
        {
            get
            {
                if (oriBlruWithMeshMat == null)
                {
                    oriBlruWithMeshMat = new Material(setting.oriBlurWithMeshMat)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return oriBlruWithMeshMat;
            }
        }
        #endregion

        #region Dual Kawase Params
        private Material m_dualkawaseBlurMat;
        Material DualKawaseBlurMat
        {
            get
            {
                if (setting == null) return null;
                if (m_dualkawaseBlurMat == null)
                {
                    var dualkawaseBlur = setting.dualKawaseBlur;
                    m_dualkawaseBlurMat = new Material(dualkawaseBlur)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return m_dualkawaseBlurMat;
            }
        }
        private int[] m_kawaseBlurMipUp;
        private int[] m_kawaseBlurMipDown;


        #endregion

        #region GrainyBlur Params
        private Material m_grainyBlurMat;
        Material GrainyBlurMat
        {
            get
            {
                if (setting == null) return null;
                if (m_grainyBlurMat == null)
                {
                    var grainyBlur = setting.grainyBlur;
                    m_grainyBlurMat = new Material(grainyBlur)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return m_grainyBlurMat;
            }
        }
        #endregion

        #region Other Params
        private Material m_copyColMat;
        Material CopyColMaterial
        {
            get
            {
                if (setting == null || setting.copyColorShader == null) return null;
                if (m_copyColMat == null)
                {
                    m_copyColMat = new Material(setting.copyColorShader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return m_copyColMat;
            }
        }

        public RenderTargetIdentifier source;

        private Mesh m_FullscreenTriangle;
        /// <summary>
        /// 用于全屏绘制优化的程序化三角形
        /// </summary>
        public Mesh FullscreenTriangle
        {
            get
            {
                if (m_FullscreenTriangle != null)
                    return m_FullscreenTriangle;

                m_FullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };

                // Because we have to support older platforms (GLES2/3, DX9 etc) we can't do all of
                // this directly in the vertex shader using vertex ids :(
                m_FullscreenTriangle.SetVertices(new List<Vector3>
                {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f,  3f, 0f),
                    new Vector3( 3f, -1f, 0f)
                });
                m_FullscreenTriangle.SetIndices(new[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
                m_FullscreenTriangle.UploadMeshData(false);

                return m_FullscreenTriangle;
            }
        }

        /// <summary>
        /// 算法最大迭代次数
        /// </summary>
        private readonly int k_MaxPyramidSize = 16;

        #endregion

        public void InitScreenBlurParams()
        {
            m_kawaseBlurMipUp = new int[k_MaxPyramidSize];
            m_kawaseBlurMipDown = new int[k_MaxPyramidSize];
            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                m_kawaseBlurMipUp[i] = Shader.PropertyToID("_BlurMipUp" + i);
                m_kawaseBlurMipDown[i] = Shader.PropertyToID("_BlurMipDown" + i);
            }
        }

        RenderTexture curFrameResultRT = null;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (curFrameResultRT != null)
            {
                curFrameResultRT.Release();
                curFrameResultRT = null;
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            GrabTexture(context, renderingData);
            CommandBuffer cm = CommandBufferPool.Get("GrabAndBlur");
            switch (setting.curSelectMethod)
            {
                //原始算法版本，最高兼容性
                case BlurMethod.Original:
                    using (new ProfilingScope(cm, new ProfilingSampler("Original Blur")))
                    {
                        OriginalBlur(cm, renderingData, out curFrameResultRT, false);
                    }
                    break;
                //原始算法版本，使用新的绘制接口，shadermode 3.0以上
                case BlurMethod.OriginalWithDrawMesh:
                    using (new ProfilingScope(cm, new ProfilingSampler("OriginalWithDrawMesh Blur")))
                    {
                        OriginalBlur(cm, renderingData, out curFrameResultRT, true);
                        // DualBlur(cm, renderingData);
                    }
                    break;
                //DualKawase算法版本，使用新的绘制接口，shadermode 3.0以上
                case BlurMethod.DualKawase:
                    using (new ProfilingScope(cm, new ProfilingSampler("DualKawase Blur")))
                    {
                        DualKawaseBlur(cm, renderingData, out curFrameResultRT);
                    }
                    break;
                //Grainy算法版本，使用新的绘制接口，shadermode 3.0以上
                case BlurMethod.Grainy:
                    using (new ProfilingScope(cm, new ProfilingSampler("Grainy Blur")))
                    {
                        GrainyBlur(cm, renderingData, out curFrameResultRT);
                    }
                    break;
            }
            cm.SetRenderTarget(source, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
            cm.SetGlobalTexture(Shader.PropertyToID("_GrabAndBlurTex"), curFrameResultRT);
            context.ExecuteCommandBuffer(cm);
            CommandBufferPool.Release(cm);

            foreach (var setter in BlitRtSetters)
            {
                setter.SetRT(curFrameResultRT);
            }
            //BlitRtSetters.Clear();
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {

        }

        private void GrabTexture(in ScriptableRenderContext context, RenderingData renderingData)
        {
            if (BlitGrabRtSetters.Count > 0)
            {
                CommandBuffer cm = CommandBufferPool.Get("UIGrab");
                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;
                foreach (var item in BlitGrabRtSetters)
                {
                    cm.Blit(source, item.setter(opaqueDesc));
                }
                context.ExecuteCommandBuffer(cm);
                CommandBufferPool.Release(cm);
                BlitGrabRtSetters.Clear();
            }
        }


        /// <summary>
        /// AOI Proj 原始版本Blur算法，类似于Kawase计算方法
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="renderingData"></param>
        private void OriginalBlur(CommandBuffer cm, RenderingData renderingData, out RenderTexture resultRT, bool isUseDrawMesh = false)
        {
            float widthMod = 1.0f / (1.0f * (1 << setting.DownSampleNum));

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.width >>= setting.DownSampleNum;
            opaqueDesc.height >>= setting.DownSampleNum;
            opaqueDesc.depthBufferBits = 0;

            int blur1 = Shader.PropertyToID("_Blur1");
            int blur2 = Shader.PropertyToID("_Blur2");
            cm.GetTemporaryRT(blur1, opaqueDesc);
            cm.GetTemporaryRT(blur2, opaqueDesc);

            if (isUseDrawMesh)
            {
                OriBlurWithMeshMat.SetFloat("_DownSampleValue", setting.BlurSpreadSize * widthMod);
                BlitFullscreenTriangle(cm, source, blur1, OriBlurWithMeshMat, 0);
            }
            else
            {
                OriBlurMat.SetFloat("_DownSampleValue", setting.BlurSpreadSize * widthMod);
                cm.Blit(source, blur1, OriBlurMat, 0);
            }

            for (int i = 0; i < setting.BlurIterations; i++)
            {
                float iterationOffs = (i * 1.0f);
                if (isUseDrawMesh)
                {
                    OriBlurWithMeshMat.SetFloat("_DownSampleValue", setting.BlurSpreadSize * widthMod + iterationOffs);
                    BlitFullscreenTriangle(cm, blur1, blur2, OriBlurWithMeshMat, 1);
                    BlitFullscreenTriangle(cm, blur2, blur1, OriBlurWithMeshMat, 2);
                }
                else
                {
                    OriBlurMat.SetFloat("_DownSampleValue", setting.BlurSpreadSize * widthMod + iterationOffs);
                    cm.Blit(blur1, blur2, OriBlurMat, 1);
                    cm.Blit(blur2, blur1, OriBlurMat, 2);
                }
            }

            resultRT = RenderTexture.GetTemporary(opaqueDesc);
            if (isUseDrawMesh)
            {
                BlitFullscreenTriangle(cm, blur1, resultRT, CopyColMaterial);
            }
            else
            {
                cm.Blit(blur1, resultRT);
            }

            cm.ReleaseTemporaryRT(blur1);
            cm.ReleaseTemporaryRT(blur2);
            // RenderTexture.ReleaseTemporary(blur1);
            // RenderTexture.ReleaseTemporary(blur2);
        }

        /// <summary>
        /// Dual Kawase Blur 
        /// </summary>
        /// <remarks> Ref: SIGGRAPH-2015: Bandwidth-Efficient Rendering by ARM </remarks>
        /// <param name="cmd"> CommandBuffer </param>
        /// <param name="renderingData"> RenderingData </param>
        private void DualKawaseBlur(CommandBuffer cmd, RenderingData renderingData, out RenderTexture resultRT)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            int tw = Mathf.FloorToInt(desc.width / setting.dualKawaseRTDownScaling);
            int th = Mathf.FloorToInt(desc.height / setting.dualKawaseRTDownScaling);
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            desc.width = tw;
            desc.height = th;

            var offsetID = Shader.PropertyToID("_Offset");
            DualKawaseBlurMat.SetFloat(offsetID, setting.dualKawaseBlurRadius);

            // Downsample - gaussian pyramid
            RenderTargetIdentifier lastDown = source;
            for (int i = 0; i < setting.dualKawaseIteration; i++)
            {
                int mipDown = m_kawaseBlurMipDown[i];
                int mipUp = m_kawaseBlurMipUp[i];

                desc.width = tw;
                desc.height = th;

                cmd.GetTemporaryRT(mipDown, desc, FilterMode.Bilinear);
                cmd.GetTemporaryRT(mipUp, desc, FilterMode.Bilinear);

                BlitFullscreenTriangle(cmd, lastDown, mipDown, DualKawaseBlurMat);
                lastDown = mipDown;

                tw = Mathf.Max(1, tw >> 1);
                th = Mathf.Max(1, th >> 1);

            }

            // Upsample (bilinear by default, HQ filtering does bicubic instead
            int lastUp = m_kawaseBlurMipDown[setting.dualKawaseIteration - 1];
            for (int i = setting.dualKawaseIteration - 2; i >= 0; i--)
            {
                int mipUp = m_kawaseBlurMipUp[i];
                BlitFullscreenTriangle(cmd, lastUp, mipUp, DualKawaseBlurMat, 1);
                lastUp = mipUp;
            }

            var finalDesc = renderingData.cameraData.cameraTargetDescriptor;
            finalDesc.depthBufferBits = 0;
            finalDesc.msaaSamples = 1;
            finalDesc.width = Mathf.FloorToInt(finalDesc.width / setting.dualKawaseRTDownScaling);
            finalDesc.height = Mathf.FloorToInt(finalDesc.height / setting.dualKawaseRTDownScaling);
            resultRT = RenderTexture.GetTemporary(finalDesc);
            BlitFullscreenTriangle(cmd, BuiltinRenderTextureType.CurrentActive, resultRT, DualKawaseBlurMat, 1);

            for (int i = 0; i < setting.dualKawaseIteration; i++)
            {
                cmd.ReleaseTemporaryRT(m_kawaseBlurMipDown[i]);
                cmd.ReleaseTemporaryRT(m_kawaseBlurMipUp[i]);
            }
        }

        /// <summary>
        /// Grainy Blur
        /// </summary>
        /// <remarks> Ref: https://www.shadertoy.com/view/Mt3czf by ShaderToy </remarks>
        /// <param name="cmd"></param>
        /// <param name="renderingData"></param>
        private void GrainyBlur(CommandBuffer cmd, RenderingData renderingData, out RenderTexture resultRT)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            int tw = Mathf.FloorToInt(desc.width / setting.grainyRTDownScaling);
            int th = Mathf.FloorToInt(desc.height / setting.grainyRTDownScaling);
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            desc.width = tw;
            desc.height = th;

            var offsetID = Shader.PropertyToID("_Params");
            GrainyBlurMat.SetVector(offsetID, new Vector2(setting.grainyBlurRadius / th, setting.grainyIteration));

            // Downsample - gaussian pyramid
            // RenderTargetIdentifier lastDown = source;
            var bufferID = Shader.PropertyToID("_BufferRT");
            cmd.GetTemporaryRT(bufferID, desc, FilterMode.Bilinear);
            BlitFullscreenTriangle(cmd, source, bufferID, GrainyBlurMat);
            // cmd.Blit(source, bufferID, GrainyBlurMat);
            resultRT = RenderTexture.GetTemporary(desc);
            BlitFullscreenTriangle(cmd, BuiltinRenderTextureType.CurrentActive, resultRT, CopyColMaterial);
            // cmd.Blit(bufferID, resultRT);

            cmd.ReleaseTemporaryRT(bufferID);
        }

        void BlitFullscreenTriangle(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material useMat, int pass = 0, bool clear = false, Rect? viewport = null, bool useDrawProcedual = false)
        {
            using (new ProfilingScope(cmd, new ProfilingSampler("BlitFullscreenTriangle")))
            {
                cmd.BeginSample("SetGlobalTexture");
                int mainTexID = Shader.PropertyToID("_MainTex");
                cmd.SetGlobalTexture(mainTexID, source);
                cmd.EndSample("SetGlobalTexture");

                cmd.BeginSample("SetRenderTarget");
                cmd.SetRenderTarget(destination, viewport == null ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
                cmd.EndSample("SetRenderTarget");

                if (viewport != null)
                    cmd.SetViewport(viewport.Value);

                if (clear)
                    cmd.ClearRenderTarget(true, true, Color.clear);

                if (useMat == null)
                {
                    Debug.LogError("BlitFullscreenTriangle need a material but get a null one !");
                    return;
                }

                if (useDrawProcedual)
                {
                    //Draw DrawProcedural cast more performance In some case
                    cmd.BeginSample("DrawProcedural");
                    cmd.DrawProcedural(Matrix4x4.identity, useMat, pass, MeshTopology.Triangles, 3, 1, null);
                    cmd.EndSample("DrawProcedural");
                }
                else
                {
                    cmd.BeginSample("DrawMeshTriangle");
                    cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, useMat, 0, pass);
                    cmd.EndSample("DrawMeshTriangle");
                }
            }
        }

    }

    public class BlurRTSetter
    {
        private Action<RenderTexture> setter;
        public BlurRTSetter(Action<RenderTexture> action)
        {
            setter = action;
        }

        public void SetRT(RenderTexture renderTexture)
        {
            setter?.Invoke(renderTexture);
        }
    }

    public class GrabRTSetter
    {
        public Func<RenderTextureDescriptor, RenderTexture> setter { get; private set; }
        public GrabRTSetter(Func<RenderTextureDescriptor, RenderTexture> rt)
        {
            setter = rt;
        }
    }

    public static HashSet<BlurRTSetter> BlitRtSetters = new HashSet<BlurRTSetter>();
    public static HashSet<GrabRTSetter> BlitGrabRtSetters = new HashSet<GrabRTSetter>();

    public GrabAndBlurSetting setting;

    GrabAndBlurPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new GrabAndBlurPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents,
            setting = setting,
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        scriptablePass.source = renderer.cameraColorTarget;
        renderer.EnqueuePass(scriptablePass);
    }
}
