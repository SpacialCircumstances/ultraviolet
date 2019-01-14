using Ultraviolet.Core;
using Ultraviolet.Graphics;

namespace Ultraviolet.OpenGL.Graphics.Graphics2D
{
    /// <summary>
    /// Represents the OpenGL implementation of the <see cref="Ultraviolet.Graphics.BasicEffect"/> class.
    /// </summary>
    public sealed class OpenGLBasicEffect : BasicEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLBasicEffect"/> class.
        /// </summary>
        public OpenGLBasicEffect(UltravioletContext uv)
            : base(CreateEffectImplementation(uv))
        {

        }

        /// <inheritdoc/>
        protected override void OnVertexColorEnabledChanged() => UpdateCurrentTechnique();

        /// <inheritdoc/>
        protected override void OnTextureEnabledChanged() => UpdateCurrentTechnique();

        /// <summary>
        /// Creates the effect implementation.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <returns>The effect implementation.</returns>
        private static EffectImplementation CreateEffectImplementation(UltravioletContext uv)
        {
            Contract.Require(uv, nameof(uv));

            var techniques = new[]
            {
                new OpenGLEffectTechnique(uv, "Position",
                    new[] { new OpenGLEffectPass(uv, null, new OpenGLShaderProgram(uv, vertShader, fragShader, false)) }),
                new OpenGLEffectTechnique(uv, "PositionColor",
                    new[] { new OpenGLEffectPass(uv, null, new OpenGLShaderProgram(uv, vertShaderColored, fragShaderColored, false)) }),
                new OpenGLEffectTechnique(uv, "PositionTexture",
                    new[] { new OpenGLEffectPass(uv, null, new OpenGLShaderProgram(uv, vertShaderTextured, fragShaderTextured, false)) }),
                new OpenGLEffectTechnique(uv, "PositionColorTexture",
                    new[] { new OpenGLEffectPass(uv, null, new OpenGLShaderProgram(uv, vertShaderColoredTextured, fragShaderColoredTextured, false)) }),
            };
            return new OpenGLEffectImplementation(uv, techniques);
        }

        /// <summary>
        /// Changes the current technique for this effect based on its property values.
        /// </summary>
        private void UpdateCurrentTechnique()
        {
            var index = 0;

            if (TextureEnabled)
            {
                index += 2;
            }

            if (VertexColorEnabled)
            {
                index += 1;
            }

            CurrentTechnique = Techniques[index];
        }

        // Shaders - basic
        private static readonly UltravioletSingleton<OpenGLVertexShader> vertShader = 
            new UltravioletSingleton<OpenGLVertexShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect.vert")); });
        private static readonly UltravioletSingleton<OpenGLFragmentShader> fragShader = 
            new UltravioletSingleton<OpenGLFragmentShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffect.frag")); });

        // Shaders - colored
        private static readonly UltravioletSingleton<OpenGLVertexShader> vertShaderColored = 
            new UltravioletSingleton<OpenGLVertexShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffectColored.vert")); });
        private static readonly UltravioletSingleton<OpenGLFragmentShader> fragShaderColored = 
            new UltravioletSingleton<OpenGLFragmentShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffectColored.frag")); });

        // Shaders - textured
        private static readonly UltravioletSingleton<OpenGLVertexShader> vertShaderTextured = 
            new UltravioletSingleton<OpenGLVertexShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffectTextured.vert")); });
        private static readonly UltravioletSingleton<OpenGLFragmentShader> fragShaderTextured = 
            new UltravioletSingleton<OpenGLFragmentShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffectTextured.frag")); });

        // Shaders - colored & textured
        private static readonly UltravioletSingleton<OpenGLVertexShader> vertShaderColoredTextured = 
            new UltravioletSingleton<OpenGLVertexShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffectColoredTextured.vert")); });
        private static readonly UltravioletSingleton<OpenGLFragmentShader> fragShaderColoredTextured = 
            new UltravioletSingleton<OpenGLFragmentShader>(UltravioletSingletonFlags.DisabledInServiceMode, 
                uv => { return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("BasicEffectColoredTextured.frag")); });
    }
}
