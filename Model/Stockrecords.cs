using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Stockrecords
    {
        public int seqNo { get; set; }
        public string pdtID { get; set; }
        public float lftNum { get; set; }
        public float orderNum { get; set; }
        public float invNum { get; set; }
        public string remark { get; set; }
    }
}
