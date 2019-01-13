namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents the alpha rendering modes supported by physically-based materials.
    /// </summary>
    public enum PbrAlphaMode
    {
        /// <summary>
        /// Output is fully opaque; alpha values are ignored.
        /// </summary>
        Opaque,

        /// <summary>
        /// Output is either fully opaque or fully transparent depending on the alpha value and a specified cutoff value.
        /// </summary>
        Mask,

        /// <summary>
        /// Output is combined with the background using the normal painting operation.
        /// </summary>
        Blend,
    }
}
