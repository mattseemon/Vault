using Seemon.Vault.Core.Helpers.Extensions;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Seemon.Vault.Helpers.Controls
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty SecurePasswordBindingProperty = DependencyProperty.RegisterAttached(
            "SecurePassword",
            typeof(SecureString),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(new SecureString(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AttachedPropertyValueChanged));

        private static readonly DependencyProperty _passwordBindingMarshallerProperty = DependencyProperty.RegisterAttached(
            "PasswordBindingMarshaller",
            typeof(PasswordBindingMarshaller),
            typeof(PasswordBoxHelper),
            new PropertyMetadata());

        public static void SetSecurePassword(PasswordBox element, SecureString secureString) => element.SetValue(SecurePasswordBindingProperty, secureString);

        public static SecureString GetSecurePassword(PasswordBox element) => element.GetValue(SecurePasswordBindingProperty) as SecureString;

        private static void AttachedPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;

            if (passwordBox.GetValue(_passwordBindingMarshallerProperty) is not PasswordBindingMarshaller bindingMarshaller)
            {
                bindingMarshaller = new PasswordBindingMarshaller(passwordBox);
                passwordBox.SetValue(_passwordBindingMarshallerProperty, bindingMarshaller);
            }

            bindingMarshaller.UpdatePasswordBox(e.NewValue as SecureString);
        }

        private class PasswordBindingMarshaller
        {
            private readonly PasswordBox _passwordBox;
            private bool _isMarshalling;

            public PasswordBindingMarshaller(PasswordBox passwordBox)
            {
                _passwordBox = passwordBox;
                _passwordBox.PasswordChanged += OnPasswordChanged;
            }

            public void UpdatePasswordBox(SecureString password)
            {
                if (_isMarshalling)
                    return;

                _isMarshalling = true;

                try
                {
                    _passwordBox.Password = password.ToUnsecuredString();
                }
                finally
                {
                    _isMarshalling = false;
                }
            }

            private void OnPasswordChanged(object sender, RoutedEventArgs e)
            {
                if (_isMarshalling)
                    return;

                _isMarshalling = true;
                try
                {
                    SetSecurePassword(_passwordBox, _passwordBox.SecurePassword.Copy());
                }
                finally
                {
                    _isMarshalling = false;
                }
            }
        }
    }
}
