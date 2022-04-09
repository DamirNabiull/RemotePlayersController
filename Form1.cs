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
using System.Data.SqlServerCe;

namespace PlayersController
{
    public partial class Menu : Form
    {
        private SqlConnection _sqlConnection;
        private SqlDataAdapter _sqlDataAdapter;

        private SqlCeCommand _cmd;
        private SqlCeConnection _connection;
        private SqlCeDataAdapter _adapter;
        private string _connection_string = "DataSource=\"InnerDatabase.sdf\"; Password=\"pass\"";

        private DataTable _dataTable;
        private Checker _checker;
        private HttpWebRequest request = null;

        public Menu()
        {
            _checker = new Checker();
            InitializeComponent();
        }

        private void create_db()
        {
            try
            {
                SqlCeEngine engine = new SqlCeEngine(_connection_string);
                engine.CreateDatabase();

                _connection = new SqlCeConnection(_connection_string);
                _connection.Open();

                _cmd = new SqlCeCommand(@"create table Players
                    (
                    [Id]   INT           IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                    [Name] NVARCHAR (300) NOT NULL,
                    [IP]   NVARCHAR (15)  NOT NULL,
                    [Port] NVARCHAR (5)   NOT NULL,
                    [MAC]  NVARCHAR (50)  NOT NULL
                    )
                    ", _connection);
                _cmd.ExecuteNonQuery();
            }
            catch
            {
                Trace.WriteLine("DB exist");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            create_db();

            _connection = new SqlCeConnection(_connection_string);
            _connection.Open();

            _adapter = new SqlCeDataAdapter(@"select Name, IP, MAC from Players", _connection);
            _dataTable = new DataTable();
            _adapter.Fill(_dataTable);
            dataGridView1.DataSource = _dataTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = textBox1.Text, ip = textBox2.Text, MAC = textBox4.Text;
            String port = "8080";
            
            if (_checker.check_Ip(ip) && _checker.check_Port(port) && _checker.check_Name(name) && _checker.check_mac(MAC))
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
                        _dataTable.Clear();

                        _cmd = new SqlCeCommand("insert into Players(Name, IP, Port, Mac) values (@Name, @IP, @Port, @MAC)", _connection);
                        _cmd.Parameters.AddWithValue("@Name", name);
                        _cmd.Parameters.AddWithValue("@IP", ip);
                        _cmd.Parameters.AddWithValue("@Port", port);
                        _cmd.Parameters.AddWithValue("@MAC", MAC);

                        _cmd.ExecuteNonQuery();

                        _adapter.Fill(_dataTable);

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
                MessageBox.Show("Некорректный ip, mac или пустое имя");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                Player player = new Player();


                player.Name = ((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[0]).Value.ToString();
                player.IP = ((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[1]).Value.ToString();
                player.Port = "8080";
                player.MAC = ((DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells[2]).Value.ToString();

                PlayerControl pc = new PlayerControl(player);
                pc.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Пустая клетка");
            }
        }
    }
}
