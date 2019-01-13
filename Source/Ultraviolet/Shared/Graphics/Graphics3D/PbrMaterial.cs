using System;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents the base class for physically-based materials.
    /// </summary>
    public abstract class PbrMaterial : Material
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PbrMaterial"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="effect">The effect which this material encapsulates.</param>
        public PbrMaterial(UltravioletContext uv, Effect effect) 
            : base(uv, effect)
        {

        }

        /// <summary>
        /// Gets or sets the material's alpha rendering mode.
        /// </summary>
        public PbrAlphaMode AlphaMode { get; set; }

        /// <summary>
        /// Gets or sets the cutoff value when in <see cref="PbrAlphaMode.Mask"/> mode. 
        /// If the alpha value is greater than or equal to this value then it is rendered as fully opaque; otherwise,
        /// it is rendered as fully transparent.
        /// </summary>
        public Single AlphaCutoff { get; set; }

        /// <summary>
        /// Gets or sets the material's tangent space normal map texture.
        /// </summary>
        public Texture2D NormalTexture { get; set; }

        /// <summary>
        /// Gets or sets the sampler state for the tangent space normal map texture.
        /// </summary>
        public SamplerState NormalSamplerState { get; set; }

        /// <summary>
        /// Gets or sets the scalar multiplier which is applied to each normal vector of
        /// the tangent space normal map texture. This value is ignored if <see cref="NormalTexture"/> is not specified.
        /// </summary>
        public Single NormalTextureScale { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the occlusion map texture.
        /// </summary>
        public Texture2D OcclusionTexture { get; set; }
        
        /// <summary>
        /// Gets or sets the sampler state for the occlusion map texture.
        /// </summary>
        public SamplerState OcclusionSamplerState { get; set; }

        /// <summary>
        /// Gets or sets the scalar multiplier controlling the amount of occlusion applied.
        /// A value of 0.0 means no occlusion. A value of 1.0 means full occlusion. This value is ignored if <see cref="OcclusionTexture"/> is not specified.
        /// </summary>
        public Single OcclusionStrength { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the emissive map texture.
        /// </summary>
        public Texture2D EmissiveTexture { get; set; }
        
        /// <summary>
        /// Gets or sets the sampler state for the emissive map texture.
        /// </summary>
        public SamplerState EmissiveSamplerState { get; set; }

        /// <summary>
        /// Gets or sets the color and intensity of the light emitted by the material.
        /// If <see cref="EmissiveTexture"/> is specified, this value is multiplied with the texel values.
        /// </summary>
        public Color EmissiveFactor { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates whether the material is double sided.
        /// When this value is <see langword="false"/>, back-face culling is disabled.
        /// </summary>
        public Boolean DoubleSided { get; set; }
    }
}
