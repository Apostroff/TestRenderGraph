using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

namespace Rendering.Runtime
{
  public class OpaqueRenderPass
  {
    static readonly ShaderTagId[] shaderTagIds = {
        new("SRPDefaultUnlit"),
        new("CustomLit")
    };

    private CameraRenderAttachments _cameraRenderAttachments;

    public RendererListHandle RendererListHandle => _rendererListHandle;

    private RendererListDesc GetRenderListDesc(CullingResults cullingResults, Camera camera)
    {
      return new RendererListDesc(shaderTagIds, cullingResults, camera)
      {
          sortingCriteria = SortingCriteria.CommonOpaque,
          renderQueueRange = RenderQueueRange.opaque,
          rendererConfiguration = PerObjectData.None,
      };
    }

    private RendererListHandle _rendererListHandle;

    public void Setup(Camera camera, RenderGraph renderGraph, CullingResults cullingResults)
    {
      RendererListDesc rendererListDesc = GetRenderListDesc(cullingResults, camera);
      _rendererListHandle = renderGraph.CreateRendererList(rendererListDesc);
    }

    public static void RenderCallback(OpaqueRenderPass pass, RenderGraphContext context)
    {
      pass.Render(context);
    }

    private void Render(RenderGraphContext context)
    {
      context.cmd.DrawRendererList(_rendererListHandle);
      context.renderContext.ExecuteCommandBuffer(context.cmd);
      context.cmd.Clear();
      
      CleanUp();
    }

    private void CleanUp()
    {
    }
  }
}