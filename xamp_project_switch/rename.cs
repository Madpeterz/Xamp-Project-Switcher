﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xamp_project_switch
{
    public partial class rename : Form
    {
        public string read_value = "";
        public rename(string defaulttext="")
        {
            InitializeComponent();
            textBox1.Text = defaulttext;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            read_value = textBox1.Text;
            this.Close();
        }

        private void rename_Load(object sender, EventArgs e)
        {

        }
    }
}
