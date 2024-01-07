using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRM.common
{
    public partial class Constant
    {
        public static string Regex_ = @"^[-+]?\d*\.?\d+(-\d*\.?\d+)*$";

        public static string RegexNum = @"^[^a-zA-Z]*$";

        public static string RegexDouble = @"[-+]?\d*\.\d+|[-+]?\d+";

        public static string LineSpit = " ";

    }
}
