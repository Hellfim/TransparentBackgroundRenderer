using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering
{
    public class TransparentBackgroundRenderer : MonoBehaviour
    {
        private static RTHandle _targetRTHandle;
        
        [SerializeField]
        private Camera _camera;
        
        [SerializeField]
        private Shader _shader;
        
        [SerializeField]
        private RenderTexture _targetTexture;

        private Material _material;
        private RTHandle _sourceRTHandle;
        private RenderTexture _sourceTexture;

        private void Awake()
        {
            _material = CoreUtils.CreateEngineMaterial(_shader);
            if (_targetTexture != null)
            {
                _targetRTHandle ??= RTHandles.Alloc(_targetTexture);

                _sourceTexture = new RenderTexture(_targetTexture.descriptor);
                _sourceRTHandle = RTHandles.Alloc(_sourceTexture);
                
                _camera.targetTexture = _sourceTexture;
                
                _material.SetTexture(Shader.PropertyToID("_Texture"), _sourceTexture);
            }
        }

        private void OnDestroy()
        {
            /*
             * _targetRTHandle should be released only if it's not an asset reference and created dynamically (like sourceTexture here).
             * Static asset objects are destroyed in builds (in the Editor it's fine) when RTHandle gets released, so if you release _targetRTHandle,
             * reload scene with this script and try to to use _targetTexture you will get 'null' as _targetTexture asset will be removed in-game
             *
             * That's also the sole reason why _targetTexture has static modifier in this script
             */
            RTHandles.Release(_sourceRTHandle);
            //No need to manually destroy _sourceTexture here, because RTHandles.Release(_sourceRTHandle) call destroys it internally
        }

        private void LateUpdate()
        {
            var cmd = CommandBufferPool.Get();

            Blitter.BlitCameraTexture(cmd, _sourceRTHandle, _targetRTHandle, _material, 0);

            Graphics.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}