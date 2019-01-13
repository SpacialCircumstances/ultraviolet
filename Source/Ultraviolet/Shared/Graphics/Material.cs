using System;
using Ultraviolet.Core;

namespace Ultraviolet.Graphics
{
    /// <summary>
    /// Represents a collection of <see cref="Effect"/> parameters associated with a particular object.
    /// </summary>
    public abstract class Material : UltravioletResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="effect">The effect which this material encapsulates.</param>
        public Material(UltravioletContext uv, Effect effect)
            : base(uv)
        {
            Contract.Require(effect, nameof(effect));

            this.Effect = effect;
        }

        /// <summary>
        /// Prepares to render an object with this material.
        /// </summary>
        /// <param name="camera">The current camera.</param>
        /// <param name="worldMatrix">The current world matrix for the object being rendered.</param>
        public void Begin(Camera camera, ref Matrix worldMatrix)
        {
            Contract.Require(camera, nameof(camera));
            Contract.Ensure(!begun, "TODO");

            pass = 0;
            OnBegin(camera, ref worldMatrix);

            begun = true;
        }

        /// <summary>
        /// Finishes rendering with the material.
        /// </summary>
        public void End()
        {
            Contract.Ensure(begun, "TODO");

            pass = 0;
            OnEnd();

            begun = false;
        }

        /// <summary>
        /// Prepares to render the next pass of an object with this material.
        /// </summary>
        /// <returns><see langword="true"/> if the next pass was applied; otherwise, <see langword="false"/>.</returns>
        public Boolean ApplyNextPass()
        {
            Contract.Ensure(begun, "TODO");

            var technique = Effect.CurrentTechnique;
            if (pass >= technique.Passes.Count)
                return false;

            technique.Passes[pass].Apply();
            OnPassApplied(pass);
            pass++;

            return true;
        }

        /// <summary>
        /// Gets the <see cref="Effect"/> which this material encapsulates.
        /// </summary>
        public Effect Effect { get; }

        /// <summary>
        /// Called when rendering with this material begins.
        /// </summary>
        /// <param name="camera">The current camera.</param>
        /// <param name="worldMatrix">The current world matrix for the object being rendered.</param>
        protected virtual void OnBegin(Camera camera, ref Matrix worldMatrix)
        {

        }

        /// <summary>
        /// Called when this material is reset.
        /// </summary>
        protected virtual void OnEnd()
        {

        }

        /// <summary>
        /// Called when a new pass is applied.
        /// </summary>
        /// <param name="passIndex">The index of the current pass.</param>
        protected virtual void OnPassApplied(Int32 passIndex)
        {

        }

        // State values.
        private Boolean begun;
        private Int32 pass;
    }
}
