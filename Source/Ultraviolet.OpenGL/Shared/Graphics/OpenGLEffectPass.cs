using System;
using System.Collections.Generic;
using Ultraviolet.Core;
using Ultraviolet.Graphics;

namespace Ultraviolet.OpenGL.Graphics
{
    /// <summary>
    /// Represents the OpenGL implementation of the EffectPass class.
    /// </summary>
    public sealed class OpenGLEffectPass : EffectPass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectPass"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="name">The effect pass' name.</param>
        /// <param name="program">The effect pass' shader program.</param>
        public OpenGLEffectPass(UltravioletContext uv, String name, OpenGLShaderProgram program)
            : base(uv)
        {
            Contract.Require(program, nameof(program));

            this.Name = name ?? String.Empty;
            this.Program = program;
        }

        /// <inheritdoc/>
        public override void Apply()
        {
            OpenGLState.UseProgram(Program);

            base.Apply();

            foreach (var uniform in Program.Uniforms)
            {
                uniform.Apply();
            }
        }

        /// <inheritdoc/>
        public override String Name { get; }

        /// <summary>
        /// Gets the effect pass' shader program.
        /// </summary>
        public OpenGLShaderProgram Program { get; }
    }
}
