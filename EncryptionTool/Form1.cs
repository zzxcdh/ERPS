using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EncryptionTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            string enStr = txtEncrpt.Text;
            txtDesencript.Text = EncriptionFunction.Encryption(enStr);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string enStr = txtDesencript.Text;
            txtEncrpt.Text = EncriptionFunction.Decrypt(enStr);
        }
    }
}
