﻿using System;
using TwistedLogik.Nucleus;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D;
using TwistedLogik.Ultraviolet.UI.Presentation.Styles;

namespace TwistedLogik.Ultraviolet.UI.Presentation
{
    /// <summary>
    /// Represents the base class for standard Ultraviolet Presentation Framework elements.
    /// </summary>
    public abstract class FrameworkElement : UIElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElement"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="id">The unique identifier of this element within its layout.</param>
        public FrameworkElement(UltravioletContext uv, String id)
            : base(uv, id)
        {
            this.visualStateGroups = new VisualStateGroupCollection(this);
            this.visualStateGroups.Create("focus", VSGFocus);
        }

        /// <summary>
        /// Gets or sets the element's width in device-independent pixels.
        /// </summary>
        public Double Width
        {
            get { return GetValue<Double>(WidthProperty); }
            set { SetValue<Double>(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the element's minimum width in device independent pixels.
        /// </summary>
        public Double MinWidth
        {
            get { return GetValue<Double>(MinWidthProperty); }
            set { SetValue<Double>(MinWidthProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the element's maximum width in device independent pixels.
        /// </summary>
        public Double MaxWidth
        {
            get { return GetValue<Double>(MaxWidthProperty); }
            set { SetValue<Double>(MaxWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the element's height in device-independent pixels.
        /// </summary>
        public Double Height
        {
            get { return GetValue<Double>(HeightProperty); }
            set { SetValue<Double>(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the element's minimum height in device independent pixels.
        /// </summary>
        public Double MinHeight
        {
            get { return GetValue<Double>(MinHeightProperty); }
            set { SetValue<Double>(MinHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the element's maximum height in device independent pixels.
        /// </summary>
        public Double MaxHeight
        {
            get { return GetValue<Double>(MaxHeightProperty); }
            set { SetValue<Double>(MaxHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the element's outer margin.
        /// </summary>
        public Thickness Margin
        {
            get { return GetValue<Thickness>(MarginProperty); }
            set { SetValue<Thickness>(MarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the element's horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return GetValue<HorizontalAlignment>(HorizontalAlignmentProperty); }
            set { SetValue<HorizontalAlignment>(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the element's vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return GetValue<VerticalAlignment>(VerticalAlignmentProperty); }
            set { SetValue<VerticalAlignment>(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Width"/> property changes.
        /// </summary>
        public event UIElementEventHandler WidthChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="MinWidth"/> property changes.
        /// </summary>
        public event UIElementEventHandler MinWidthChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="MaxWidth"/> property changes.
        /// </summary>
        public event UIElementEventHandler MaxWidthChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="Height"/> property changes.
        /// </summary>
        public event UIElementEventHandler HeightChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="MinHeight"/> property changes.
        /// </summary>
        public event UIElementEventHandler MinHeightChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="MaxHeight"/> property changes.
        /// </summary>
        public event UIElementEventHandler MaxHeightChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="Margin"/> property changes.
        /// </summary>
        public event UIElementEventHandler MarginChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="HorizontalAlignment"/> property changes.
        /// </summary>
        public event UIElementEventHandler HorizontalAlignmentChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="VerticalAlignment"/> property changes.
        /// </summary>
        public event UIElementEventHandler VerticalAlignmentChanged;

        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        [Styled("width")]
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(Double), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleWidthChanged, () => Double.NaN, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="MinWidth"/> dependency property.
        /// </summary>
        [Styled("minwidth")]
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register("MinWidth", typeof(Double), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleMinWidthChanged, () => 0.0, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="MaxWidth"/> dependency property.
        /// </summary>
        [Styled("maxwidth")]
        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register("MaxWidth", typeof(Double), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleMaxWidthChanged, () => Double.PositiveInfinity, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        [Styled("height")]
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(Double), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleHeightChanged, () => Double.NaN, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="MinHeight"/> dependency property.
        /// </summary>
        [Styled("minheight")]
        public static readonly DependencyProperty MinHeightProperty = DependencyProperty.Register("MinHeight", typeof(Double), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleMinHeightChanged, () => 0.0, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="MaxHeight"/> dependency property.
        /// </summary>
        [Styled("maxheight")]
        public static readonly DependencyProperty MaxHeightProperty = DependencyProperty.Register("MaxHeight", typeof(Double), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleMaxHeightChanged, () => Double.PositiveInfinity, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="Margin"/> dependency property.
        /// </summary>
        [Styled("margin")]
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register("Margin", typeof(Thickness), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleMarginChanged, () => Thickness.Zero, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="HorizontalAlignment"/> dependency property.
        /// </summary>
        [Styled("halign")]
        public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleHorizontalAlignmentChanged, () => HorizontalAlignment.Left, DependencyPropertyOptions.AffectsArrange));

        /// <summary>
        /// Identifies the <see cref="VerticalAlignment"/> dependency property.
        /// </summary>
        [Styled("valign")]
        public static readonly DependencyProperty VerticalAlignmentProperty = DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(FrameworkElement),
            new DependencyPropertyMetadata(HandleVerticalAlignmentChanged, () => VerticalAlignment.Top, DependencyPropertyOptions.AffectsArrange));

        /// <summary>
        /// Called immediately prior to <see cref="MeasureOverride(Size2D)"/>.
        /// </summary>
        /// <param name="availableSize">The size of the area which the element's parent has 
        /// specified is available for the element's layout.</param>
        internal virtual void PreMeasureOverride(Size2D availableSize)
        {

        }

        /// <summary>
        /// Called immediately prior to <see cref="ArrangeOverride(Size2D, ArrangeOptions)"/>.
        /// </summary>
        /// <param name="finalSize">The element's final size.</param>
        /// <param name="options">A set of <see cref="ArrangeOptions"/> values specifying the options for this arrangement.</param>
        internal virtual void PreArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {

        }

        /// <summary>
        /// Called immediately prior to <see cref="Position(Point2D)"/>.
        /// </summary>
        /// <param name="position">The position of the element's parent element in absolute screen space.</param>
        internal virtual void PrePositionOverride(Point2D position)
        {

        }

        /// <inheritdoc/>
        internal override void ApplyStyledVisualStateTransition(UvssStyle style, String value)
        {
            Contract.Require(style, "style");
            Contract.RequireNotEmpty(value, "value");

            if (View != null && View.Stylesheet != null)
            {
                var storyboard = View.Stylesheet.InstantiateStoryboardByName(Ultraviolet, value);
                if (storyboard != null)
                {
                    var group = default(String);
                    var from  = default(String);
                    var to    = default(String);

                    switch (style.Arguments.Count)
                    {
                        case 2:
                            group = style.Arguments[0];
                            from  = null;
                            to    = style.Arguments[1];
                            break;

                        case 3:
                            group = style.Arguments[0];
                            from  = style.Arguments[1];
                            to    = style.Arguments[2];
                            break;

                        default:
                            throw new NotSupportedException();
                    }

                    VisualStateGroups.SetVisualStateTransition(group, from, to, storyboard);
                }
            }
        }

        /// <inheritdoc/>
        protected sealed override void DrawCore(UltravioletTime time, DrawingContext dc)
        {
            if (!LayoutUtil.IsDrawn(this))
                return;

            DrawOverride(time, dc);
        }

        /// <inheritdoc/>
        protected sealed override void UpdateCore(UltravioletTime time)
        {
            UpdateOverride(time);
        }

        /// <inheritdoc/>
        protected sealed override void StyleCore(UvssDocument stylesheet)
        {
            StyleOverride(stylesheet);

            base.StyleCore(stylesheet);
        }

        /// <inheritdoc/>
        protected sealed override Size2D MeasureCore(Size2D availableSize)
        {
            var margin = this.Margin;

            var xMargin = margin.Left + margin.Right;
            var yMargin = margin.Top + margin.Bottom;

            double minWidth, maxWidth;
            LayoutUtil.GetBoundedMeasure(Width, MinWidth, MaxWidth, out minWidth, out maxWidth);

            double minHeight, maxHeight;
            LayoutUtil.GetBoundedMeasure(Height, MinHeight, MaxHeight, out minHeight, out maxHeight);

            var availableWidthSansMargin  = Math.Max(0, availableSize.Width - xMargin);
            var availableHeightSansMargin = Math.Max(0, availableSize.Height - yMargin);

            var tentativeWidth  = Math.Max(minWidth, Math.Min(maxWidth, availableWidthSansMargin));
            var tentativeHeight = Math.Max(minHeight, Math.Min(maxHeight, availableHeightSansMargin));
            var tentativeSize   = new Size2D(tentativeWidth, tentativeHeight);

            PreMeasureOverride(tentativeSize);

            var measuredSize = MeasureOverride(tentativeSize);

            var measuredWidth  = xMargin + Math.Max(minWidth, measuredSize.Width);
            var measuredHeight = yMargin + Math.Max(minHeight, measuredSize.Height);

            var finalWidth  = Math.Max(0, measuredWidth);
            var finalHeight = Math.Max(0, measuredHeight);

            return new Size2D(finalWidth, finalHeight);
        }

        /// <inheritdoc/>
        protected sealed override Size2D ArrangeCore(RectangleD finalRect, ArrangeOptions options)
        {
            var margin = Margin;

            var xMargin = margin.Left + margin.Right;
            var yMargin = margin.Top + margin.Bottom;

            var desiredWidth  = (HorizontalAlignment == HorizontalAlignment.Stretch) ? finalRect.Width : Math.Min(DesiredSize.Width, finalRect.Width);
            var desiredHeight = (VerticalAlignment == VerticalAlignment.Stretch) ? finalRect.Height : Math.Min(DesiredSize.Height, finalRect.Height);

            var candidateSize = new Size2D(desiredWidth - xMargin, desiredHeight - yMargin);
            PreArrangeOverride(candidateSize, options);

            var usedSize = ArrangeOverride(candidateSize, options);

            var usedWidth  = Math.Min(usedSize.Width, candidateSize.Width);
            var usedHeight = Math.Min(usedSize.Height, candidateSize.Height);

            var fill   = (options & ArrangeOptions.Fill) == ArrangeOptions.Fill;
            var hAlign = fill ? HorizontalAlignment.Stretch : HorizontalAlignment;
            var vAlign = fill ? VerticalAlignment.Stretch : VerticalAlignment;

            if (hAlign == HorizontalAlignment.Stretch)
                usedWidth = finalRect.Width - xMargin;

            if (vAlign == VerticalAlignment.Stretch)
                usedHeight = finalRect.Height - yMargin;

            var xOffset = margin.Left;

            switch (hAlign)
            {
                case HorizontalAlignment.Center:
                    xOffset = margin.Left + (((finalRect.Width - xMargin) - usedWidth) / 2.0);
                    break;

                case HorizontalAlignment.Right:
                    xOffset = finalRect.Width - (usedWidth + margin.Right);
                    break;
            }

            var yOffset = margin.Top;

            switch (vAlign)
            {
                case VerticalAlignment.Center:
                    yOffset = margin.Top + (((finalRect.Height - yMargin) - usedHeight) / 2.0);
                    break;

                case VerticalAlignment.Bottom:
                    yOffset = finalRect.Height - (usedHeight + margin.Bottom);
                    break;
            }

            RenderOffset = new Point2D(xOffset, yOffset);

            usedSize = new Size2D(usedWidth, usedHeight);
            return usedSize;
        }

        /// <inheritdoc/>
        protected sealed override void PositionCore(Point2D position)
        {
            PrePositionOverride(position);
            PositionOverride(position);

            base.PositionCore(position);
        }

        /// <inheritdoc/>
        protected internal override void OnFocused()
        {
            VisualStateGroups.GoToState("focus", "focused");

            base.OnFocused();
        }

        /// <inheritdoc/>
        protected internal override void OnBlurred()
        {
            VisualStateGroups.GoToState("focus", "blurred");

            base.OnBlurred();
        }

        /// <summary>
        /// When overridden in a derived class, draws the element using the 
        /// specified <see cref="SpriteBatch"/> for a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Draw(UltravioletTime)"/>.</param>
        /// <param name="dc">The drawing context that describes the render state of the layout.</param>
        protected virtual void DrawOverride(UltravioletTime time, DrawingContext dc)
        {

        }

        /// <summary>
        /// When overridden in a derived class, updates the element's state for
        /// a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Update(UltravioletTime)"/>.</param>
        protected virtual void UpdateOverride(UltravioletTime time)
        {

        }

        /// <summary>
        /// When overridden in a derived class, applies the specified stylesheet
        /// to this element and to any child elements for 
        /// a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="stylesheet">The stylesheet to apply to this element and its children.</param>
        protected virtual void StyleOverride(UvssDocument stylesheet)
        {

        }

        /// <summary>
        /// When overridden in a derived class, calculates the element's desired 
        /// size and the desired sizes of any child elements for a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="availableSize">The size of the area which the element's parent has 
        /// specified is available for the element's layout.</param>
        /// <returns>The element's desired size, considering the size of any content elements.</returns>
        protected virtual Size2D MeasureOverride(Size2D availableSize)
        {
            return Size2D.Zero;
        }

        /// <summary>
        /// When overridden in a derived class, sets the element's final area relative to its 
        /// parent and arranges the element's children within its layout area for
        /// a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The element's final size.</param>
        /// <param name="options">A set of <see cref="ArrangeOptions"/> values specifying the options for this arrangement.</param>
        protected virtual Size2D ArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {
            return finalSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions the element in absolute screen space for 
        /// a <see cref="FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="position">The position of the element's parent element in absolute screen space.</param>
        protected virtual void PositionOverride(Point2D position)
        {

        }

        /// <summary>
        /// Raises the <see cref="WidthChanged"/> event.
        /// </summary>
        protected virtual void OnWidthChanged()
        {
            var temp = WidthChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="MinWidthChanged"/> event.
        /// </summary>
        protected virtual void OnMinWidthChanged()
        {
            var temp = MinWidthChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="MaxWidthChanged"/> event.
        /// </summary>
        protected virtual void OnMaxWidthChanged()
        {
            var temp = MaxWidthChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="HeightChanged"/> event.
        /// </summary>
        protected virtual void OnHeightChanged()
        {
            var temp = HeightChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="MinHeightChanged"/> event.
        /// </summary>
        protected virtual void OnMinHeightChanged()
        {
            var temp = MinHeightChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="MaxHeightChanged"/> event.
        /// </summary>
        protected virtual void OnMaxHeightChanged()
        {
            var temp = MaxHeightChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="MarginChanged"/> event.
        /// </summary>
        protected virtual void OnMarginChanged()
        {
            var temp = MarginChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="HorizontalAlignmentChanged"/> event.
        /// </summary>
        protected virtual void OnHorizontalAlignmentChanged()
        {
            var temp = HorizontalAlignmentChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="VerticalAlignmentChanged"/> event.
        /// </summary>
        protected virtual void OnVerticalAlignmentChanged()
        {
            var temp = VerticalAlignmentChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Gets the element's collection of visual state groups.
        /// </summary>
        protected VisualStateGroupCollection VisualStateGroups
        {
            get { return visualStateGroups; }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Width"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleWidthChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnWidthChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="MinWidth"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleMinWidthChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnMinWidthChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="MaxWidth"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleMaxWidthChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnMaxWidthChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Height"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleHeightChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnHeightChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="MinHeight"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleMinHeightChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnMinHeightChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="MaxHeight"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleMaxHeightChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnMaxHeightChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Margin"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleMarginChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnMarginChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="HorizontalAlignment"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleHorizontalAlignmentChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnHorizontalAlignmentChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="VerticalAlignment"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleVerticalAlignmentChanged(DependencyObject dobj)
        {
            var frameworkElement = (FrameworkElement)dobj;
            frameworkElement.OnVerticalAlignmentChanged();
        }

        // Standard visual state groups.
        private static readonly String[] VSGFocus = new[] { "focused", "blurred" };

        // Property values.
        private readonly VisualStateGroupCollection visualStateGroups;
    }
}
