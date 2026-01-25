using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Client
{
    public partial class MainAppWindow : Window
    {
        public ObservableCollection<Patient> Patients { get; set; }
        private ICollectionView _patientsView;
        private Point _mousePosition; // Для корректного Drag при максимизированном окне

        public MainAppWindow()
        {
            InitializeComponent();

            LoadDummyPatients();
            this.MouseLeftButtonDown += MainAppWindow_MouseLeftButtonDown;
            this.MouseMove += MainAppWindow_MouseMove;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь можно добавить контекстное меню
            MessageBox.Show("Меню настроек или дополнительных действий", "Меню",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Кнопка сворачивания окна
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Кнопка разворачивания/восстановления окна
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

        // Кнопка закрытия окна
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

        private void MainAppWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, был ли клик в области шапки (но не на кнопках)
            var hitElement = e.OriginalSource as FrameworkElement;

            // Если кликнули не на кнопке (или ее дочерних элементах)
            if (!IsChildOfWindowButton(hitElement))
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    // Запоминаем позицию мыши
                    _mousePosition = e.GetPosition(this);

                    // Захватываем мышь для отслеживания перемещения
                    this.CaptureMouse();
                }
            }
        }

        private void MainAppWindow_MouseMove(object sender, MouseEventArgs e)
        {
            // Если левая кнопка мыши нажата и мышь захвачена
            if (e.LeftButton == MouseButtonState.Pressed && this.IsMouseCaptured)
            {
                var currentPosition = e.GetPosition(this);

                // Если мышка переместилась достаточно (чтобы избежать случайных срабатываний)
                if (Math.Abs(currentPosition.X - _mousePosition.X) > 2 ||
                    Math.Abs(currentPosition.Y - _mousePosition.Y) > 2)
                {
                    // Освобождаем захват мыши
                    this.ReleaseMouseCapture();

                    // Обрабатываем переход из максимизированного состояния
                    if (this.WindowState == WindowState.Maximized)
                    {
                        // Вычисляем позицию для нормального состояния
                        var screenPoint = this.PointToScreen(currentPosition);
                        var relativeX = currentPosition.X / this.ActualWidth;

                        // Переходим в нормальный режим
                        this.WindowState = WindowState.Normal;

                        // Обновляем размеры окна
                        this.UpdateLayout();

                        // Устанавливаем новую позицию окна
                        this.Left = screenPoint.X - (relativeX * this.ActualWidth);
                        this.Top = screenPoint.Y - currentPosition.Y;
                    }

                    // Начинаем перетаскивание
                    this.DragMove();
                }
            }
        }

        // Альтернативная, более простая реализация без MouseMove
        private void MainAppWindow_MouseLeftButtonDown_Simple(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, был ли клик в области шапки (но не на кнопках)
            var hitElement = e.OriginalSource as FrameworkElement;

            // Если кликнули не на кнопке (или ее дочерних элементах)
            if (!IsChildOfWindowButton(hitElement))
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    // Для максимизированного окна
                    if (this.WindowState == WindowState.Maximized)
                    {
                        // Вычисляем относительную позицию клика
                        var mousePos = e.GetPosition(this);
                        var relativeX = mousePos.X / this.ActualWidth;
                        var relativeY = mousePos.Y / this.ActualHeight;

                        // Переходим в нормальный режим
                        this.WindowState = WindowState.Normal;

                        // Получаем позицию мыши на экране
                        var screenPoint = this.PointToScreen(mousePos);

                        // Устанавливаем размеры окна (можно оставить текущие или задать свои)
                        double newWidth = 1200; // или this.Width если оно задано
                        double newHeight = 800; // или this.Height если оно задано

                        // Позиционируем окно так, чтобы точка клика осталась под мышью
                        this.Left = screenPoint.X - (relativeX * newWidth);
                        this.Top = screenPoint.Y - (relativeY * newHeight);
                        this.Width = newWidth;
                        this.Height = newHeight;

                        // Ждем обновления layout
                        this.UpdateLayout();

                        // Повторно получаем позицию после изменения размера
                        mousePos = e.GetPosition(this);
                    }

                    // Вызываем стандартный DragMove
                    try
                    {
                        this.DragMove();
                    }
                    catch (InvalidOperationException)
                    {
                        // Игнорируем исключение, которое может возникнуть если окно не видно
                    }
                }
            }
        }

        // Проверяем, является ли элемент кнопкой окна
        private bool IsChildOfWindowButton(FrameworkElement element)
        {
            while (element != null)
            {
                if (element is Button button)
                {
                    // Проверяем, это ли наши кнопки управления окном
                    var content = button.Content?.ToString();
                    if (content == "–" || content == "□" || content == "✕" || content == "⋮")
                        return true;

                    // Альтернативно можно проверять по имени кнопок
                    if (button.Name == "MinimizeButton" ||
                        button.Name == "MaximizeButton" ||
                        button.Name == "CloseButton" ||
                        button.Name == "MenuButton")
                        return true;
                }

                // Также проверяем другие элементы, которые не должны обрабатывать Drag
                if (element is TextBox || element is ComboBox || element is DatePicker)
                    return true;

                element = element.Parent as FrameworkElement;
            }
            return false;
        }

        // Или более надежный метод с указанием конкретной панели для Drag
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

        // =========================================================
        // ЗАГРУЗКА ДАННЫХ
        // =========================================================
        private void LoadDummyPatients()
        {
            Patients = new ObservableCollection<Patient>();

            for (int i = 1; i <= 50; i++)
            {
                Patients.Add(new Patient
                {
                    CardNumber = $"CARD{i:000}",
                    AppointmentDate = new DateTime(2026, i % 12 + 1, 13),
                    BirthDate = new DateTime(1980 + i % 10, i % 12 + 1, 1),
                    FullName = $"Пациент {i}",
                    Phone = $"+7 900 000 0{i:00}{i:00}",
                    Gender = i % 2 == 0 ? "М" : "Ж",
                    Department = i % 3 == 0 ? "Терапия" : "Хирургия",
                    Diagnosis = i % 2 == 0 ? "ОРВИ" : "Грипп"
                });
            }

            _patientsView = CollectionViewSource.GetDefaultView(Patients);
            _patientsView.Filter = FilterPatients;

            MainPatientsDataGrid.ItemsSource = _patientsView;
        }

        // =========================================================
        // ФИЛЬТР
        // =========================================================
        private bool FilterPatients(object obj)
        {
            if (obj is not Patient p)
                return false;

            var from = DateFromPicker.SelectedDate;
            var to = DateToPicker.SelectedDate;

            if (from.HasValue && p.AppointmentDate < from.Value)
                return false;

            if (to.HasValue && p.AppointmentDate > to.Value)
                return false;

            return true;
        }

        // =========================================================
        // СОБЫТИЕ ИЗМЕНЕНИЯ ДАТЫ
        // =========================================================
        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            _patientsView?.Refresh();
        }

        // =========================================================
        // СБРОС ФИЛЬТРА
        // =========================================================
        /*private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            DateFromPicker.SelectedDate = null;
            DateToPicker.SelectedDate = null;
            _patientsView.Refresh();
        }*/
    }

    // =============================================================
    // МОДЕЛЬ
    // =============================================================
    public class Patient
    {
        public string CardNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Diagnosis { get; set; }
    }
}