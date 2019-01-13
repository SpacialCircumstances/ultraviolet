using System;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents a <see cref="Material"/> for <see cref="BasicEffect"/>.
    /// </summary>
    public sealed class BasicMaterial : Material
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicMaterial"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="effect">The effect which this material encapsulates.</param>
        private BasicMaterial(UltravioletContext uv, BasicEffect effect)
            : base(uv, effect)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="BasicMaterial"/> class.
        /// </summary>
        /// <param name="effect">The effect which the material encapsulates, or <see langword="null"/> to use the effect's singleton instance.</param>
        /// <returns>The instance of <see cref="PbrMetallicRoughnessMaterial"/> that was created.</returns>
        public static BasicMaterial Create(BasicEffect effect)
        {
            return new BasicMaterial(UltravioletContext.DemandCurrent(), effect);
        }

        /// <summary>
        /// Gets or sets the material's diffuse color.
        /// </summary>
        public Color DiffuseColor { get; set; }

        /// <summary>
        /// Gets or sets the material's texture.
        /// </summary>
        public Texture2D Texture { get; set; }
        
        /// <inheritdoc/>
        protected override void OnBegin(Camera camera, ref Matrix worldMatrix)
        {
            base.OnBegin(camera, ref worldMatrix);

            var basicEffect = (BasicEffect)Effect;

            basicEffect.SrgbColor = basicEffect.Ultraviolet.GetGraphics().Capabilities.SrgbEncodingEnabled;
            basicEffect.TextureEnabled = Texture != null;
            basicEffect.Texture = Texture;
            basicEffect.VertexColorEnabled = false;
            basicEffect.DiffuseColor = DiffuseColor;

            camera.GetViewMatrix(out var view);
            camera.GetProjectionMatrix(out var proj);

            basicEffect.World = worldMatrix;
            basicEffect.View = view;
            basicEffect.Projection = proj;
        }

        /// <inheritdoc/>
        protected override void OnPassApplied(Int32 passIndex)
        {
            base.OnPassApplied(passIndex);
        }
    }
}
