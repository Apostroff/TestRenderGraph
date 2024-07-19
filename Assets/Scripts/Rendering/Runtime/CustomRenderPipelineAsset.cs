using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering.Runtime
{
  [CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
  public partial class CustomRenderPipelineAsset : RenderPipelineAsset
  {

    protected override RenderPipeline CreatePipeline()
    {
      return new CustomRenderPipeline();
    }
  }
}