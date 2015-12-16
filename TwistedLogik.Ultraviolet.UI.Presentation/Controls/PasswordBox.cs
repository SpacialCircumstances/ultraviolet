﻿using System;
using System.Security;
using System.Text;
using TwistedLogik.Nucleus;
using TwistedLogik.Nucleus.Text;
using TwistedLogik.Ultraviolet.Input;
using TwistedLogik.Ultraviolet.UI.Presentation.Controls.Primitives;
using TwistedLogik.Ultraviolet.UI.Presentation.Input;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Controls
{
    /// <summary>
    /// Represents a control designed for securely entering passwords.
    /// </summary>
    [UvmlKnownType(null, "TwistedLogik.Ultraviolet.UI.Presentation.Controls.Templates.PasswordBox.xml")]
    public sealed class PasswordBox : Control
    {
        /// <summary>
        /// Initializes the <see cref="PasswordBox"/> type.
        /// </summary>
        static PasswordBox()
        {
            EventManager.RegisterClassHandler(typeof(PasswordBox), TextBoxBase.SelectionChangedEvent, new UpfRoutedEventHandler(HandleSelectionChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordBox"/> control.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public PasswordBox(UltravioletContext uv, String name)
            : base(uv, name)
        {

        }

        /// <summary>
        /// Gets the password which has been entered into the password box.
        /// </summary>
        /// <returns>A string containing the password which has been entered into the password box.</returns>
        [SecurityCritical]
        public String GetPassword()
        {
            if (PART_Editor != null)
                return PART_Editor.GetPassword();

            return String.Empty;
        }

        /// <summary>
        /// Gets the password which has been entered into the password box.
        /// </summary>
        /// <param name="stringBuilder">A <see cref="StringBuilder"/> to populate with the password which has been entered into the password box.</param>
        [SecurityCritical]
        public void GetPassword(StringBuilder stringBuilder)
        {
            Contract.Require(stringBuilder, "stringBuilder");

            if (PART_Editor != null)
                PART_Editor.GetPassword(stringBuilder);
        }

        /// <summary>
        /// Sets the password which has been entered into the password box.
        /// </summary>
        /// <param name="value">A <see cref="String"/> containing the password to set.</param>
        [SecurityCritical]
        public void SetPassword(String value)
        {
            if (PART_Editor != null)
                PART_Editor.SetPassword(value);
        }

        /// <summary>
        /// Sets the password which has been entered into the password box.
        /// </summary>
        /// <param name="value">A <see cref="StringBuilder"/> containing password to set.</param>
        [SecurityCritical]
        public void SetPassword(StringBuilder value)
        {
            if (PART_Editor != null)
                PART_Editor.SetPassword(value);
        }

        /// <summary>
        /// Sets the password which has been entered into the password box.
        /// </summary>
        /// <param name="value">A <see cref="StringSegment"/> containing password to set.</param>
        [SecurityCritical]
        public void SetPassword(StringSegment value)
        {
            if (PART_Editor != null)
                PART_Editor.SetPassword(value);
        }

        /// <summary>
        /// Clears the password box's content.
        /// </summary>
        public void Clear()
        {
            if (PART_Editor != null)
                PART_Editor.Clear();
        }

        /// <summary>
        /// Selects the specified range of text.
        /// </summary>
        /// <param name="start">The index of the first character to select.</param>
        /// <param name="length">The number of characters to select.</param>
        public void Select(Int32 start, Int32 length)
        {
            if (PART_Editor != null)
                PART_Editor.Select(start, length);
        }

        /// <summary>
        /// Selects the entirety of the editor's text.
        /// </summary>
        public void SelectAll()
        {
            if (PART_Editor != null)
                PART_Editor.SelectAll();
        }
        
        /// <summary>
        /// Pastes the contents of the clipboard at the current caret position.
        /// </summary>
        public void Paste()
        {
            if (PART_Editor != null)
                PART_Editor.Paste();
        }

        /// <summary>
        /// Gets or sets the character which is used to mask the entered password.
        /// </summary>
        public Char PasswordChar
        {
            get { return GetValue<Char>(PasswordCharProperty); }
            set { SetValue(PasswordCharProperty, value); }
        }
        
        /// <summary>
        /// Gets an instance of <see cref="SecureString"/> that represents the password box's current content.
        /// </summary>
        public SecureString SecurePassword
        {
            get
            {
                if (PART_Editor != null)
                    return PART_Editor.SecurePassword;

                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selection highlight is displayed when the password box does not have focus.
        /// </summary>
        public Boolean IsInactiveSelectionHighlightEnabled
        {
            get { return GetValue<Boolean>(IsInactiveSelectionHighlightEnabledProperty); }
            set { SetValue(IsInactiveSelectionHighlightEnabledProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the password box has focus and selected text.
        /// </summary>
        public Boolean IsSelectionActive
        {
            get { return GetValue<Boolean>(IsSelectionActiveProperty); }
        }

        /// <summary>
        /// Gets or sets the maximum length of the password which is entered into the password box.
        /// </summary>
        public Int32 MaxLength
        {
            get { return GetValue<Int32>(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// Occurs when the password box's password changes.
        /// </summary>
        public event UpfRoutedEventHandler PasswordChanged
        {
            add { AddHandler(PasswordChangedEvent, value); }
            remove { RemoveHandler(PasswordChangedEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PasswordChar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordCharProperty = DependencyProperty.Register("PasswordChar", typeof(Char), typeof(PasswordBox),
            new PropertyMetadata<Char>('*', PropertyMetadataOptions.None, HandlePasswordCharChanged));

        /// <summary>
        /// Identifies the <see cref="IsInactiveSelectionHighlightEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInactiveSelectionHighlightEnabledProperty = 
            TextBoxBase.IsInactiveSelectionHighlightEnabledProperty.AddOwner(typeof(PasswordBox));

        /// <summary>
        /// Identifies the <see cref="IsSelectionActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectionActiveProperty =
            TextBoxBase.IsSelectionActiveProperty.AddOwner(typeof(PasswordBox));

        /// <summary>
        /// Identifies the <see cref="MaxLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty = 
            TextBox.MaxLengthProperty.AddOwner(typeof(PasswordBox));

        /// <summary>
        /// Identifies the <see cref="PasswordChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PasswordChangedEvent = EventManager.RegisterRoutedEvent("PasswordChanged",
            RoutingStrategy.Bubble, typeof(UpfRoutedEventHandler), typeof(PasswordBox));

        /// <inheritdoc/>
        protected override void OnMouseDown(MouseDevice device, MouseButton button, ref RoutedEventData data)
        {
            if (button == MouseButton.Left)
            {
                Focus();
                CaptureMouse();
            }

            if (PART_Editor != null && IsMouseWithinEditor())
                PART_Editor.HandleMouseDown(device, button, ref data);

            data.Handled = true;
            base.OnMouseDown(device, button, ref data);
        }

        /// <inheritdoc/>
        protected override void OnMouseUp(MouseDevice device, MouseButton button, ref RoutedEventData data)
        {
            if (button == MouseButton.Left)
            {
                ReleaseMouseCapture();
            }

            if (PART_Editor != null && IsMouseWithinEditor())
                PART_Editor.HandleMouseUp(device, button, ref data);

            data.Handled = true;
            base.OnMouseUp(device, button, ref data);
        }

        /// <inheritdoc/>
        protected override void OnMouseDoubleClick(MouseDevice device, MouseButton button, ref RoutedEventData data)
        {
            if (PART_Editor != null && IsMouseWithinEditor())
                PART_Editor.HandleMouseDoubleClick(device, button, ref data);

            base.OnMouseDoubleClick(device, button, ref data);
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseDevice device, Double x, Double y, Double dx, Double dy, ref RoutedEventData data)
        {
            if (PART_Editor != null)
                PART_Editor.HandleMouseMove(device, ref data);

            data.Handled = true;
            base.OnMouseMove(device, x, y, dx, dy, ref data);
        }

        /// <inheritdoc/>
        protected override void OnLostMouseCapture(ref RoutedEventData data)
        {
            if (PART_Editor != null)
                PART_Editor.HandleLostMouseCapture();

            data.Handled = true;
            base.OnLostMouseCapture(ref data);
        }

        /// <inheritdoc/>
        protected override void OnGotKeyboardFocus(KeyboardDevice device, IInputElement oldFocus, IInputElement newFocus, ref RoutedEventData data)
        {
            Ultraviolet.GetInput().ShowSoftwareKeyboard(KeyboardMode.Text);

            if (PART_Editor != null)
                PART_Editor.HandleGotKeyboardFocus();

            UpdateIsSelectionActive();

            base.OnGotKeyboardFocus(device, oldFocus, newFocus, ref data);
        }

        /// <inheritdoc/>
        protected override void OnLostKeyboardFocus(KeyboardDevice device, IInputElement oldFocus, IInputElement newFocus, ref RoutedEventData data)
        {
            Ultraviolet.GetInput().HideSoftwareKeyboard();

            if (PART_Editor != null)
                PART_Editor.HandleLostKeyboardFocus();

            UpdateIsSelectionActive();

            base.OnLostKeyboardFocus(device, oldFocus, newFocus, ref data);
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyboardDevice device, Key key, ModifierKeys modifiers, ref RoutedEventData data)
        {
            if (PART_Editor != null)
                PART_Editor.HandleKeyDown(device, key, modifiers, ref data);

            base.OnKeyDown(device, key, modifiers, ref data);
        }

        /// <inheritdoc/>
        protected override void OnTextInput(KeyboardDevice device, ref RoutedEventData data)
        {
            if (PART_Editor != null)
                PART_Editor.HandleTextInput(device, ref data);

            base.OnTextInput(device, ref data);
        }

        /// <summary>
        /// Occurs when the control handles a <see cref="TextBoxBase.SelectionChangedEvent"/> routed event.
        /// </summary>
        private static void HandleSelectionChanged(DependencyObject dobj, ref RoutedEventData data)
        {
            var passwordBox = (PasswordBox)dobj;
            passwordBox.UpdateIsSelectionActive();

            if (passwordBox.PART_Editor != null && data.OriginalSource == passwordBox.PART_Editor)
                data.Handled = true;
        }

        /// <summary>
        /// Occurs when the value of the <see cref="PasswordChar"/> dependency property changes.
        /// </summary>
        private static void HandlePasswordCharChanged(DependencyObject element, Char oldValue, Char newValue)
        {
            var passwordBox = (PasswordBox)element;
            if (passwordBox.PART_Editor != null)
                passwordBox.PART_Editor.ReplaceTextWithMask(newValue);
        }

        /// <summary>
        /// Gets a value indicating whether the mouse is currently inside of the editor.
        /// </summary>
        private Boolean IsMouseWithinEditor()
        {
            var scrollViewer = (PART_Editor == null) ? null : PART_Editor.Parent as ScrollViewer;

            var mouseTarget = (Control)scrollViewer ?? this;
            var mouseBounds = mouseTarget.Bounds;

            return mouseBounds.Contains(Mouse.GetPosition(mouseTarget));
        }

        /// <summary>
        /// Updates the value of the <see cref="IsSelectionActive"/> property.
        /// </summary>
        private void UpdateIsSelectionActive()
        {
            var isSelectionActive = IsKeyboardFocusWithin;

            if (PART_Editor == null || PART_Editor.SelectionLength == 0)
                isSelectionActive = false;

            var oldValue = GetValue<Boolean>(IsSelectionActiveProperty);
            if (oldValue != isSelectionActive)
            {
                SetValue(TextBoxBase.IsSelectionActivePropertyKey, isSelectionActive);
            }
        }

        // Component references.
        private readonly PasswordEditor PART_Editor = null;        
    }
}