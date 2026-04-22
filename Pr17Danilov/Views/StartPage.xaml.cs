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
using System.Data.Entity;

namespace Pr17Danilov.Views
{
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var masters = db.Users.Where(u => u.Role == UserRole.Master && !u.IsFrozen).ToList();
                MasterCombo.ItemsSource = masters;
                MasterCombo.DisplayMemberPath = "FullName";
                MasterCombo.SelectedValuePath = "Id";

                var serviceTypes = db.ServiceTypes.ToList();
                ServiceTypeCombo.ItemsSource = serviceTypes;
                ServiceTypeCombo.DisplayMemberPath = "Name";
                ServiceTypeCombo.SelectedValuePath = "Id";

                var masterServices = db.MasterServices.ToList();
                var services = db.ServiceTypes.ToList();

                var result = new List<MasterViewModel>();
                foreach (var master in masters)
                {
                    var serviceIds = masterServices.Where(ms => ms.MasterId == master.Id).Select(ms => ms.ServiceTypeId).ToList();
                    var serviceNames = services.Where(s => serviceIds.Contains(s.Id)).Select(s => s.Name).ToList();

                    result.Add(new MasterViewModel
                    {
                        MasterId = master.Id,
                        MasterName = master.FullName,
                        Services = string.Join(", ", serviceNames)
                    });
                }
                MastersListView.ItemsSource = result;
            }
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            var selectedMasterId = MasterCombo.SelectedValue as int?;
            var selectedServiceId = ServiceTypeCombo.SelectedValue as int?;

            using (var db = new AppDbContext())
            {
                var masters = db.Users.Where(u => u.Role == UserRole.Master && !u.IsFrozen);

                if (selectedMasterId.HasValue)
                    masters = masters.Where(m => m.Id == selectedMasterId.Value);

                var masterServices = db.MasterServices.ToList();
                var services = db.ServiceTypes.ToList();

                var result = new List<MasterViewModel>();
                foreach (var master in masters.ToList())
                {
                    var serviceIds = masterServices.Where(ms => ms.MasterId == master.Id).Select(ms => ms.ServiceTypeId).ToList();

                    if (selectedServiceId.HasValue && !serviceIds.Contains(selectedServiceId.Value))
                        continue;

                    var serviceNames = services.Where(s => serviceIds.Contains(s.Id)).Select(s => s.Name).ToList();

                    result.Add(new MasterViewModel
                    {
                        MasterId = master.Id,
                        MasterName = master.FullName,
                        Services = string.Join(", ", serviceNames)
                    });
                }
                MastersListView.ItemsSource = result;
            }
        }

        private void SelectMaster_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as Button).Tag as MasterViewModel;
            if (vm != null)
            {
                NavigationService?.Navigate(new AppointmentPage(vm.MasterId));
            }
        }
    }

    public class MasterViewModel
    {
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public string Services { get; set; }
    }
}
