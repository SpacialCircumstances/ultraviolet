using System;
using System.Linq;
using glTFLoader.Schema;
using Ultraviolet.Content;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents a content processor which converts instances of <see cref="Gltf"/> to Ultraviolet <see cref="Model"/> instances.
    /// </summary>
    [ContentProcessor]
    [CLSCompliant(false)]
    public sealed partial class GltfProcessor : ContentProcessor<Gltf, Model>
    {
        /// <inheritdoc/>
        public override Model Process(ContentManager manager, IContentProcessorMetadata metadata, Gltf input)
        {
            using (var cache = new ModelObjectCache(manager, metadata, input))
            {
                var scenes = input.Scenes.Select(x => new ModelScene(x.Name,
                    x.Nodes.Select(y => cache.GetModelNode(y)).ToList())).ToList();

                var model = new Model(manager.Ultraviolet, scenes);
                model.Scenes.ChangeDefaultScene(input.Scene);
                return model;
            }
        }        
    }
}
