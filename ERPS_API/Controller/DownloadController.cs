using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ERPS_API.Controller
{
    public class DownloadController : ApiController
    {
        [HttpGet]
        [Route("api/GetFile")]
        public HttpResponseMessage GetFile(string url, string name)
        {
            try
            {
                var FilePath = System.Web.Hosting.HostingEnvironment.MapPath(url);
                var stream = new FileStream(FilePath, FileMode.Open);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name
                };
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}
