using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayersController
{
    public partial class Form1 : Form
    {
        private SqlConnection _sqlConnection;
        private SqlDataAdapter _sqlDataAdapter;
        private DataTable _dataTable;
        private Checker _checker;
        private HttpWebRequest request = null;

        public Form1()
        {
            _checker = new Checker();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            _sqlConnection = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}\PlayersDatabase.mdf;Integrated Security=True");
            _sqlConnection.Open();

            _sqlDataAdapter = new SqlDataAdapter("select Id, Name, IP, Port, MAC from Players", _sqlConnection);

            _dataTable = new DataTable();

            _sqlDataAdapter.Fill(_dataTable);


            dataGridView1.DataSource = _dataTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _dataTable.Clear();
            String name = textBox1.Text, ip = textBox2.Text, port = textBox3.Text, MAC = textBox4.Text;

            if (_checker.check_ip(ip) && _checker.check_port(port))
            {
                try
                {
                    request = (HttpWebRequest)WebRequest.Create($@"http://{ip}:{port}/ping");
                    request.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Trace.WriteLine(response.StatusCode);

                    Trace.WriteLine(response.StatusCode == HttpStatusCode.OK);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var sqlCmd = new SqlCommand("addPlayer", _sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@Name", name);
                        sqlCmd.Parameters.AddWithValue("@IP", ip);
                        sqlCmd.Parameters.AddWithValue("@Port", port);
                        sqlCmd.Parameters.AddWithValue("@MAC", MAC);
                        sqlCmd.ExecuteNonQuery();

                        _sqlDataAdapter.Fill(_dataTable);

                        dataGridView1.DataSource = _dataTable;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка соединения с плеером");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Нет соединения с плеером");
                    Trace.WriteLine(ex.ToString());
                }
                finally
                {
                    request?.Abort();
                }
            } 
            else
            {
                MessageBox.Show("Некорректный ip или port");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            Player player = new Player();

            player.Id = Int32.Parse(((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[0]).Value.ToString());
            player.Name = ((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[1]).Value.ToString();
            player.IP = ((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[2]).Value.ToString();
            player.Port = ((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[3]).Value.ToString();
            player.MAC = ((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[4]).Value.ToString();

            Trace.WriteLine(cell.Value);

            PlayerControl pc = new PlayerControl(player);
            pc.ShowDialog();
        }
    }
}
