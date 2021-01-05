using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPS_API.App_Start
{
    public class UserDataPackage<T>
    {
        public int code { get; set; }
        public T data { get; set; }
    }
}