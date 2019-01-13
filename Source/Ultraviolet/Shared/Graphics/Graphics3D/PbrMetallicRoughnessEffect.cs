namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="PbrMetallicRoughnessEffect"/> class.
    /// </summary>
    /// <param name="uv">The Ultraviolet context.</param>
    /// <returns>The instance of <see cref="PbrMetallicRoughnessEffect"/> that was created.</returns>
    public delegate PbrMetallicRoughnessEffect PbrMetallicRoughnessEffectFactory(UltravioletContext uv);

    /// <summary>
    /// Represents an effect that implements the metallic/roughness model for physically-based rendering.
    /// </summary>
    public abstract class PbrMetallicRoughnessEffect : Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PbrMetallicRoughnessEffect"/> class.
        /// </summary>
        /// <param name="impl">The <see cref="EffectImplementation"/> that implements this effect.</param>
        protected PbrMetallicRoughnessEffect(EffectImplementation impl)
            : base(impl)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="PbrMetallicRoughnessEffect"/> class.
        /// </summary>
        /// <returns>The instance of <see cref="PbrMetallicRoughnessEffect"/> that was created.</returns>
        public static PbrMetallicRoughnessEffect Create()
        {
            var uv = UltravioletContext.DemandCurrent();
            return uv.GetFactoryMethod<PbrMetallicRoughnessEffectFactory>()(uv);
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="PbrMetallicRoughnessEffect"/> class.
        /// </summary>
        public static PbrMetallicRoughnessEffect Instance => instance.Value;

        // A singleton instance of the effect.
        private static readonly UltravioletSingleton<PbrMetallicRoughnessEffect> instance = new UltravioletSingleton<PbrMetallicRoughnessEffect>(
            UltravioletSingletonFlags.DisabledInServiceMode, uv => Create());
    }
}
