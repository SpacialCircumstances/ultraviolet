using System;
using System.IO;
using glTFLoader;
using glTFLoader.Schema;
using Ultraviolet.Content;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents an importer for Khronos Group GL Transmission Files.
    /// </summary>
    [ContentImporter(".gltf")]
    [CLSCompliant(false)]
    public sealed class GltfImporter : ContentImporter<Gltf>
    {
        /// <inheritdoc/>
        public override Gltf Import(IContentImporterMetadata metadata, Stream stream)
        {
            var model = Interface.LoadModel(stream);

            if (!String.Equals("2.0", model.Asset.Version, StringComparison.Ordinal))
                throw new NotSupportedException("TODO");

            return model;
        }
    }
}
