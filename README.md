# TransparentBackgroundRenderer
Tested with Unity 2023.2.0a15 for Android and iOS builds (on several devices I have at hand).

This is an example project of a script which allows rendering of tranparent background textures with post processes into a RenderTexture

Currently there is no built-in solution to render camera with enabled post processes and transparent (zero alpha with solid color) background into a RenderTexture.
Script and shader in this project helps with that (with some limitations).

Main logic behind this is rendering from camera with solid green color to a _source_ RenderTexture, then copying this result into _target_ RenderTexture via some shader which switches solid green color with zero alpha. Important note here is that there is no alpha in the _source_ due to the way Unity renders post processes. It's calculated afterwards.

This logic happens in LateUpdate method. You might want to move it to a CustomRenderPass or somewhere else if you want to get fancy. For me executing this in LateUpdate gets things done and that's enough.

    private void LateUpdate()
    {
        var cmd = CommandBufferPool.Get();

        Blitter.BlitCameraTexture(cmd, _sourceRTHandle, _targetRTHandle, _material, 0);

        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Clear();
    }

As I stated about alpha value is calculated on the fly, so depending on a material you use or post process effects you apply result may vary (small or big green outline around objects).
In some cases it's possible to fix by changing provided 0.4f value in a shader (highlighted in bold below). For example, in my own project I use 0.8f.
In other cases fiddling with that value will not help you which means you are doomed until Unity provides a built-in solution (or maybe you come up with a better idea).

    float get_inverted_color_mask(const float3 color)
    {
        const float3 mask_color = float3(0, 1, 0);
        return abs(1 - saturate(1 - (distance(mask_color, color) - **0.4f**) / 1e-5));
    }
