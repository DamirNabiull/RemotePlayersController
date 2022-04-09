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

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.JPG;*.PNG)|*.JPG;*.PNG"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    Trace.WriteLine(open_dialog.FileName);
                    Byte[] bytes = File.ReadAllBytes(open_dialog.FileName);
                    String file = Convert.ToBase64String(bytes);

                    string[] subs = open_dialog.FileName.Split('\\');
                    Trace.WriteLine(subs[subs.Length - 1]);
                    Trace.WriteLine(file);

                    request = (HttpWebRequest)WebRequest.Create($@"http://{_player.IP}:{_player.Port}/uploadImage");
                    try
                    {
                        request.ContentType = "application/json";
                        request.Method = "POST";

                        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                        {
                            string json = "{\"image\":\"" + $"{file}\"," +
                                "\"image_name\":\"" + $"{subs[subs.Length - 1]}" + "\"}";
                            Trace.WriteLine(json);
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
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
