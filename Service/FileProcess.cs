using IRM.common;
using irm_wpf.common;
using irm_wpf.EFCore;
using irm_wpf.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRM.Service
{
    public class FileProcess
    {

        /**
         * 
         * 读取文件，并逐行处理
         */
        public void Process(String filePath, out Dictionary<string, List<string>> _dictionary)
        {
            _dictionary = new Dictionary<string, List<string>>();
            int lineCnt = 0;
            if (!filePath.Equals(String.Empty))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    String line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineCnt++;
                        LineProcess(line, _dictionary);
                    }
                }
            }
            saveData(getFileName(filePath), _dictionary, lineCnt);

        }

        private void saveData(String fileName, Dictionary<string, List<string>> dict, int Cnt)
        {
            using (var context = new MyDbContext())
            {
                string fileId = SnowflakeIdGenerator.NextId().ToString();
                DataList dataList = new DataList
                {
                    FileId = fileId,
                    CreateTime = DateTime.Now,
                    SampleCnt = Cnt
                };
                context.DataLists.Add(dataList);
                if (dict != null && dict.Count > 0)
                {
                    foreach (var pair in dict)
                    {
                        if (pair.Value.Any())
                        {
                            foreach (var line in pair.Value)
                            {
                                LineProcessService lineProcess = new LineProcessService();
                                var strs = lineProcess.ProcessLine(line);
                                double xValue, yValue, zValue;
                                getXYZValue(strs, out xValue, out yValue, out zValue);
                                DataDetail dataDetail = new DataDetail
                                {
                                    FileId = pair.Key,
                                    SampleID = strs[0],
                                    Temperature = strs[1],
                                    XValue = xValue.ToString(),
                                    YValue = yValue.ToString(),
                                    ZValue = zValue.ToString(),
                                    XOrg = strs[2],
                                    YOrg = strs[3],
                                    ZOrg = strs[4],
                                    C = strs[5],
                                    OrgLine = line
                                };
                                context.DataDetails.Add(dataDetail);
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
        }


        private string getFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        /**
         * 
         * 读取每行数据并按样品分类
         */
        private void LineProcess(String s, Dictionary<string, List<string>> _dictionary)
        {
            List<string> list = null;

            var title = getSampleTitle(s);
            try
            {
                list = _dictionary.First(x => x.Key == title).Value;
            }
            catch (InvalidOperationException)
            {
                list = new List<string>();
            }
            list.Add(s);
            _dictionary[title] = list;
        }


        /**
         * 
         * 获取样品标题
         */
        private string getSampleTitle(String line)
        {
            return line.Split(Constant.LineSpit)[0];
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
            xValue = Math.Round(Math.Abs(x * pow), 6);
            yValue = Math.Round(Math.Abs(y * pow), 6);
            zValue = Math.Round(Math.Abs(z * pow), 6);
        }
    }
}
