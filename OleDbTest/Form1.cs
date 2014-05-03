using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OleDbTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            OleDbDataAdapter oledbAdapter;
            string strconn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Temp\snitz_forums_2000.mdb;Persist Security Info=False;";
            using (OleDbConnection conn = new OleDbConnection(strconn))
            {
                string sql = textBox1.Text;
                oledbAdapter = new OleDbDataAdapter(sql, conn);
                oledbAdapter.Fill(ds);
                oledbAdapter.Dispose();
                conn.Close();

            }
            dataGridView1.DataSource = ds.Tables[0];
        }
    }
}
