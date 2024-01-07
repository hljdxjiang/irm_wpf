using IRM.Entity;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace IRM.Service
{
    public class OxyPlotService
    {

        private List<TableItem> dataModels;

        private double maxYValue;//Y值最大值

        private double maxXValue;//X轴最大值

        private LineProcessService _lineProcessService;

        public OxyPlotService()
        {
            _lineProcessService = new LineProcessService();
        }

        public void GetListView(string title, List<string> list, out PlotModel plotModel, out List<TableItem> items)
        {
            buildDefaultPlotView(title, out plotModel);
            //buildDefaultDataGrid(out dataGrid);

            items = new List<TableItem>();


            var lineSeriesX = new LineSeries
            {
                Color = OxyColors.Green,
                LineJoin = OxyPlot.LineJoin.Bevel,
                Title = "soft",
                StrokeThickness = 3,
                MarkerType = MarkerType.Circle, // 设置标记类型
                MarkerSize = 8,                 // 设置标记大小
                MarkerFill = OxyColors.Green    // 设置标记填充颜色
            };
            var lineSeriesY = new LineSeries
            {
                Color = OxyColors.Red,
                LineJoin = OxyPlot.LineJoin.Bevel,
                StrokeThickness = 3,
                Title = "medium",
                MarkerType = MarkerType.Square, // 设置标记类型
                MarkerSize = 8,                 // 设置标记大小
                MarkerFill = OxyColors.Red    // 设置标记填充颜色
            };
            var lineSeriesZ = new LineSeries
            {
                Color = OxyColors.Blue,
                LineJoin = OxyPlot.LineJoin.Bevel,
                Title = "hard",
                StrokeThickness = 3,
                MarkerType = MarkerType.Triangle, // 设置标记类型
                MarkerSize = 8,                 // 设置标记大小
                MarkerFill = OxyColors.Blue    // 设置标记填充颜色
            };

            if (list != null && list.Count > 0)
            {
                maxYValue = 0.00D;
                maxXValue = 0.00D;
                foreach (var s in list)
                {
                    var strs = _lineProcessService.ProcessLine(s);
                    if (strs.Count > 6)
                    {
                        double xValue, yValue, zValue;
                        int temp = getTemperature(strs[1]);
                        getXYZValue(strs, out xValue, out yValue, out zValue);

                        lineSeriesX.Points.Add(new DataPoint(temp, xValue));
                        lineSeriesY.Points.Add(new DataPoint(temp, yValue));
                        lineSeriesZ.Points.Add(new DataPoint(temp, zValue));
                        items.Add(new TableItem
                        {
                            SampleID = strs[0],
                            Temperature = strs[1],
                            XValue = xValue.ToString(),
                            YValue = yValue.ToString(),
                            ZValue = zValue.ToString(),
                            XOrg = strs[2],
                            YOrg = strs[3],
                            ZOrg = strs[4],
                            C = strs[5],
                        });
                    }

                }

                plotModel.Series.Add(lineSeriesX);
                plotModel.Series.Add(lineSeriesY);
                plotModel.Series.Add(lineSeriesZ);
                buildLineAnnotation(plotModel);

                //dataGrid.BindingContext = new DataViewModel();
                //dataGrid.ItemsSource = dataModels;
            }
        }

        private void buildLineAnnotation(PlotModel plotModel)
        {

            var annotation = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                X = 5,
                TextPosition = new DataPoint(2, 5),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,

                //MinimumX = int.Parse(maxXValue.ToString()) - 100, // X 坐标位置，可以根据需要调整
                //MaximumX = int.Parse(maxXValue.ToString()) - 50,
                //Y = int.Parse(maxXValue.ToString()) - 5,
                Text = "Soft",
                StrokeThickness = 3,
                LineStyle = LineStyle.Solid, // 设置为 None，以消除线的显示
                Color = OxyColors.Gray, // 设置形状的颜色
                Layer = AnnotationLayer.AboveSeries
            };
            plotModel.Annotations.Add(annotation);
        }


        private void buildDefaultPlotView(string title, out PlotModel plotModel)
        {
            plotModel = new PlotModel { Title = title, Legends = { }, IsLegendVisible = true };
            var xares = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "IRM(A/m)"
            };
            var yares = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Temperature(℃)",
            };
            plotModel.Axes.Add(xares);
            plotModel.Axes.Add(yares);

            var xlegen = new Legend
            {
                LegendTitleColor = OxyColor.FromUInt32(666666),
                LegendPosition = LegendPosition.RightTop,
                LegendSymbolLength = 80,
                LegendItemSpacing = 40,
                LegendFontSize = 20,
                LegendOrientation = LegendOrientation.Horizontal,
            };
            plotModel.Legends.Add(xlegen);
        }

        /**
         * 根据不同的Y最大值，生成图表
         * 
         */
        private void buildDefaultPlotViewWithYValue(string title, out PlotModel plotModel)
        {
            plotModel = new PlotModel { Title = title };
            var xares = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "IRM(A/m)"
            };
            var yares = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Temperature(℃)"
            };
            plotModel.Axes.Add(xares);
            plotModel.Axes.Add(yares);
        }


        private int getTemperature(String input)
        {
            int index = input.Length - 1;
            while (index >= 0 && char.IsDigit(input[index]))
            {
                index--;
            }

            if (index == input.Length - 1)
            {
                return 0;
            }
            // 提取数字部分并转换为整数
            string numberPart = input.Substring(index + 1);
            int result;
            if (int.TryParse(numberPart, out result))
            {
                this.maxXValue = this.maxXValue > result ? this.maxXValue : result;
                return result;
            }
            else
            {
                // 如果提取失败，你可以选择抛出异常或者返回默认值
                throw new ArgumentException("字符串尾部没有有效的数字部分");
            }
        }

        private void getXYZValue(List<String> list, out double xValue, out double yValue, out double zValue)
        {
            double x, y, z;
            int s6;
            double.TryParse(list[2], out x);
            double.TryParse(list[3], out y);
            double.TryParse(list[4], out z);
            int.TryParse(list[5], out s6);
            var pow = Math.Pow(10, s6);
            xValue = Math.Round(Math.Abs(x * pow), 4);
            yValue = Math.Round(Math.Abs(y * pow), 4);
            zValue = Math.Round(Math.Abs(z * pow), 4);
            this.maxYValue = maxYValue > xValue ? maxYValue : xValue;
        }   
    }
}
