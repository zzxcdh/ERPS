using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPS_API.App_Start
{
    /// <summary>
    /// 状态码
    /// </summary>
    public class ApiStatusCode
    {
        /// <summary>
        /// OK
        /// </summary>
        public const int OK = 0;

        /// <summary>
        /// 失败
        /// </summary>
        public const int FAIL = 1;

        /// <summary>
        /// 异常
        /// </summary>
        public const int EXCEPTION = 2;
    }
}