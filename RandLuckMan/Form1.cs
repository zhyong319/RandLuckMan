using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace RandLuckMan
{
    public partial class Form1 : Form
    {
        XmlDocument xmlDocument;
        XmlNodeList xmlNodeList;
        int keyPressCount;
        bool configModeFlag;
        int mouseClickCouter; //鼠标单击次数计数
        Label selectLable; // 保存选中的控件
        
        public struct LUCKMANINFO
        {
            public string section;
            public string name;
        } ;
        LUCKMANINFO luckManInfo;
        public Form1()
        {
            InitializeComponent();
            mouseClickCouter = 0;
            configModeFlag = false;
           
            selectLable = null;
            keyPressCount = 0;
            this.WindowState = FormWindowState.Maximized; //全屏显示
            this.FormBorderStyle = FormBorderStyle.None;
            LoadXML("UserInfoLib.xml"); //加载XML文件
            if (File.Exists("background.jpg")) //如果有背景图片，加载背景图片
            {
                try
                 {
                    this.BackgroundImage = Image.FromFile("background.jpg");
                }

                catch (FileNotFoundException)
                {
                    MessageBox.Show("没有找到指定的 userInfoLib.xml 文件，请检查");
                }
            }

            if (File.Exists("LuckMan.txt"))  //文件存在
            {
                File.Delete("LuckMan.txt"); //删除原LUCKMAN.TXT文件
                using (StreamWriter sw = File.CreateText("LuckMan.txt"))
                {
                    sw.WriteLine("README");
                    sw.WriteLine("该文本仅保存本次随机抽选结果！");
                    sw.WriteLine("=====================================");
                }
            }

            this.label3.Location = new Point(0,Screen.GetBounds(this).Bottom-30); //设置LABEL3控件的位置

        }

        private int GetRandNumber(int upNum) //获取随机数
        {
            int randNum;
            DateTime dateTime = DateTime.Now;
            Random random = new Random((int)dateTime.Ticks);
            randNum = random.Next(upNum);
            return randNum;
        }

        private void LoadXML(string filePath) //加载XML文件
        {
            xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(filePath);
            }
            catch (XmlException)
            {
                MessageBox.Show("在 XML 中没有加载或解析错误.");
                return;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("UserInfoLib.XML文档没有找到,请检查并确认文件名");
                return;
            }
            XmlElement xmlElement = xmlDocument.DocumentElement;
            xmlNodeList = xmlElement.ChildNodes;
        }

        private void SelectLuckUser() //随机选择选中的用户
        {
            int num = 0;          
            num = GetRandNumber(xmlNodeList.Count);
            string strSection, strName;
            XmlNodeList childNodeList = xmlNodeList.Item(num).ChildNodes;
            strSection = childNodeList.Item(0).InnerText;
            strName = childNodeList.Item(1).InnerText;
            this.label1.Text = strSection;
            this.label2.Text = strName;
            this.label3.Text = num.ToString();
            luckManInfo.name = strName;
            luckManInfo.section = strSection;
        }
        private void OnKeyPress(object sender, KeyEventArgs e)   //检查什么键按下了
        {
           
            if (e.KeyCode == Keys.Space) //按下空格
            {
                if (keyPressCount == 0)
                {
                    this.timer1.Enabled = true;
                    keyPressCount++;
                    return;
                }
                else
                {
                    this.timer1.Enabled = false;
                    keyPressCount = 0;
                    Write(luckManInfo);
                    return;
                }
            }
            if (e.KeyCode == Keys.Q) //按下Q键
            {
                this.Close();
                return;
            }
            if (e.KeyCode == Keys.H) //按下H键
            {
                // Insert code here
                return;
            }
            if (e.KeyCode == Keys.S) //按下S键
            {
                configModeFlag = true; //进入配置模式
                this.label3.Text = "进入配置模式";
                return;
            }
            if (e.KeyCode == Keys.Escape) //按下ESC键
            {
                configModeFlag = false;
                selectLable = null;
                this.label3.Text = "退出配置模式";
                return;
            }
        }

        private void OnTimeUp(object sender, EventArgs e)
        {
            SelectLuckUser();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (configModeFlag == true)
            {
                
                selectLable = this.label1;
                
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (configModeFlag == true)
            {
               
                selectLable = this.label2;
            }
        }

        private void Write(LUCKMANINFO man)
        {
            DateTime dt = DateTime.Now;
            string strDateTime = dt.ToLocalTime().ToString();
            string filePath = "LuckMan.txt";
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(strDateTime + "  " + man.section + "  " + man.name);
                sw.WriteLine("------------------------------------------------------------------");
            }        

        }

        private void OnMouseDownLabel1(object sender, MouseEventArgs e)  //label控件1上鼠标单击
        {
           
            mouseClickCouter++;
            if (mouseClickCouter >=2)
            {
                selectLable = null;
                mouseClickCouter = 0;
            }
            this.label3.Text = "Mouse Click Down. " + mouseClickCouter.ToString() + " times!";
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (configModeFlag == true)
            {
                if(mouseClickCouter ==1 )  selectLable.Location = new Point(e.X, e.Y);
            }
        }

        private void OnMouseDownLabel2(object sender, MouseEventArgs e) //labe2控件1上鼠标单击
        {
            mouseClickCouter++;
            if (mouseClickCouter >= 2)
            {
                selectLable = null;
                mouseClickCouter = 0;
            }
            this.label3.Text = "Mouse Click Down. " + mouseClickCouter.ToString() + " times!";
        }
    }
}
