using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Shared;

namespace Client
{
    public partial class MainAppWindow : Window
    {
        private ICollectionView _patientsView;
        private Point _mousePosition; // Для корректного Drag при максимизированном окне
        private readonly IPatientService _patientService;

        private int _newPatientCounter = 1;

        public MainAppWindow(IPatientService patientService)
        {
            InitializeComponent();

            // После того как XAML создал PatientsPageControl
            PatientsPageControl.Initialize(patientService);
        }

        public void OpenNewPatientTab()
        {
            var editor = new PatientEditorControl();

            // Создаём кнопку крестика
            var closeButton = new Button
            {
                Content = "✕",
                Width = 16,
                Height = 16,
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var headerText = new TextBlock
            {
                Text = $"Новый пациент {_newPatientCounter++}",
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(headerText);
            headerPanel.Children.Add(closeButton);

            var tab = new TabItem
            {
                Header = headerPanel,
                Content = editor
            };

            // Подписка на крестик
            closeButton.Click += (s, e) =>
            {
                MainTabControl.Items.Remove(tab);
            };

            

            editor.CloseRequested += () =>
            {
                MainTabControl.Items.Remove(tab);
            };

            MainTabControl.Items.Add(tab);
            MainTabControl.SelectedItem = tab;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                // Меняем символ на кнопке
                if (sender is Button btn)
                {
                    btn.Content = "□";
                }
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                // Меняем символ на кнопке
                if (sender is Button btn)
                {
                    btn.Content = "❐"; // Двойной квадрат для восстановления
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Можно добавить подтверждение закрытия
            var result = MessageBox.Show("Закрыть приложение?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        // Обновление символа на кнопке максимизации при изменении состояния окна
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Если окно максимизировано, сначала делаем его нормальным
                if (this.WindowState == WindowState.Maximized)
                {
                    // Устанавливаем позицию окна до выполнения DragMove
                    var mousePos = e.GetPosition(this);
                    var screenPos = this.PointToScreen(mousePos);

                    // Временно отключаем максимизацию
                    this.WindowState = WindowState.Normal;

                    // Рассчитываем новую позицию окна так, чтобы мышка была примерно на том же месте
                    this.Left = screenPos.X - mousePos.X;
                    this.Top = screenPos.Y - mousePos.Y;

                    // Немного обновляем layout перед началом перемещения
                    this.UpdateLayout();
                }

                // Вызываем стандартный DragMove
                this.DragMove();
            }
        }
        // Дополнительно: обработчик для предотвращения случайного Drag на элементах
        private void PreventDragOnControls(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            _patientsView?.Refresh();
        }
    }
}