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
        public Form1()
        {
            InitializeComponent();
            
            keyPressCount = 0;
            
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
        }
        private void OnKeyPress(object sender, KeyEventArgs e)   //检查什么键按下了
        {
           
            if (e.KeyCode == Keys.Space) //按下空格
            {
                if (keyPressCount == 0)
                {
                    this.timer1.Enabled = true;
                    keyPressCount++;
                }
                else
                {
                    this.timer1.Enabled = false;
                    keyPressCount = 0;
                }
            }
            if (e.KeyCode == Keys.Q) //按下Q键
            {
                this.Close();
            }
            if (e.KeyCode == Keys.H) //按下H键
            {

            }
        }

        private void OnTimeUp(object sender, EventArgs e)
        {
            SelectLuckUser();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
