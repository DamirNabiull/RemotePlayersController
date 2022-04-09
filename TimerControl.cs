using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
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
    public partial class TimerControl : Form
    {
        private Checker _checker;
        private HttpWebRequest request;
        private SqlCeCommand _cmd;
        private SqlCeConnection _connection;
        private SqlCeDataAdapter _adapter;
        private string _connection_string = "DataSource=\"InnerDatabase.sdf\"; Password=\"pass\"";

        public TimerControl()
        {
            _checker = new Checker();
            InitializeComponent();
        }

        private string get_players()
        {
            _connection = new SqlCeConnection(_connection_string);
            _connection.Open();

            string to_return = "";

            DataSet ds = new DataSet();

            _adapter = new SqlCeDataAdapter(@"select Name, IP, MAC from Players where Port='8080'", _connection);

            _adapter.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                to_return += $"\"{ds.Tables[0].Rows[0][2]}\":\"" + $"{ds.Tables[0].Rows[0][1]}" + "\",";
            }

            Trace.WriteLine(to_return);

            return to_return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String ip = textBox1.Text, start = textBox2.Text, end = textBox3.Text;
            if (_checker.check_time(start) && _checker.check_time(end) && _checker.check_Ip(ip))
            {
                get_players();

                request = (HttpWebRequest)WebRequest.Create($@"http://{ip}:9090/set");
                try
                {
                    request.ContentType = "application/json";
                    request.Method = "POST";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        string json = "{" + get_players() +
                            $"\"start\":\"" + $"{start}" + "\"," +
                            $"\"end\":\"" + $"{end}" + "\"" + "}";
                        streamWriter.Write(json);

                        Trace.WriteLine(json);
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        MessageBox.Show("Задача выполнена");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка соединения с таймером");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Нет соединения с таймером");
                    Trace.WriteLine(ex.ToString());
                }
                finally
                {
                    request?.Abort();
                }
            }
            else
            {
                MessageBox.Show("Некорректный ip или время!\nФормат времени: 00. Где 00 указывает 12 часов ночи.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String ip = textBox1.Text;
            request = (HttpWebRequest)WebRequest.Create($@"http://{ip}:9090/suspendAll");
            request.Method = "GET";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Trace.WriteLine(response.StatusCode);

                Trace.WriteLine(response.StatusCode == HttpStatusCode.OK);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show("Процесс запущен");
                }
                else
                {
                    MessageBox.Show("Ошибка соединения с таймером");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Нет соединения с таймером");
                Trace.WriteLine(ex.ToString());
            }
            finally
            {
                request?.Abort();
            }
        }
    }
}
