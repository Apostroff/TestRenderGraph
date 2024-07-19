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

    private Camera _camera;
    
    private RendererListDesc _rendererListDesc = new()
    {
      sortingCriteria = SortingCriteria.CommonOpaque,
      renderQueueRange = RenderQueueRange.opaque,
    };

    private RendererListHandle _rendererListHandle;

    public void Setup(Camera camera, RenderGraph renderGraph)
    {
      _camera = camera;
      _rendererListHandle = renderGraph.CreateRendererList(_rendererListDesc);
    }

    public static void Record(Camera camera, RenderGraph renderGraph)
    {
      using RenderGraphBuilder builder = renderGraph.AddRenderPass("Opaque", out OpaqueRenderPass pass);
      pass.Setup(camera, renderGraph);
      builder.SetRenderFunc<OpaqueRenderPass>(RenderCallback);
      builder.Dispose();
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
      _camera = null;
    }
  }
}