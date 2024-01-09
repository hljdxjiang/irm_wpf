using IRM.Entity;
using IRM.Service;
using Microsoft.Win32;
using OxyPlot.Wpf;
using OxyPlot;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using irm_wpf.Entity;
using irm_wpf.EFCore;

namespace IRM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DataHistory : Window
    {
        public ICommand ViewCommand { get; private set; }

        public ICommand DelCommand { get; private set; }

        public DataHistory()
        {
            InitializeComponent();

            ViewCommand = new RelayCommand<DataList>(ViewCommandExecute);

            DelCommand = new RelayCommand<DataList>(DelCommandExecute);

        }

        private void LoadList(){
            using (var context = new MyDbContext()){
                var list=context.DataLists.ToList();
                //TODO 给dataGrid绑定list；
            }
        }

        private void ViewCommandExecute(DataList parameter)
        {
            // 操作查看
        }

        private void DelCommandExecute(DataList parameter)
        {
            // 操作删除
        }
    }
}