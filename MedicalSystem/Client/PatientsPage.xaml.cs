using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class PatientsPage : UserControl
    {
        public PatientsPage()
        {
            InitializeComponent();
        }

        public void Initialize(IPatientService patientService)
        {
            var vm = new PatientsViewModel(patientService);
            DataContext = vm;

            // Асинхронная загрузка пациентов
            vm.LoadPatientsAsync();

            // Подписка на создание новой вкладки
            vm.RequestNewPatientTab += () =>
            {
                // Родительское окно открывает вкладку
                if (Window.GetWindow(this) is MainAppWindow mainWindow)
                {
                    mainWindow.OpenNewPatientTab();
                }
            };
        }
    }
}
