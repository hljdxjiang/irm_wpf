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
    public partial class DataHistory : Page
    {
        public DataHistory()
        {
            InitializeComponent();

        }

        private void LoadList()
        {
            using (var context = new MyDbContext())
            {
                DateTime currentDate = DateTime.Now;
                DateTime thirtyDaysAgo = currentDate.AddDays(-30);
                var list = context.DataLists.Where(data => data.CreateTime >= thirtyDaysAgo).ToList();//只查30天以内的数据
                //TODO 给dataGrid绑定list；
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataList selectedItem = (DataList)dataGrid.SelectedItem;
            using (var context = new MyDbContext)
            {
                context.DataLists.Remove(selectedItem);
                var list = context.DataDetails.Where(item => item.FileId == selectedItem.FileId);
                if (list.Any())
                {
                    context.DataDetails.RemoveRange(list);
                }
                context.SaveChanges();

            }
        }

        private void buildDictory(DataDetail[] details，out Dictionary<string, List<string>> _dictionary)
        {
            _dictionary = new Dictionary<string, List<string>>();
            if (details.Length > 0)
            {
                foreach (var item in details)
                {
                    List<string> list = null;
                    try
                    {
                        list = _dictionary.First(x => x.Key == item.SampleID).Value;
                    }
                    catch (InvalidOperationException)
                    {
                        list = new List<string>();
                    }
                    list.Add(s);
                    _dictionary[item.SampleID] = list;
                }
            }
        }



        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataList selectedItem = (DataList)dataGrid.SelectedItem;
            using (var context = new MyDbContext())
            {
                var list = context.DataDetails.Where(item => item.FileId == selectedItem.FileId);
                if(list.Any()){
                    HistoryWIndow historyWIndow=new HistoryWIndow();
                    Dictionary<string, List<string>>  _dictionary;
                    buildDictory(list.ToArray(),out _dictionary);
                    historyWIndow.DataDictionary=_dictionary;
                    historyWIndow.ShowDialog();
                }
            }
        }
    }
}