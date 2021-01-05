using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPS_API.App_Start
{
    public class BaseDataPackage<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<T> DataList { get; set; }
        public List<object> ObjList { get; set; }

        public Dictionary<string, string> Keys { get; set; }
    }
}