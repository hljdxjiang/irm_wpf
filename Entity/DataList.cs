using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace irm_wpf.Entity {
    public class DataList {
        public int ID { get; set; }

        public required string FileId{get;set;}

        public required string FileName { get; set; }

        public DateTime CreateTime { get; set; }

        public string  userID { get; set; }

        public int SampleCnt { get; set; }
    }
}