using Ultraviolet.Core;
using Ultraviolet.Graphics;
using Ultraviolet.Graphics.Graphics3D;

namespace Ultraviolet.OpenGL.Graphics.Graphics3D
{
    /// <summary>
    /// Represents the OpenGL implementation of the <see cref="PbrMetallicRoughnessEffect"/> class.
    /// </summary>
    public sealed class OpenGLPbrMetallicRoughnessEffect : PbrMetallicRoughnessEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLPbrMetallicRoughnessEffect"/> class.
        /// </summary>
        public OpenGLPbrMetallicRoughnessEffect(UltravioletContext uv)
            : base(CreateEffectImplementation(uv))
        {

        }

        /// <summary>
        /// Creates the effect implementation.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <returns>The effect implementation.</returns>
        private static EffectImplementation CreateEffectImplementation(UltravioletContext uv)
        {
            Contract.Require(uv, nameof(uv));

            var program = new OpenGLShaderProgram(uv, vertShader, fragShader, false);
            var passes = new[] { new OpenGLEffectPass(uv, null, program) };
            var techniques = new[] { new OpenGLEffectTechnique(uv, null, passes) };
            return new OpenGLEffectImplementation(uv, techniques);
        }

        // The shaders that make up this effect.
        private static readonly UltravioletSingleton<OpenGLVertexShader> vertShader =
            new UltravioletSingleton<OpenGLVertexShader>(UltravioletSingletonFlags.DisabledInServiceMode, uv => {
                return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("PbrMetallicRoughnessEffect.vert")); });
        private static readonly UltravioletSingleton<OpenGLFragmentShader> fragShader =
            new UltravioletSingleton<OpenGLFragmentShader>(UltravioletSingletonFlags.DisabledInServiceMode, uv => {
                return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("PbrMetallicRoughnessEffect.frag")); });
    }
}
