using DAL_MySQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClient
{
    public partial class Form1 : Form
    {        
        static readonly Uri _baseAddress = new Uri(@"http://localhost:3408");
        static readonly Uri _address = new Uri(_baseAddress, @"/api/testWhIn");


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string whID = txtWhID.Text.Trim();
            string pdtID = txtPdtID.Text.Trim();
            string invNum = txtInNum.Text.Trim();
            
            string paramList = "{\"whID\":" + whID + ",\"pdtID\":\"" + pdtID + ",\"invNum\":\"" + invNum + "\"}";
            WebClient webClient = new WebClient();
            byte[] postData = Encoding.UTF8.GetBytes(paramList);            
            webClient.Headers.Add("Content-Type", "application/json"); //采取POST方式必须加的header
            webClient.Headers.Add("ContentLength", postData.Length.ToString());
            byte[] responseData = webClient.UploadData(_address, "POST", postData); //
            
            //webClient.Headers["Accept"] = "application/json";
            //webClient.Encoding = Encoding.UTF8;
            //webClient.DownloadStringCompleted += (send, es) =>
            //{
            //    if (es.Result != null)
            //    {
            //        var test = JsonConvert.DeserializeObject<mproduct>(es.Result);
            //        richTextBox1.Text = es.Result;
            //    }
            //    else
            //    {
            //        MessageBox.Show(es.Error.Message);
            //    }
            //};
            //webClient.DownloadStringAsync(_address);

        }
    }
}
