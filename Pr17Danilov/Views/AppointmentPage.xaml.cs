using BeautySalonApp.Models;
using Pr17Danilov.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pr17Danilov.Views
{
    public partial class AppointmentPage : Page
    {
        private int _masterId;

        public AppointmentPage(int masterId)
        {
            InitializeComponent();
            _masterId = masterId;
            LoadServices();
        }

        private void LoadServices()
        {
            using (var db = new AppDbContext())
            {
                var masterServices = db.MasterServices.Where(ms => ms.MasterId == _masterId).Select(ms => ms.ServiceTypeId).ToList();
                var services = db.ServiceTypes.Where(s => masterServices.Contains(s.Id)).ToList();
                ServiceCombo.ItemsSource = services;
            }
        }

        private void ServiceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker.SelectedDate = null;
            TimeListView.ItemsSource = null;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceCombo.SelectedItem == null || !DatePicker.SelectedDate.HasValue)
                return;

            var selectedDate = DatePicker.SelectedDate.Value.Date;
            var service = ServiceCombo.SelectedItem as ServiceType;

            var allSlots = new List<DateTime>();
            for (int hour = 9; hour <= 19; hour++)
            {
                allSlots.Add(selectedDate.AddHours(hour));
            }

            using (var db = new AppDbContext())
            {
                var busySlots = db.Appointments
                    .Where(a => a.MasterId == _masterId && a.DateTime.Date == selectedDate && !a.IsCompleted)
                    .Select(a => a.DateTime)
                    .ToList();

                var freeSlots = allSlots.Where(slot => !busySlots.Contains(slot)).ToList();
                TimeListView.ItemsSource = freeSlots.Select(s => s.ToString("HH:mm")).ToList();
            }
        }

        private void TimeSlot_Click(object sender, RoutedEventArgs e)
        {
            if (!SessionService.IsAuthenticated)
            {
                MessageBox.Show("Для записи необходимо войти в аккаунт");
                return;
            }

            if (SessionService.CurrentUser.Role != UserRole.Client)
            {
                MessageBox.Show("Только клиенты могут записываться");
                return;
            }

            var timeStr = (sender as Button).Content.ToString();
            var date = DatePicker.SelectedDate.Value;
            var time = DateTime.Parse(timeStr);
            var datetime = date.Date + time.TimeOfDay;
            var service = ServiceCombo.SelectedItem as ServiceType;

            var result = MessageBox.Show($"Записаться на {service.Name} на {datetime:dd.MM.yyyy HH:mm}?", "Подтверждение", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var appointment = new Appointment
                    {
                        DateTime = datetime,
                        ClientId = SessionService.CurrentUser.Id,
                        MasterId = _masterId,
                        ServiceTypeId = service.Id,
                        IsCompleted = false,
                        Comment = "",
                        PaymentMethod = ""
                    };
                    db.Appointments.Add(appointment);
                    db.SaveChanges();
                }
                MessageBox.Show("Вы успешно записались!");
                NavigationService?.GoBack();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
