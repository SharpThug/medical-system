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

        private int _newPatientCounter = 1;

        public MainAppWindow()
        {
            InitializeComponent();

            PatientsPageControl.RequestNewPatientTab += OpenNewPatientTab;

            //LoadDummyPatients();
        }

        private void OpenNewPatientTab()
        {
            var editor = new PatientEditorControl();

            var tab = new TabItem
            {
                Header = $"Новый пациент {_newPatientCounter++}",
                Content = editor
            };

            editor.PatientSaved += p =>
            {
                PatientsPageControl.Patients.Add(p);
                PatientsPageControl.MainPatientsDataGrid.Items.Refresh();
                MainTabControl.Items.Remove(tab);
            };

            editor.CloseRequested += () =>
            {
                MainTabControl.Items.Remove(tab);
            };

            MainTabControl.Items.Add(tab);
            MainTabControl.SelectedItem = tab;
        }

        /*private void NewPatient_Click(object sender, RoutedEventArgs e)
        {
            var editor = new PatientEditorControl();

            var tab = new TabItem
            {
                Header = $"Новый пациент {_newPatientCounter++}",
                Content = editor
            };

            // сохранить пациента
            editor.PatientSaved += p =>
            {
                Patients.Add(p);
                _patientsView.Refresh();
                MainTabControl.Items.Remove(tab);
            };

            // закрыть вкладку
            editor.CloseRequested += () =>
            {
                MainTabControl.Items.Remove(tab);
            };

            MainTabControl.Items.Add(tab);
            MainTabControl.SelectedItem = tab;
        }*/

        /*private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь можно добавить контекстное меню
            MessageBox.Show("Меню настроек или дополнительных действий", "Меню",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }*/

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
        /*private void LoadDummyPatients()
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
        }*/

        // =========================================================
        // ФИЛЬТР
        // =========================================================
        /*private bool FilterPatients(object obj)
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
        }*/
        
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
    
}