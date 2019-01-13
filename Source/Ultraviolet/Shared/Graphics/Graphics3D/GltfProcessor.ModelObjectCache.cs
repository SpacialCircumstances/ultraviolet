using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using glTFLoader.Schema;
using Ultraviolet.Content;

namespace Ultraviolet.Graphics.Graphics3D
{
    public sealed partial class GltfProcessor
    {
        /// <summary>
        /// Represents a cache containing objects which have been created from the GLTF object hierarchy.
        /// </summary>
        private sealed class ModelObjectCache : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ModelObjectCache"/> class.
            /// </summary>
            /// <param name="manager">The content manager which is loading the asset.</param>
            /// <param name="metadata">The metadata for the asset which is being loaded.</param>
            /// <param name="input">The <see cref="Gltf"/> from which to create model objects.</param>
            public ModelObjectCache(ContentManager manager, IContentProcessorMetadata metadata, Gltf input)
            {
                this.manager = manager;
                this.metadata = metadata;
                this.gltf = input;
            }

            /// <summary>
            /// Releases resources associated with the cache.
            /// </summary>
            public void Dispose()
            {
                GC.SuppressFinalize(this);

                foreach (var kvp in bufferCache)
                    kvp.Value.Dispose();

                bufferCache.Clear();
                bufferViewCache.Clear();
                modelMeshCache.Clear();
                modelNodeCache.Clear();
                textureCache.Clear();
                samplerStateCache.Clear();
            }

            /// <summary>
            /// Gets the data at the specified URI.
            /// </summary>
            public Byte[] GetDataFromUri(String uri, out String mediatype, Boolean addAssetAsDependency = true)
            {
                if (uri.StartsWith("data:"))
                {
                    var ixColon = "data".Length;
                    var ixComma = uri.IndexOf(',', ixColon + 1);
                    if (ixComma < 0)
                        throw new ArgumentException(nameof(uri));

                    var mimetypeAndEncoding = uri.Substring(ixColon + 1, ixComma - (ixColon + 1));
                    var mimetypeAndEncodingDelimIx = String.IsNullOrEmpty(mimetypeAndEncoding) ? -1 : mimetypeAndEncoding.IndexOf(';', ixColon + 1);
                    var mimetype = mimetypeAndEncodingDelimIx >= 0 ? mimetypeAndEncoding.Substring(0, mimetypeAndEncodingDelimIx) : mimetypeAndEncoding;
                    var encoding = mimetypeAndEncodingDelimIx >= 0 ? mimetypeAndEncoding.Substring(mimetypeAndEncodingDelimIx + 1) : "plaintext";
                    var data = uri.Substring(ixComma + 1);

                    mediatype = String.IsNullOrEmpty(mimetype) ? "text/plain;charset=US-ASCII" : mimetype;

                    return String.Equals("base64", encoding, StringComparison.Ordinal) ?
                        Convert.FromBase64String(data) : Encoding.Unicode.GetBytes(data);
                }
                else
                {
                    var path = ResolveDependencyAssetPath(metadata, uri);

                    if (addAssetAsDependency)
                        metadata.AddAssetDependency(path);

                    mediatype = null;
                    return manager.Load<Byte[]>(path, cache: false);
                }
            }

            /// <summary>
            /// Gets the specified buffer.
            /// </summary>
            public MemoryStream GetBuffer(Int32? index)
            {
                if (index == null)
                    return null;

                var indexValue = index.Value;
                if (bufferCache.TryGetValue(indexValue, out var value))
                    return value;

                var gltfBuffer = gltf.Buffers[indexValue];

                var uri = gltfBuffer.Uri;
                var data = GetDataFromUri(uri, out var mediatype);
                var stream = new MemoryStream(data, 0, data.Length, false, true);

                bufferCache[indexValue] = stream;
                return stream;
            }

            /// <summary>
            /// Gets the specified buffer view.
            /// </summary>
            public MemoryStreamView GetBufferView(Int32? index)
            {
                if (index == null)
                    return null;

                var indexValue = index.Value;
                if (bufferViewCache.TryGetValue(indexValue, out var value))
                    return value;

                var gltfBufferView = gltf.BufferViews[indexValue];

                var buffer = GetBuffer(gltfBufferView.Buffer);
                var bufferView = new MemoryStreamView(buffer, gltfBufferView.ByteOffset, gltfBufferView.ByteLength);

                bufferViewCache[indexValue] = bufferView;
                return bufferView;
            }

            /// <summary>
            /// Gets the specified model mesh object.
            /// </summary>
            public ModelMesh GetModelMesh(Int32? index)
            {
                if (index == null)
                    return null;

                var indexValue = index.Value;
                if (modelMeshCache.TryGetValue(indexValue, out var value))
                    return value;

                var gltfMesh = gltf.Meshes[indexValue];

                var meshGeometries = gltfMesh.Primitives.Select(x => GetModelMeshGeometry(x)).ToList();
                value = new ModelMesh(gltfMesh.Name, meshGeometries);

                modelMeshCache[indexValue] = value;
                return value;
            }

            /// <summary>
            /// Gets the specified model mesh geometry object.
            /// </summary>
            public ModelMeshGeometry GetModelMeshGeometry(MeshPrimitive primitive)
            {
                var geometryStream = GeometryStream.Create();
                var vertexCount = 0;
                var indexCount = 0;

                // Create the vertex buffers which will contain our vertex attributes.
                var primitiveType = GltfModeToPrimitiveType(primitive.Mode);
                var attributeGroupsByBufferView = primitive.Attributes.GroupBy(x => x.Value);
                foreach (var attributeGroupByBufferView in attributeGroupsByBufferView)
                {
                    var bufferViewIndex = gltf.Accessors[attributeGroupByBufferView.Key].BufferView;
                    var bufferView = GetBufferView(bufferViewIndex);

                    var vertexPosition = 0;
                    var vertexElements = new List<VertexElement>();
                    foreach (var attribute in attributeGroupByBufferView)
                    {
                        var accessor = gltf.Accessors[attribute.Value];

                        if (vertexCount == 0)
                            vertexCount = accessor.Count;

                        if (vertexCount != 0 && vertexCount != accessor.Count)
                            throw new InvalidOperationException("TODO");

                        var vertexElementFormat = GltfAccessorToElementFormat(accessor);
                        var vertexElementUsage = GltfAttributeToElementUsage(attribute, out var vertexIndex);
                        var vertexElement = new VertexElement(vertexPosition, vertexElementFormat, vertexElementUsage, vertexIndex);
                        vertexElements.Add(vertexElement);
                        vertexPosition += 0; // TODO: Calculate stride
                    }

                    // Fill vertex buffer from buffer view
                    var vertexDeclaration = new VertexDeclaration(vertexElements);
                    var vertexBuffer = VertexBuffer.Create(vertexDeclaration, vertexCount);
                    var vertexData = bufferView.BaseStream.GetBuffer();
                    var vertexDataHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
                    try
                    {
                        vertexBuffer.SetRawData(vertexDataHandle.AddrOfPinnedObject(), (Int32)bufferView.Offset, 0, 
                            (Int32)bufferView.Length, SetDataOptions.None);
                    }
                    finally
                    {
                        if (vertexDataHandle.IsAllocated)
                            vertexDataHandle.Free();
                    }

                    geometryStream.Attach(vertexBuffer);
                }

                // Create the index buffer, if there is one.
                if (primitive.ShouldSerializeIndices())
                {
                    var accessor = gltf.Accessors[primitive.Indices.Value];
                    indexCount = accessor.Count;

                    var bufferViewIndex = accessor.BufferView;
                    var bufferView = GetBufferView(bufferViewIndex);

                    var foo = new Byte[bufferView.Length];
                    bufferView.Seek(0, SeekOrigin.Begin);
                    bufferView.Read(foo, 0, foo.Length);
                    bufferView.Seek(0, SeekOrigin.Begin);

                    var indexDataSrc = bufferView.BaseStream.GetBuffer();
                    var indexDataDst = indexDataSrc;
                    var indexDataOffset = bufferView.Offset;
                    var indexDataLength = bufferView.Length;
                    var indexBufferType = IndexBufferElementType.Int32;
                    switch (accessor.ComponentType)
                    {
                        case Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                            indexBufferType = IndexBufferElementType.Int16;
                            indexDataOffset = 0;
                            indexDataLength = bufferView.Length * 2;
                            indexDataDst = new Byte[indexDataLength];
                            for (int i = BitConverter.IsLittleEndian ? 0 : 1, j = 0; i < indexDataDst.Length; i += sizeof(UInt16), j++)
                            {
                                indexDataDst[i] = indexDataSrc[bufferView.Offset + j];
                            }
                            break;

                        case Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                            indexBufferType = IndexBufferElementType.Int16;
                            break;

                        case Accessor.ComponentTypeEnum.UNSIGNED_INT:
                            indexBufferType = IndexBufferElementType.Int32;
                            break;

                        default:
                            throw new NotSupportedException("TODO");
                    }

                    var indexBuffer = IndexBuffer.Create(indexBufferType, indexCount);
                    var indexDataHandle = GCHandle.Alloc(indexDataDst, GCHandleType.Pinned);
                    try
                    {
                        indexBuffer.SetRawData(indexDataHandle.AddrOfPinnedObject(), (Int32)indexDataOffset, 0,
                            (Int32)indexDataLength, SetDataOptions.None);
                    }
                    finally
                    {
                        if (indexDataHandle.IsAllocated)
                            indexDataHandle.Free();
                    }

                    geometryStream.Attach(indexBuffer);
                }

                // TODO: Handle morph targets?

                // Create the geometry object.
                var material = GetMaterial(primitive.Material);
                return new ModelMeshGeometry(primitiveType, geometryStream, vertexCount, indexCount, material);
            }

            /// <summary>
            /// Gets the specified model node object.
            /// </summary>
            public ModelNode GetModelNode(Int32? index)
            {
                Matrix CalculateTransform(Node node)
                {
                    if (node.ShouldSerializeMatrix())
                    {
                        var m = node.Matrix;
                        return new Matrix(m[0], m[1], m[2], m[3], m[4], m[5], m[6], m[7], m[8], m[9], m[10], m[11], m[12], m[13], m[14], m[15]);
                    }
                    if (node.ShouldSerializeRotation() || node.ShouldSerializeScale() || node.ShouldSerializeTranslation())
                    {
                        var translation = new Vector3(node.Translation[0], node.Translation[1], node.Translation[2]);
                        var rotation = new Quaternion(node.Rotation[0], node.Rotation[1], node.Rotation[2], node.Rotation[3]);
                        var scale = new Vector3(node.Scale[0], node.Scale[1], node.Scale[2]);
                        return Matrix.CreateFromTranslationRotationScale(translation, rotation, scale);
                    }
                    return Matrix.Identity;
                };

                if (index == null)
                    return null;

                var indexValue = index.Value;
                if (modelNodeCache.TryGetValue(indexValue, out var value))
                    return value;

                var gltfNode = gltf.Nodes[indexValue];

                var nodeMesh = GetModelMesh(gltfNode.Mesh);
                var nodeTransform = CalculateTransform(gltfNode);
                var nodeChildren = gltfNode.Children?.Select(x => GetModelNode(x)).ToList();
                value = new ModelNode(gltfNode.Name, nodeMesh, nodeChildren, nodeTransform);

                modelNodeCache[indexValue] = value;
                return value;
            }

            /// <summary>
            /// Gets the specified texture.
            /// </summary>
            public Texture2D GetTexture(Int32? index)
            {
                if (index == null)
                    return null;

                var indexValue = index.Value;
                if (textureCache.TryGetValue(indexValue, out var value))
                    return value;

                var texture = default(Texture2D);

                var gltfImage = gltf.Images[indexValue];
                if (gltfImage.BufferView.HasValue)
                {
                    var imgMimeType = gltfImage.MimeType;
                    var imgData = GetBuffer(gltfImage.BufferView);
                    if (imgData == null)
                        throw new InvalidOperationException("TODO");

                    texture = manager.LoadFromStream<Texture2D>(imgData, imgMimeType.Value == Image.MimeTypeEnum.image_jpeg ? "jpg" : "png");
                }
                else
                {
                    var imgUri = gltfImage.Uri;
                    var imgData = GetDataFromUri(imgUri, out var imgMimeType);
                    using (var imgDataStream = new MemoryStream(imgData))
                    {
                        texture = manager.LoadFromStream<Texture2D>(imgDataStream, String.Equals(imgMimeType, "image/jpeg", StringComparison.Ordinal) ? "jpg" : "png");
                    }
                }

                textureCache[indexValue] = texture;
                return texture;
            }

            /// <summary>
            /// Gets the specified sampler.
            /// </summary>
            public SamplerState GetSampler(Int32? index)
            {
                if (index == null)
                    return SamplerState.LinearWrap;

                var indexValue = index.Value;
                if (samplerStateCache.TryGetValue(indexValue, out var value))
                    return value;

                var gltfSampler = gltf.Samplers[indexValue];

                var samplerState = SamplerState.Create();

                var minFilter = TextureFilter.Linear;
                if (gltfSampler.MinFilter.HasValue)
                {
                    switch (gltfSampler.MinFilter.Value)
                    {
                        case Sampler.MinFilterEnum.NEAREST:
                            minFilter = TextureFilter.Point;
                            break;

                        case Sampler.MinFilterEnum.LINEAR:
                            minFilter = TextureFilter.Linear;
                            break;

                        default:
                            throw new NotSupportedException("TODO");
                    }
                }

                var magFilter = TextureFilter.Linear;
                if (gltfSampler.MagFilter.HasValue)
                {
                    switch (gltfSampler.MagFilter.Value)
                    {
                        case Sampler.MagFilterEnum.NEAREST:
                            magFilter = TextureFilter.Point;
                            break;

                        case Sampler.MagFilterEnum.LINEAR:
                            magFilter = TextureFilter.Linear;
                            break;

                        default:
                            throw new NotSupportedException("TODO");
                    }
                }

                if (minFilter != magFilter)
                    throw new NotSupportedException("TODO");

                var addressU = TextureAddressMode.Wrap;
                switch (gltfSampler.WrapS)
                {
                    case Sampler.WrapSEnum.CLAMP_TO_EDGE:
                        addressU = TextureAddressMode.Clamp;
                        break;

                    case Sampler.WrapSEnum.MIRRORED_REPEAT:
                        addressU = TextureAddressMode.Mirror;
                        break;

                    case Sampler.WrapSEnum.REPEAT:
                        addressU = TextureAddressMode.Wrap;
                        break;

                    default:
                        throw new NotSupportedException("TODO");
                }

                var addressV = TextureAddressMode.Wrap;
                switch (gltfSampler.WrapT)
                {
                    case Sampler.WrapTEnum.CLAMP_TO_EDGE:
                        addressV = TextureAddressMode.Clamp;
                        break;

                    case Sampler.WrapTEnum.MIRRORED_REPEAT:
                        addressV = TextureAddressMode.Mirror;
                        break;

                    case Sampler.WrapTEnum.REPEAT:
                        addressV = TextureAddressMode.Wrap;
                        break;

                    default:
                        throw new NotSupportedException("TODO");
                }

                samplerState.Filter = magFilter;
                samplerState.AddressU = addressU;
                samplerState.AddressV = addressV;

                samplerStateCache[indexValue] = samplerState;
                return samplerState;
            }

            /// <summary>
            /// Gets the specified material object.
            /// </summary>
            public Material GetMaterial(Int32? index)
            {
                var effect = BasicEffect.Create();
                var material = BasicMaterial.Create(effect);

                if (index.HasValue)
                {
                    var gltfMaterial = gltf.Materials[index.Value];
                    if (gltfMaterial.ShouldSerializePbrMetallicRoughness())
                    {
                        var pbr = gltfMaterial.PbrMetallicRoughness;

                        material.DiffuseColor = new Color(
                            pbr.BaseColorFactor[0],
                            pbr.BaseColorFactor[1],
                            pbr.BaseColorFactor[2],
                            pbr.BaseColorFactor[3]);

                        if (pbr.BaseColorTexture != null)
                        {
                            var gltfTexture = gltf.Textures[pbr.BaseColorTexture.Index];
                            material.Texture = GetTexture(gltfTexture.Source);
                        }
                    }
                }

                return material;

                /*
                var value = PbrMetallicRoughnessMaterial.Create();
                value.AlphaMode = PbrAlphaMode.Opaque;
                value.AlphaCutoff = 0.5f;
                value.DoubleSided = false;

                if (index == null)
                    return value;
     
                var gltfMaterial = gltf.Materials[index.Value];
                if (gltfMaterial.ShouldSerializePbrMetallicRoughness())
                {
                    var pbr = gltfMaterial.PbrMetallicRoughness;
                    value.BaseColorFactor = new Color(
                        pbr.BaseColorFactor[0], 
                        pbr.BaseColorFactor[1], 
                        pbr.BaseColorFactor[2], 
                        pbr.BaseColorFactor[3]);

                    if (pbr.BaseColorTexture != null)
                    {
                        var gltfTexture = gltf.Textures[pbr.BaseColorTexture.Index];
                        value.BaseColorTexture = GetTexture(gltfTexture.Source);
                        value.BaseColorSamplerState = GetSampler(gltfTexture.Sampler);
                    }

                    value.MetallicFactor = pbr.MetallicFactor;
                    value.RoughnessFactor = pbr.RoughnessFactor;

                    if (pbr.MetallicRoughnessTexture != null)
                    {
                        var gltfTexture = gltf.Textures[pbr.MetallicRoughnessTexture.Index];
                        value.MetallicRoughnessTexture = GetTexture(gltfTexture.Source);
                        value.MetallicRoughnessSamplerState = GetSampler(gltfTexture.Sampler);
                    }
                }

                if (gltfMaterial.NormalTexture != null)
                {
                    var gltfTexture = gltf.Textures[gltfMaterial.NormalTexture.Index];
                    value.NormalTexture = GetTexture(gltfTexture.Source);
                    value.NormalSamplerState = GetSampler(gltfTexture.Sampler);
                    value.NormalTextureScale = gltfMaterial.NormalTexture.Scale;
                }

                if (gltfMaterial.OcclusionTexture != null)
                {
                    var gltfTexture = gltf.Textures[gltfMaterial.OcclusionTexture.Index];
                    value.OcclusionTexture = GetTexture(gltfTexture.Source);
                    value.OcclusionSamplerState = GetSampler(gltfTexture.Sampler);
                    value.OcclusionStrength = gltfMaterial.OcclusionTexture.Strength;
                }

                value.EmissiveFactor = new Color(
                    gltfMaterial.EmissiveFactor[0],
                    gltfMaterial.EmissiveFactor[1],
                    gltfMaterial.EmissiveFactor[2], 1f);

                if (gltfMaterial.EmissiveTexture != null)
                {
                    value.EmissiveTexture = null;
                }

                value.AlphaMode =
                    (gltfMaterial.AlphaMode == glTFLoader.Schema.Material.AlphaModeEnum.OPAQUE) ? PbrAlphaMode.Opaque :
                    (gltfMaterial.AlphaMode == glTFLoader.Schema.Material.AlphaModeEnum.MASK) ? PbrAlphaMode.Mask : PbrAlphaMode.Blend;
                value.AlphaCutoff = gltfMaterial.AlphaCutoff;
                value.DoubleSided = gltfMaterial.DoubleSided;

                return value;
                */
            }

            /// <summary>
            /// Converts a GLTF mode to a <see cref="PrimitiveType"/> value.
            /// </summary>
            private static PrimitiveType GltfModeToPrimitiveType(MeshPrimitive.ModeEnum mode)
            {
                switch (mode)
                {
                    case MeshPrimitive.ModeEnum.LINES:
                        return PrimitiveType.LineList;

                    case MeshPrimitive.ModeEnum.LINE_STRIP:
                        return PrimitiveType.LineStrip;

                    case MeshPrimitive.ModeEnum.TRIANGLES:
                        return PrimitiveType.TriangleList;

                    case MeshPrimitive.ModeEnum.TRIANGLE_STRIP:
                        return PrimitiveType.TriangleStrip;

                    default:
                        throw new NotSupportedException("TODO: Primitive type not supported.");
                }
            }

            /// <summary>
            /// Converts a GLTF attribute to a <see cref="VertexElementUsage"/> value.
            /// </summary>
            private static VertexElementUsage GltfAttributeToElementUsage(KeyValuePair<String, Int32> attribute, out Int32 index)
            {
                var semanticName = attribute.Key;
                var semanticIndex = 0;

                var isIndexedSemantic =
                    attribute.Key.StartsWith("TEXCOORD_") ||
                    attribute.Key.StartsWith("COLOR_") ||
                    attribute.Key.StartsWith("JOINTS_") ||
                    attribute.Key.StartsWith("WEIGHTS_");

                if (isIndexedSemantic)
                {
                    var semanticDelimIx = semanticName.LastIndexOf('_');
                    semanticName = semanticName.Substring(0, semanticDelimIx);
                    semanticIndex = Int32.Parse(semanticName.Substring(semanticDelimIx + 1));
                }

                index = semanticIndex;

                switch (semanticName)
                {
                    case "POSITION":
                        return VertexElementUsage.Position;

                    case "NORMAL":
                        return VertexElementUsage.Normal;

                    case "TANGENT":
                        throw new NotSupportedException();

                    case "TEXCOORD":
                        return VertexElementUsage.TextureCoordinate;

                    case "COLOR":
                        return VertexElementUsage.Color;

                    case "JOINTS":
                        throw new NotSupportedException();

                    case "WEIGHTS":
                        throw new NotSupportedException();
                }

                throw new NotSupportedException();
            }

            /// <summary>
            /// Converts a GLTF accessor to a <see cref="VertexElementFormat"/> value.
            /// </summary>
            private static VertexElementFormat GltfAccessorToElementFormat(Accessor accessor)
            {
                switch (accessor.Type)
                {
                    case Accessor.TypeEnum.MAT2:
                    case Accessor.TypeEnum.MAT3:
                    case Accessor.TypeEnum.MAT4:
                        throw new NotSupportedException();

                    case Accessor.TypeEnum.VEC2:
                        switch (accessor.ComponentType)
                        {
                            case Accessor.ComponentTypeEnum.FLOAT:
                                return VertexElementFormat.Vector2;

                            case Accessor.ComponentTypeEnum.SHORT:
                                return accessor.Normalized ? VertexElementFormat.NormalizedShort2 : VertexElementFormat.Short2;

                            case Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                                return accessor.Normalized ? VertexElementFormat.NormalizedUnsignedShort2 : VertexElementFormat.UnsignedShort2;
                        }
                        break;

                    case Accessor.TypeEnum.VEC3:
                        switch (accessor.ComponentType)
                        {
                            case Accessor.ComponentTypeEnum.FLOAT:
                                return VertexElementFormat.Vector3;
                        }
                        break;

                    case Accessor.TypeEnum.VEC4:
                        switch (accessor.ComponentType)
                        {
                            case Accessor.ComponentTypeEnum.FLOAT:
                                return VertexElementFormat.Vector4;

                            case Accessor.ComponentTypeEnum.SHORT:
                                return accessor.Normalized ? VertexElementFormat.NormalizedShort4 : VertexElementFormat.Short4;

                            case Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                                return accessor.Normalized ? VertexElementFormat.NormalizedUnsignedShort4 : VertexElementFormat.UnsignedShort4;
                        }
                        break;

                    case Accessor.TypeEnum.SCALAR:
                        switch (accessor.ComponentType)
                        {
                            case Accessor.ComponentTypeEnum.FLOAT:
                                return VertexElementFormat.Single;
                        }
                        break;
                }

                throw new NotSupportedException();
            }
            
            // Content metadata.
            private readonly ContentManager manager;
            private readonly IContentProcessorMetadata metadata;

            // Object caches.
            private readonly Gltf gltf;
            private readonly Dictionary<Int32, MemoryStream> bufferCache = new Dictionary<Int32, MemoryStream>();
            private readonly Dictionary<Int32, MemoryStreamView> bufferViewCache = new Dictionary<Int32, MemoryStreamView>();
            private readonly Dictionary<Int32, ModelMesh> modelMeshCache = new Dictionary<Int32, ModelMesh>();
            private readonly Dictionary<Int32, ModelNode> modelNodeCache = new Dictionary<Int32, ModelNode>();
            private readonly Dictionary<Int32, Texture2D> textureCache = new Dictionary<Int32, Texture2D>();
            private readonly Dictionary<Int32, SamplerState> samplerStateCache = new Dictionary<Int32, SamplerState>();
        }
    }
}
