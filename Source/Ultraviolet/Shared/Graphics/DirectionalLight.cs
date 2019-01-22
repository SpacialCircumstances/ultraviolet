using System;

namespace Ultraviolet.Graphics
{
    /// <summary>
    /// Represents a directional light.
    /// </summary>
    public class DirectionalLight
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionalLight"/> class.
        /// </summary>
        /// <param name="directionParameter">The effect parameter which represents the light's direction.</param>
        /// <param name="diffuseColorParameter">The effect parameter which represents the light's diffuse color.</param>
        /// <param name="specularColorParameter">The effect parameter which represents the light's specular color.</param>
        /// <param name="cloneSource">An existing instance from which to copy parameters.</param>
        /// <remarks>The initial parameter values are either copied from the cloned object or set to default values if no cloned object is specified.
        /// The three <see cref="EffectParameter"/> instances are updated whenever the direction, diffuse color, or specular color properties are
        /// changed, or you can set these to <see langword="null"/> if you are cloning an existing instance.</remarks>
        public DirectionalLight(
            EffectParameter directionParameter,
            EffectParameter diffuseColorParameter,
            EffectParameter specularColorParameter,
            DirectionalLight cloneSource = null)
        {
            if (cloneSource == null)
            {
                this.directionParameter = directionParameter;
                this.diffuseColorParameter = diffuseColorParameter;
                this.specularColorParameter = specularColorParameter;
            }
            else
            {
                this.Enabled = cloneSource.Enabled;
                this.directionParameter = cloneSource.directionParameter;
                this.direction = cloneSource.direction;
                this.diffuseColorParameter = cloneSource.diffuseColorParameter;
                this.diffuseColor = cloneSource.diffuseColor;
                this.specularColorParameter = cloneSource.specularColorParameter;
                this.specularColor = cloneSource.specularColor;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the light is enabled.
        /// </summary>
        public Boolean Enabled { get; set; }

        /// <summary>
        /// Gets or sets the light's direction. This value must be a unit vector.
        /// </summary>
        public Vector3 Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = value;
                this.directionParameter?.SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the diffuse color of the light.
        /// </summary>
        public Color DiffuseColor
        {
            get { return this.diffuseColor; }
            set
            {
                this.diffuseColor = value;
                this.diffuseColorParameter?.SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the specular color of the light.
        /// </summary>
        public Color SpecularColor
        {
            get { return this.specularColor; }
            set
            {
                this.specularColor = value;
                this.specularColorParameter?.SetValue(value);
            }
        }

        // Effect parameters.
        private readonly EffectParameter directionParameter;
        private readonly EffectParameter diffuseColorParameter;
        private readonly EffectParameter specularColorParameter;

        // Parameter values.
        private Vector3 direction;
        private Color diffuseColor;
        private Color specularColor;
    }
}
