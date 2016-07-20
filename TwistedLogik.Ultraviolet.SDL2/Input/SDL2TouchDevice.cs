﻿using System;
using System.Collections.Generic;
using TwistedLogik.Nucleus;
using TwistedLogik.Nucleus.Messages;
using TwistedLogik.Ultraviolet.Input;
using TwistedLogik.Ultraviolet.Platform;
using TwistedLogik.Ultraviolet.SDL2.Messages;
using TwistedLogik.Ultraviolet.SDL2.Native;

namespace TwistedLogik.Ultraviolet.SDL2.Input
{
    /// <summary>
    /// Represents the SDL2 implementation of the <see cref="TouchDevice"/> class.
    /// </summary>
    public sealed class SDL2TouchDevice : TouchDevice,
        IMessageSubscriber<UltravioletMessageID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SDL2TouchDevice"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="index">The index of the SDL2 touch device represented by this object.</param>
        public SDL2TouchDevice(UltravioletContext uv, Int32 index)
            : base(uv)
        {
            // HACK: Working around an Android emulator glitch here -- it will
            // return a touch ID of 0 even after saying that the device exists,
            // and yet not produce any kind of SDL error... I hate emulators.            
            var id = SDL.GetTouchDevice(index);
            if (id == 0 && !String.IsNullOrEmpty(SDL.GetError()))
                throw new SDL2Exception();

            this.sdlTouchID = id;

            uv.Messages.Subscribe(this,
                SDL2UltravioletMessages.SDLEvent);
        }

        /// <inheritdoc/>
        void IMessageSubscriber<UltravioletMessageID>.ReceiveMessage(UltravioletMessageID type, MessageData data)
        {
            var evt = ((SDL2EventMessageData)data).Event;
            switch (evt.type)
            {
                case SDL_EventType.FINGERDOWN:
                    if (evt.tfinger.touchId == sdlTouchID)
                    {
                        if (!isRegistered)
                            Register();

                        BeginTouch(ref evt);
                    }
                    break;

                case SDL_EventType.FINGERUP:
                    if (evt.tfinger.touchId == sdlTouchID)
                    {
                        if (!isRegistered)
                            Register();

                        EndTouch(ref evt);
                    }
                    break;

                case SDL_EventType.FINGERMOTION:
                    if (evt.tfinger.touchId == sdlTouchID)
                    {
                        if (!isRegistered)
                            Register();

                        UpdateTouch(ref evt);
                    }
                    break;

                case SDL_EventType.MULTIGESTURE:
                    if (evt.mgesture.touchId == sdlTouchID)
                    {
                        if (!isRegistered)
                            Register();

                        OnMultiGesture(evt.mgesture.x, evt.mgesture.y, 
                            evt.mgesture.dTheta, evt.mgesture.dDist, evt.mgesture.numFingers);
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Resets the device's state in preparation for the next frame.
        /// </summary>
        public void ResetDeviceState()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            taps.Clear();
        }

        /// <inheritdoc/>
        public override void Update(UltravioletTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            timestamp = time.TotalTime.Ticks;
        }

        /// <inheritdoc/>
        public override Point2F NormalizeCoordinates(IUltravioletWindow window, Point2 coordinates)
        {
            Contract.Require(window, nameof(window));
            Contract.EnsureNotDisposed(this, Disposed);

            return new Point2F(
                coordinates.X / (Single)window.ClientSize.Width,
                coordinates.Y / (Single)window.ClientSize.Height);
        }

        /// <inheritdoc/>
        public override Point2F NormalizeCoordinates(IUltravioletWindow window, Int32 x, Int32 y)
        {
            Contract.Require(window, nameof(window));
            Contract.EnsureNotDisposed(this, Disposed);

            return new Point2F(
                x / (Single)window.ClientSize.Width,
                y / (Single)window.ClientSize.Height);
        }

        /// <inheritdoc/>
        public override Point2 DenormalizeCoordinates(IUltravioletWindow window, Point2F coordinates)
        {
            Contract.Require(window, nameof(window));
            Contract.EnsureNotDisposed(this, Disposed);

            return new Point2(
                (Int32)(coordinates.X * window.ClientSize.Width),
                (Int32)(coordinates.Y * window.ClientSize.Height));
        }

        /// <inheritdoc/>
        public override Point2 DenormalizeCoordinates(IUltravioletWindow window, Single x, Single y)
        {
            Contract.Require(window, nameof(window));
            Contract.EnsureNotDisposed(this, Disposed);
            
            return new Point2(
                (Int32)(x * window.ClientSize.Width),
                (Int32)(y * window.ClientSize.Height));
        }

        /// <inheritdoc/>
        public override Boolean IsActive(Int64 touchID)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].TouchID == touchID)
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override Boolean TryGetTouch(Int64 touchID, out TouchInfo touchInfo)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var touch in touches)
            {
                if (touch.TouchID == touchID)
                {
                    touchInfo = touch;
                    return true;
                }
            }

            touchInfo = default(TouchInfo);
            return false;
        }

        /// <inheritdoc/>
        public override TouchInfo GetTouch(Int64 touchID)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var touch in touches)
            {
                if (touch.TouchID == touchID)
                    return touch;
            }

            throw new ArgumentException(nameof(touchID));
        }

        /// <inheritdoc/>
        public override TouchInfo GetTouchByIndex(Int32 index)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return touches[index];
        }

        /// <inheritdoc/>
        public override Boolean WasTapped()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return taps.Count > 0;
        }

        /// <inheritdoc/>
        public override Boolean WasTappedWithin(RectangleF area)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var tap in taps)
            {
                if (area.Contains(tap.X, tap.Y))
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override Boolean IsTouchWithin(RectangleF area)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var touch in touches)
            {
                if (area.Contains(touch.CurrentX, touch.CurrentY))
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override Boolean IsTouchWithin(Int64 touchID, RectangleF area)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var touch in touches)
            {
                if (touch.TouchID == touchID)
                    return area.Contains(touch.CurrentX, touch.CurrentY);
            }

            return false;
        }

        /// <inheritdoc/>
        public override Int32 TouchCount
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return touches.Count;
            }
        }

        /// <inheritdoc/>
        public override Int32 TapCount
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return taps.Count;
            }
        }

        /// <inheritdoc/>
        public override Boolean IsRegistered => isRegistered;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (!Ultraviolet.Disposed)
                {
                    Ultraviolet.Messages.Unsubscribe(this);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Begins a new touch input.
        /// </summary>
        private void BeginTouch(ref SDL_Event evt)
        {
            var touchID = nextTouchID++;
            var touchInfo = new TouchInfo(timestamp, touchID, evt.tfinger.fingerId, 
                evt.tfinger.x, evt.tfinger.y, evt.tfinger.x, evt.tfinger.y, evt.tfinger.pressure);

            touches.Add(touchInfo);

            OnTouchDown(touchID, touchInfo.FingerID, touchInfo.CurrentX, touchInfo.CurrentY, touchInfo.Pressure);        
        }

        /// <summary>
        /// Ends an active touch input.
        /// </summary>
        private void EndTouch(ref SDL_Event evt)
        {
            for (int i = 0; i < touches.Count; i++)
            {
                var touch = touches[i];
                if (touch.FingerID == evt.tfinger.fingerId)
                {
                    touches.RemoveAt(i);
                    OnTouchUp(touch.TouchID, touch.FingerID);

                    if (timestamp - touch.Timestamp <= TimeSpan.FromMilliseconds(MaximumTapDelay).Ticks)
                    {
                        var vOrigin = new Vector2(touch.OriginX, touch.OriginY);
                        var vCurrent = new Vector2(touch.CurrentX, touch.CurrentY);
                        var vDelta = vCurrent - vOrigin;
                        if (vDelta.Length() <= MaximumTapDistance)
                        {
                            EndTap(touch.TouchID, touch.FingerID, touch.OriginX, touch.OriginY);
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Ends a tap.
        /// </summary>
        private void EndTap(Int64 touchID, Int64 fingerID, Single x, Single y)
        {
            var tapInfo = new TouchTapInfo(touchID, fingerID, x, y);
            taps.Add(tapInfo);

            OnTap(touchID, fingerID, x, y);
        }

        /// <summary>
        /// Updates an active touch input.
        /// </summary>
        private void UpdateTouch(ref SDL_Event evt)
        {
            for (int i = 0; i < touches.Count; i++)
            {
                var touch = touches[i];
                if (touch.FingerID == evt.tfinger.fingerId)
                {
                    var dx = evt.tfinger.x - touch.CurrentX;
                    var dy = evt.tfinger.y - touch.CurrentY;

                    touches[i] = new TouchInfo(touch.Timestamp, touch.TouchID, touch.FingerID,
                        touch.OriginX, touch.OriginY, evt.tfinger.x, evt.tfinger.y, evt.tfinger.pressure);

                    OnTouchMotion(touch.TouchID, touch.FingerID, 
                        evt.tfinger.x, evt.tfinger.y, dx, dy, evt.tfinger.pressure);

                    break;
                }
            }
        }

        /// <summary>
        /// Flags the device as registered.
        /// </summary>
        private void Register()
        {
            var input = (SDL2UltravioletInput)Ultraviolet.GetInput();
            if (input.RegisterTouchDevice(this))
                isRegistered = true;
        }

        // State values.
        private readonly List<TouchInfo> touches = new List<TouchInfo>(5);
        private readonly List<TouchTapInfo> taps = new List<TouchTapInfo>(5);
        private readonly Int64 sdlTouchID;
        private Int64 nextTouchID = 1;
        private Int64 timestamp;
        private Boolean isRegistered;
    }
}
