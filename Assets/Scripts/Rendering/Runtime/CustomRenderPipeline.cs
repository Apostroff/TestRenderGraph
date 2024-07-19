using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace Rendering.Runtime
{
  public partial class CustomRenderPipeline : RenderPipeline
  {
    private static readonly ProfilingSampler samplerOpaque = new("Opaque Geometry");
    private RenderGraph _renderGraph = new("Custom SRP Render Graph");
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
      var renderGraphParameters = new RenderGraphParameters
      {
          commandBuffer = CommandBufferPool.Get(),
          currentFrameIndex = Time.renderedFrameCount,
          scriptableRenderContext =  context,
          rendererListCulling = false,
      };

      for (var i = 0; i < cameras.Length; i++)
      {
        Render(context, cameras[i], renderGraphParameters);
      }
      
      context.ExecuteCommandBuffer(renderGraphParameters.commandBuffer);
      context.Submit();
      CommandBufferPool.Release(renderGraphParameters.commandBuffer);
    }

    private void Render(ScriptableRenderContext context, Camera camera, RenderGraphParameters parameters)
    {
      if (!camera.TryGetCullingParameters(out var cullingParameters))
      {
        return;
      }
      Vector2Int bufferSize = new Vector2Int(camera.pixelWidth, camera.pixelHeight);
      CameraRenderAttachments attachments = SetupPass.Record(_renderGraph, false, bufferSize, camera);
      
      CullingResults cullingResults = context.Cull(ref cullingParameters);
      
      using (_renderGraph.RecordAndExecute(parameters)) 
      {
        using (RenderGraphBuilder builder = _renderGraph.AddRenderPass("Opaque", out OpaqueRenderPass pass, samplerOpaque))
        {
          pass.Setup(camera, _renderGraph, cullingResults);
          builder.AllowPassCulling(false);
          builder.UseRendererList(pass.RendererListHandle);
          builder.UseColorBuffer(_renderGraph.ImportBackbuffer(BuiltinRenderTextureType.CameraTarget), 0);
          builder.SetRenderFunc<OpaqueRenderPass>(OpaqueRenderPass.RenderCallback);
        }
      }
      
      //_renderGraph.EndFrame();
    }
    
    protected override void Dispose(bool disposing)
    {
      _renderGraph.Cleanup();
      _renderGraph = null;
    }
  }
}