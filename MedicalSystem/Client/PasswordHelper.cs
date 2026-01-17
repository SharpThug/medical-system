using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordHelper),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false));

        public static string GetBoundPassword(DependencyObject dp) => (string)dp.GetValue(BoundPassword);
        public static void SetBoundPassword(DependencyObject dp, string value) => dp.SetValue(BoundPassword, value);

        private static bool GetUpdatingPassword(DependencyObject dp) => (bool)dp.GetValue(UpdatingPassword);
        private static void SetUpdatingPassword(DependencyObject dp, bool value) => dp.SetValue(UpdatingPassword, value);

        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is not PasswordBox passwordBox)
                return;

            if (GetUpdatingPassword(passwordBox))
                return;

            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            passwordBox.Password = (string)e.NewValue ?? string.Empty;
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox)
                return;

            SetUpdatingPassword(passwordBox, true);
            SetBoundPassword(passwordBox, passwordBox.Password);
            SetUpdatingPassword(passwordBox, false);
        }
    }
}
