using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Image_label_tool
{

    public partial class Form1 : Form
    {
        // 資料夾路徑
        string folderPath;
        int pmax = 0, pnow = 0; //檔案進度
        string[] files;
        bool cl = false;

        public Form1()
        {
            InitializeComponent();
            
            //當前路徑
            //string current_dir = System.Environment.CurrentDirectory;

            StreamReader sr = new StreamReader(@"config.label.txt");
            while (!sr.EndOfStream)
            {
                // 每次讀取一行，直到檔尾
                string line = sr.ReadLine();
                AddNewButton(line);
            }
            sr.Close();
            Shown += (o, e) => MessageBox.Show("longer_lung@aseglobal.com","聯絡資訊");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            //fdb.RootFolder = Environment.SpecialFolder.MyDocuments;
            folderDialog.Description = "選擇存放要標記的圖片的資料夾...";
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 清除listbox1
                listBox1.Items.Clear();

                folderPath = folderDialog.SelectedPath;
                folderTextBox.Text = folderPath;

                files = Directory.GetFiles(folderPath);
                string[] dirs = Directory.GetDirectories(folderPath);
                if (files.Length >= 0)
                {
                    progressBar1.Value = 0;
                    progressBar1.Maximum = files.Length - 1;
                    pmax = files.Length;
                    label2.Text = String.Format("{0}/{1}", pnow + 1, pmax);
                    foreach (string file in files)
                    {
                        listBox1.Items.Add(Path.GetFileName(file));
                    }
                    // listbox focus on first
                    listBox1.SelectedIndex = 0;
                    listBox1.Focus();

                    if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                    if (cl)
                    {
                        pictureBox1.Image = Add_WarningLine_Into_Img(Image.FromFile(files[pnow]));
                    }
                    else {
                        pictureBox1.Image = Image.FromFile(files[pnow]);
                    }
                    
                    
                }
            }
#if DEBUG
                MessageBox.Show(folderDialog.SelectedPath);
#endif

        }

        private void next_Click(object sender, EventArgs e)
        {
            //NextImgNoLabel();
            Next();
        }

        private void prev_Click(object sender, EventArgs e)
        {
            //PrevImgNoLabel();
            Prev();
        }


        int c = 0;
        public System.Windows.Forms.Button AddNewButton(string line)
        {
            System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
            this.Controls.Add(btn);
            btn.Top = c * 28 + 100;
            btn.Left = 764;
            btn.Text = line;
            btn.Click += new System.EventHandler(btn_Click);
            c = c + 1;
            return btn;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            string label = (sender as Button).Text;

            NextImg(label);
        }


        void NextImg(string label)
        {
            // 檔案名稱
            SaveToCSV(Path.GetFileName(files[pnow]), label);
            Next();
        }
        void Next()
        {
            if (pnow >= pmax - 1)
            {
                MessageBox.Show("最後一張照片！");
            }
            else
            {
                pnow = pnow + 1;
                progressBar1.Value++;
                listBox1.SelectedIndex = pnow;
                listBox1.Focus();
                if (cl)
                {
                    pictureBox1.Image = Add_WarningLine_Into_Img(Image.FromFile(files[pnow]));
                }
                else {
                    pictureBox1.Image = Image.FromFile(files[pnow]);
                }
                label2.Text = String.Format("{0}/{1}", pnow + 1, pmax);
            }
        }
        void Prev()
        {
            if (pnow <= 0)
            {
                MessageBox.Show("第一張照片！");
            }
            else
            {
                pnow = pnow - 1;
                progressBar1.Value--;
                listBox1.SelectedIndex = pnow;
                listBox1.Focus();
                if (cl)
                {
                    pictureBox1.Image = Add_WarningLine_Into_Img(Image.FromFile(files[pnow]));
                }
                else {
                    pictureBox1.Image = Image.FromFile(files[pnow]);
                }
                
                label2.Text = String.Format("{0}/{1}", pnow + 1, pmax);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string curItem = listBox1.SelectedItem.ToString();
            int index = listBox1.FindString(curItem);
            pnow = index;
            if (cl)
            {
                pictureBox1.Image = Add_WarningLine_Into_Img(Image.FromFile(files[pnow]));
            }
            else {
                pictureBox1.Image = Image.FromFile(files[index]);
            }
            label2.Text = String.Format("{0}/{1}", index + 1, pmax);
            progressBar1.Value = index;
        }

        private void clean_btn_Click(object sender, EventArgs e)
        {
            folderTextBox.Text = null;

            progressBar1.Value = 0;

            pnow = 0;

            label2.Text = "/";

            listBox1.Items.Clear();

            pictureBox1.Image.Dispose();
            pictureBox1.Image = null;
        }

        void SaveToCSV(string imageName, string label)
        {
            /*
            string[] sArray = imageName.Split('\\');
            int idx = sArray.Length;
            imageName = sArray[idx - 1];
            folderPath
            */
#if DEBUG
            MessageBox.Show(imageName, label);
#endif
            string labelFolder = folderPath + @"\Label";
            if (!Directory.Exists(labelFolder))
            {
                Directory.CreateDirectory(labelFolder);
            }

            //utf-8-BOM 
            StreamWriter sw = new StreamWriter(labelFolder + @"\label.csv", true, Encoding.UTF8);
            sw.WriteLine(imageName + "," + label);
            sw.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                cl = true;
            }
            else {
                cl = false;
            }
        }

        private Bitmap Add_WarningLine_Into_Img(Image img)
        {
            Bitmap bitmap=null;
            try
            {
                Bitmap image = new Bitmap(new Bitmap(img));
                Graphics g = Graphics.FromImage(image);
                Pen pen1 = new Pen(Color.Red, 3);
                g.DrawLine(pen1, image.Width / 10, image.Height / 10, image.Width - image.Width / 10, image.Height / 10);
                g.DrawLine(pen1, image.Width - image.Width / 10, image.Height / 10, image.Width - image.Width / 10, image.Height - image.Height / 10);
                g.DrawLine(pen1, image.Width - image.Width / 10, image.Height - image.Height / 10, image.Width / 10, image.Height - image.Height / 10);
                g.DrawLine(pen1, image.Width / 10, image.Height - image.Height / 10, image.Width / 10, image.Height / 10);
                bitmap = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return bitmap;
        }
    }

}
