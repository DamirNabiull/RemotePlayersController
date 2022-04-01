using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayersController
{
    public partial class PlayerControl : Form
    {
        private Player _player;
        private HttpWebRequest request;
        public PlayerControl(Player player)
        {
            _player = player;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PlayerControl_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = _player.Name;
            this.textBox2.Text = _player.IP;
            this.textBox3.Text = _player.Port;
            this.textBox4.Text = _player.MAC;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendGetRequest("setURL");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendGetRequest("setImage");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendGetRequest("setVideo");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.textBox5.Text.Length > 0)
            {
                request = (HttpWebRequest)WebRequest.Create($@"http://{_player.IP}:{_player.Port}/uploadURL");
                try
                {
                    request.ContentType = "application/json";
                    request.Method = "POST";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        string json = "{\"url\":\"" + $"{this.textBox5.Text}" + "\"}";
                        streamWriter.Write(json);
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        MessageBox.Show("Задача выполнена");
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
                MessageBox.Show("Введите ссылку!");
            }
        }

        private bool SendGetRequest(string route)
        {
            bool status = false;
            request = (HttpWebRequest)WebRequest.Create($@"http://{_player.IP}:{_player.Port}/{route}");
            request.Method = "GET";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Trace.WriteLine(response.StatusCode);

                Trace.WriteLine(response.StatusCode == HttpStatusCode.OK);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show("Процесс запущен");
                    status = true;
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
            return status;
        }
    }
}
