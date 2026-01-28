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
        }
    }
}
