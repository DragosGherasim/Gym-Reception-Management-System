using System.Windows;
using System.Windows.Controls;

namespace Gym_Reception_Management_System.Utils.Password
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
                typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata("", OnPasswordPropertyChanged));

        public static readonly DependencyProperty CanBindProperty =
            DependencyProperty.RegisterAttached("CanBind",
                typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
                typeof(PasswordBoxHelper));

        public static void SetCanBind(DependencyObject dp, bool value)
        {
            dp.SetValue(CanBindProperty, value);
        }

        public static bool GetCanBind(DependencyObject dp) => (bool)dp.GetValue(CanBindProperty);

        public static string GetPassword(DependencyObject dp) =>
            (string)dp.GetValue(PasswordProperty);

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
                                                      DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox)) return;

            passwordBox.PasswordChanged -= PasswordChanged;

            if (!GetIsUpdating(passwordBox)) passwordBox.Password = (string)e.NewValue;
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

            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}