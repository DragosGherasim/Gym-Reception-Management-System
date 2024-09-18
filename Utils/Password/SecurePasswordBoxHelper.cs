using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Gym_Reception_Management_System.Utils.Password
{
    public static class SecurePasswordBoxHelper
    {
        public static readonly DependencyProperty SecurePasswordProperty =
            DependencyProperty.RegisterAttached("SecurePassword",
                typeof(SecureString), typeof(SecurePasswordBoxHelper),
                new FrameworkPropertyMetadata(null, OnSecurePasswordPropertyChanged));

        public static readonly DependencyProperty CanBindProperty =
            DependencyProperty.RegisterAttached("CanBind",
                typeof(bool), typeof(SecurePasswordBoxHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
                typeof(SecurePasswordBoxHelper));

        public static void SetCanBind(DependencyObject dp, bool value)
        {
            dp.SetValue(CanBindProperty, value);
        }

        public static bool GetCanBind(DependencyObject dp) => (bool)dp.GetValue(CanBindProperty);

        public static SecureString GetSecurePassword(DependencyObject dp) =>
            (SecureString)dp.GetValue(SecurePasswordProperty);

        public static void SetSecurePassword(DependencyObject dp, SecureString value)
        {
            dp.SetValue(SecurePasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnSecurePasswordPropertyChanged(DependencyObject sender,
                                                            DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox)) return;

            passwordBox.PasswordChanged -= PasswordChanged;

            if (!GetIsUpdating(passwordBox)) passwordBox.Password = SecureStringToString((SecureString)e.NewValue);
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender,
                                   DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            if ((bool)e.OldValue) passwordBox.PasswordChanged -= PasswordChanged;

            if ((bool)e.NewValue) passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            SetIsUpdating(passwordBox, true);

            if (passwordBox == null) return;

            SetSecurePassword(passwordBox, StringToSecureString(passwordBox.Password));
            SetIsUpdating(passwordBox, false);
        }

        private static SecureString StringToSecureString(string password)
        {
            var secureString = new SecureString();
            foreach (var c in password) secureString.AppendChar(c);

            return secureString;
        }

        private static string SecureStringToString(SecureString secureString)
        {
            var ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }
    }
}