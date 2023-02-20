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
using System.Windows.Shapes;

namespace XDPM_App.SupWin
{
    /// <summary>
    /// Логика взаимодействия для TabEditWindow.xaml
    /// </summary>
    public partial class TabEditWindow : Window
    {
        private TabItemBox _box;
        private int _count = 1;
        public TabEditWindow(TabItemBox box)
        {
            InitializeComponent();
            _box = box;
        }

        private void TabNameConfirmedButton_Click(object sender, RoutedEventArgs e)
        {
            _box = new(TabNameTextBlock.Text.ToString(), _count);
            Close();
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
            => _count = 1;

        private void Button_Click2(object sender, RoutedEventArgs e)
            => _count = 2;

        private void Button_Click4(object sender, RoutedEventArgs e)
            => _count = 4;

        private void Button_Click9(object sender, RoutedEventArgs e)
            => _count = 9;
    }
}
