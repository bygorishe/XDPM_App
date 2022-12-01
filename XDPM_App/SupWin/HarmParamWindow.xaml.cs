using XDPM_App.ADMP;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace XDPM_App
{
    public partial class HarmParamWindow : Window
    {
        readonly int n;
        readonly List<TextBox> listA, listf;
        readonly Data data;
        public HarmParamWindow(int N, Data data)
        {
            InitializeComponent();

            this.n = N;
            this.data = data;
            data.HarmParams = new(n);
            listA = new(n);
            listf = new(n);
            for (int i = 0; i < n; i++)
            {
                RowDefinition rowInfo = new() { MaxHeight = 35 };
                grid.RowDefinitions.Add(rowInfo);

                TextBlock num = new();
                num.Text += i;
                Grid.SetRow(num, i);
                Grid.SetColumn(num, 0);
                num.Margin = new Thickness(5, 10, 0, 0);
                num.HorizontalAlignment = HorizontalAlignment.Center;
                grid.Children.Add(num);

                TextBox A = new();
                Grid.SetRow(A, i);
                Grid.SetColumn(A, 1);
                A.Margin = new Thickness(5, 10, 5, 0);
                grid.Children.Add(A);
                listA.Add(A);

                TextBox f = new();
                Grid.SetRow(f, i);
                Grid.SetColumn(f, 2);
                f.Margin = new Thickness(5, 10, 5, 0);
                grid.Children.Add(f);
                listf.Add(f);
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e) => Close();

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < n; i++)
            {
                if (double.TryParse(listA[i].Text, out double value1)
                    && double.TryParse(listf[i].Text, out double value2))
                    data.HarmParams.Add(new Common.HarmParam(value1, value2));
                else
                {
                    MessageBox.Show("Не верны параметры");
                    break;
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
