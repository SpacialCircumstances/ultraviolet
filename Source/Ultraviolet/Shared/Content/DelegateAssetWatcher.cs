﻿using System;

namespace Ultraviolet.Content
{
    /// <summary>
    /// Represents the method that is invoked when a <see cref="DelegateAssetWatcher{T}"/> is validating an asset.
    /// </summary>
    /// <param name="path">The asset path of the asset which is being reloaded.</param>
    /// <param name="asset">The asset which is being reloaded.</param>
    /// <returns><see langword="true"/> if the specified asset is valid; otherwise, <see langword="false"/>.</returns>
    public delegate bool AssetWatcherValidatingHandler<T>(String path, T asset);

    /// <summary>
    /// Represents the method that is invoked when a <see cref="DelegateAssetWatcher{T}"/> is reloading an asset.
    /// </summary>
    /// <param name="path">The asset path of the asset which was reloaded.</param>
    /// <param name="asset">The asset which was reloaded.</param>
    /// <param name="validated">A value indicating whether the asset that was loading validated successfully.</param>
    public delegate void AssetWatcherReloadingHandler<T>(String path, T asset, Boolean validated);

    /// <summary>
    /// Represents an <see cref="AssetWatcher{T}"/> which implements its methods using delegates.
    /// </summary>
    public sealed class DelegateAssetWatcher<T> : AssetWatcher<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateAssetWatcher{T}"/> class.
        /// </summary>
        /// <param name="validating">A delegate which implements the <see cref="OnValidating(String, T)"/> method.</param>
        /// <param name="reloading">A delegate which implements the <see cref="OnReloaded(String, T, Boolean)"/> method.</param>
        public DelegateAssetWatcher(AssetWatcherValidatingHandler<T> validating, AssetWatcherReloadingHandler<T> reloading)
        {
            this.validating = validating;
            this.reloading = reloading;
        }

        /// <inheritdoc/>
        public override bool OnValidating(String path, T asset) => validating?.Invoke(path, asset) ?? true;

        /// <inheritdoc/>
        public override void OnReloaded(String path, T asset, Boolean validated) => reloading?.Invoke(path, asset, validated);

        // State values.
        private AssetWatcherValidatingHandler<T> validating;
        private AssetWatcherReloadingHandler<T> reloading;
    }
}
