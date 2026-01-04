
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TriInspector;
using SerapKeremGameKit._Singletons;
using SerapKeremGameKit._Logging;

namespace SerapKeremGameKit._Managers
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoSingleton<GameManager>
    {
        [Title("URP Settings")]
        [SerializeField]
        [Range(0f, 200f)]
        private float _shadowDistance = 150f;

        [ReadOnly]
        [SerializeField]
        private UniversalRenderPipelineAsset _urpAsset;

        public UniversalRenderPipelineAsset URPAsset => _urpAsset;

        protected override void Awake()
        {
            base.Awake();
            InitializeURPSettings();
        }

        private void InitializeURPSettings()
        {
            RenderPipelineAsset renderPipeline = GraphicsSettings.defaultRenderPipeline;

            _urpAsset = renderPipeline as UniversalRenderPipelineAsset;

            if (_urpAsset == null)
            {
                TraceLogger.LogError("[GameManager] Active Render Pipeline is not URP.");
                return;
            }

            QualitySettings.shadowDistance = _shadowDistance;
        }
    }
}