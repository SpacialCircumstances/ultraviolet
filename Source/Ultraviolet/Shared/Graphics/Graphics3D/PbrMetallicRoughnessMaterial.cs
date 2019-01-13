using System;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents a <see cref="Material"/> for <see cref="PbrMetallicRoughnessEffect"/>.
    /// </summary>
    public sealed class PbrMetallicRoughnessMaterial : PbrMaterial
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PbrMetallicRoughnessMaterial"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="effect">The effect which this material encapsulates.</param>
        private PbrMetallicRoughnessMaterial(UltravioletContext uv, PbrMetallicRoughnessEffect effect)
            : base(uv, effect)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="PbrMetallicRoughnessMaterial"/> class.
        /// </summary>
        /// <param name="effect">The effect which the material encapsulates, or <see langword="null"/> to use the effect's singleton instance.</param>
        /// <returns>The instance of <see cref="PbrMetallicRoughnessMaterial"/> that was created.</returns>
        public static PbrMetallicRoughnessMaterial Create(PbrMetallicRoughnessEffect effect = null)
        {
            return new PbrMetallicRoughnessMaterial(UltravioletContext.DemandCurrent(), effect ?? PbrMetallicRoughnessEffect.Instance);
        }

        /// <summary>
        /// Gets or sets the base color of the material. If a <see cref="BaseColorTexture"/> is specified,
        /// this value is multiplied with the texel values.
        /// </summary>
        public Color BaseColorFactor { get; set; }

        /// <summary>
        /// Gets or sets the base color texture.
        /// </summary>
        public Texture2D BaseColorTexture { get; set; }

        /// <summary>
        /// Gets or sets the sampler state for the base color texture.
        /// </summary>
        public SamplerState BaseColorSamplerState { get; set; }

        /// <summary>
        /// Gets or sets the metalness of the material. 
        /// A value of 1.0 means the material is metal. A value of 0.0 means the material is a dielectric.
        /// </summary>
        public Single MetallicFactor { get; set; }

        /// <summary>
        /// Gets or sets the roughness of the material.
        /// A value of 1.0 means the material is completely rough. A value of 0.0 means the material is completely smooth.
        /// </summary>
        public Single RoughnessFactor { get; set; }

        /// <summary>
        /// Gets or sets the metallic-roughness texture. 
        /// The metalness values are sampled from the B channel. The roughness values are sampled from the G channel.
        /// </summary>
        public Texture2D MetallicRoughnessTexture { get; set; }

        /// <summary>
        /// Gets or sets the sampler state for the metallic-roughness texture.
        /// </summary>
        public SamplerState MetallicRoughnessSamplerState { get; set; }

        /// <inheritdoc/>
        protected override void OnBegin(Camera camera, ref Matrix worldMatrix)
        {
            Effect.CurrentTechnique = Effect.Techniques[0];

            base.OnBegin(camera, ref worldMatrix);
        }

        /// <inheritdoc/>
        protected override void OnPassApplied(System.Int32 passIndex)
        {
            base.OnPassApplied(passIndex);
        }
    }
}
