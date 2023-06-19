using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Xml.Linq;

namespace HentaiDownloader
{

    public partial class Form1 : Form
    {
        public bool Download;
        private const string path = "https://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=250";
        private string data;
        private string url;
        private string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private Queue<string> images = new Queue<string>();
        private int i = 0;
        private string id;

        async private void HentaiDownload()
        {
            while (Download)
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();
                    XDocument doc = XDocument.Parse(data);
                    XElement posts = doc.Element("posts");
                    foreach (XElement post in posts.Elements("post"))
                    {

                        XElement file_url = post.Element("file_url");
                        url = file_url.Value;
                        id = post.Element("id").Value;
                        if (images.Contains(url) == false)
                        {
                            images.Enqueue(url);
                            var Responce_ = await client.GetAsync(url);
                            using (var file = System.IO.File.Create(DownloadPath + @"\" + id + ".png"))
                            {
                                i++;
                                var contentStream = await Responce_.Content.ReadAsStreamAsync();
                                await contentStream.CopyToAsync(file);
                                
                                if (!Download) { return; }
                            }
                        }
                    }
                  

                }
                await Task.Delay(30000);

            }
        }
        public Form1()
        {
            InitializeComponent();
            Download = false;
            button2.Enabled = false;
            




        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DownloadPath == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
            {
                if (System.IO.Directory.Exists(DownloadPath + @"\DownloadedHentai") == false)
                {
                    System.IO.Directory.CreateDirectory(DownloadPath + @"\DownloadedHentai");
                }
                DownloadPath += @"\DownloadedHentai";
            }

            Download = true;
            HentaiDownload();
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            Download = false;
            images.Clear();
            MessageBox.Show("Успешно загружено " + i + " фотографий");
            i = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    DownloadPath = fbd.SelectedPath;
                }
            }
        }
    }
}
