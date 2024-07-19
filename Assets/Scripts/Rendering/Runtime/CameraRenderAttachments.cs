using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace Rendering.Runtime
{
  public class CameraRenderAttachments
  {
    public readonly TextureHandle colorAttachment, depthAttachment;

    public CameraRenderAttachments(TextureHandle colorAttachment, TextureHandle depthAttachment)
    {
      this.colorAttachment = colorAttachment;
      this.depthAttachment = depthAttachment;
    }
  }
}