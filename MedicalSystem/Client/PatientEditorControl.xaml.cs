using System.Windows;
using System.Windows.Controls;

using Shared;

namespace Client
{
    public partial class PatientEditorControl : UserControl
    {
        public event Action<Patient> PatientSaved;
        public event Action CloseRequested;

        public PatientEditorControl()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var p = new Patient
            {
                /*FullName = NameBox.Text,
                CardNumber = "NEW",
                AppointmentDate = DateTime.Now,
                BirthDate = DateTime.Now.AddYears(-30),
                Phone = "",
                Gender = "М",
                Department = "",
                Diagnosis = ""*/
            };
            PatientSaved?.Invoke(p);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke();
        }
    }
}
