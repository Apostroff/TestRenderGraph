using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace Rendering.Runtime
{
  public partial class CustomRenderPipeline : RenderPipeline
  {
    private RenderGraph _renderGraph = new("Custom SRP Render Graph");
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
      var renderGraphParameters = new RenderGraphParameters
      {
          commandBuffer = CommandBufferPool.Get(),
          currentFrameIndex = Time.renderedFrameCount,
          scriptableRenderContext =  context,
          rendererListCulling = true,
      };
      
      using (_renderGraph.RecordAndExecute(renderGraphParameters))
      {
        using RenderGraphBuilder builder = _renderGraph.AddRenderPass("Opaque", out OpaqueRenderPass pass);
        pass.Setup(cameras[0], _renderGraph);
        builder.SetRenderFunc<OpaqueRenderPass>(OpaqueRenderPass.RenderCallback);
        
      }
      
      _renderGraph.EndFrame();
    }
    
    protected override void Dispose(bool disposing)
    {
      _renderGraph.Cleanup();
      _renderGraph = null;
    }
  }
}