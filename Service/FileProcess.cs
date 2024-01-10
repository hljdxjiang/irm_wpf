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
            int lineCnt=0;
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
            saveData(getFileName(filePath),_dictionary,lineCnt);

        }

        private void saveData(String fileName,Dictionary<string, List<string>> dict,int Cnt){
            using(var context=new MyDbContext()){
                string fileId=SnowflakeIdGenerator.NextId().ToString();
                DataList dataList = new DataList
                {
                    FileId=fileId,
                    CreateTime=DateTime.Now,
                    SampleCnt=Cnt
                };
                context.DataLists.Add(dataList);
                if(dict!=null&&dict.Count>0){
                    foreach (var pair in dict)
                    {
                        if(pair.Value.Any()){
                            foreach(var line  in pair.Value){
                                DataDetail dataDetail=new DataDetail{
                                    FileId=pair.Key,
                                    OrgLine=line
                                };
                                context.DataDetails.Add(dataDetail);
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
        }


        private string getFileName(string filePath){
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
    }
}
