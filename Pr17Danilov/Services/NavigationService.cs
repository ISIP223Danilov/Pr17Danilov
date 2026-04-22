using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pr17Danilov.Services
{
    public static class NavigationService
    {
        private static Frame _mainFrame;

        public static void Initialize(Frame mainFrame)
        {
            _mainFrame = mainFrame;
        }

        public static void NavigateTo(Page page)
        {
            _mainFrame?.Navigate(page);
        }

        public static void GoBack()
        {
            if (_mainFrame != null && _mainFrame.CanGoBack)
                _mainFrame.GoBack();
        }
    }
}
