﻿using IRM.Entity;
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

namespace IRM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DataProcess : Page
    {
        private String file;

        Dictionary<string, List<string>> _dictionary;

        private OxyPlotService _oxyPlotService;
        private FileProcess _fileProcess;

        public DataProcess()
        {
            InitializeComponent();
            _oxyPlotService = new OxyPlotService();
            _fileProcess = new FileProcess();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = new OpenFileDialog();
            if (result.ShowDialog() == true) {

                filePath.Content = "文件路径:"+result.FileName;
                file = result.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (filePath.Equals(String.Empty))
            {
                var mbx = MessageBox.Show("请选择要处理的文件", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (mbx == MessageBoxResult.Yes) {
                    Button_Click(sender, e);
                }
            }

            try
            {
                _fileProcess.Process(file, out _dictionary);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }

            try
            {
                RenderNext(0);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        private void RenderNext(int idx)
        {
            if (idx > _dictionary.Count || idx < 0 || _dictionary.Count == 0)
            {
                //DisplayAlert("提示", "没有符合条件的数据生成", "确定");
                return;
            }
            PlotModel plotModel = null;
            //TableView tableView = null;
            //DataGrid dataGrid = null;
            List<TableItem> list;
            var item = _dictionary.ElementAt(idx);
            _oxyPlotService.GetListView(item.Key, item.Value, out plotModel, out list);
            plotView.Model = plotModel;

            var nextButtons = new StackPanel();
            nextButtons.Orientation = Orientation.Horizontal;
            nextButtons.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            if (idx > 0)
            {
                //上一个
                var lastButton = new Button
                {
                    Content = "上一个(" + _dictionary.ElementAt(idx - 1).Key + ")",
                    Margin = new Thickness(5)
                };
                lastButton.Click += (sender, args) => RenderNext(idx - 1);
                nextButtons.Children.Add(lastButton);
            }

            //下载按钮
            Button downButtoon = new Button
            {
                Content = "下载" + item.Key,
                Margin = new Thickness(5)
            };
            //下载按钮绑定方法
            downButtoon.Click += (sender, args) => download_click(item.Key, plotModel);

            nextButtons.Children.Add(downButtoon);

            if (idx < _dictionary.Count - 1)
            {
                //下一个
                var nextButton = new Button
                {
                    Content = "下一个(" + _dictionary.ElementAt(idx + 1).Key + ")",
                    Margin = new Thickness(5)
                };
                nextButton.Click += (sender, args) => RenderNext(idx + 1);
                nextButtons.Children.Add(nextButton);

            }

            //更新按钮展示前清空
            btns.Children.Clear();

            btns.Children.Add(new Label
            {
                Content = (idx + 1).ToString() + "/" + _dictionary.Count,
                HorizontalContentAlignment=System.Windows.HorizontalAlignment.Center,
                FontSize = 20,
            });
            btns.Children.Add(nextButtons);
            dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            dataGrid.Height = 450;
            dataGrid.ItemsSource= list;


            /*DataViewModel dataView = new DataViewModel();
            dataView.Items = list;
            uiGrid.BindingContext = dataView;
            uiGrid.BackgroundColor = Colors.White;
            uiGrid.Background = Colors.White;
            uiGrid.HeaderBackground = Colors.Gray;
            uiGrid.HeaderBordersVisible = true;
            uiGrid.HeaderHeight = 50;
            uiGrid.RowHeight = 35;
            uiGrid.BorderColor = Colors.White;
            uiGrid.ItemsSource = list;*/
        }

        private async void download_click(string key, PlotModel plotModel)
        {
            var result = new OpenFolderDialog();
            if (result.ShowDialog() ==true)
            {
                var fileName = System.IO.Path.GetFileName(file) + key + ".svg";
                var outPath = result.FolderName;

                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                // 创建 SVG 渲染器
                var svgExporter = new OxyPlot.Wpf.SvgExporter { Width = 800, Height = 600 };

                // 将 PlotModel 导出为 SVG 字符串
                var svgString = svgExporter.ExportToString(plotModel);

                System.IO.File.WriteAllText(System.IO.Path.Combine(outPath, fileName), svgString);

                MessageBox.Show("下载成功，文件路径：" + outPath,"提示", MessageBoxButton.OK,MessageBoxImage.Information);
                
            }

        }
    }
}