using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace irm_wpf.Entity {
    public class DataDetail {

        public int ID { get; set; }
        public int DataID { get; set; }
        public string SampleID { get; set; }
        public string Temperature { get; set; }
        public string XValue { get; set; }
        public string YValue { get; set; }
        public string ZValue { get; set; }
        public string XOrg { get; set; }
        public string YOrg { get; set; }
        public string ZOrg { get; set; }
        public string C { get; set; }
    }
}