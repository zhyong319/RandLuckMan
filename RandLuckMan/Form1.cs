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
        bool configModeFlag;//APP配置模式
        bool errorModeFlag;//APP出错模式
        int mouseClickCouter; //鼠标单击次数计数
        Label selectLable; // 保存选中的控件
        string strNameListFile; //保存待抽将人员名单的xml文件名
        string strImageBK; //保存背景图片文件名
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
            errorModeFlag = false;
            selectLable = null;
            keyPressCount = 0;
            this.WindowState = FormWindowState.Maximized; //全屏显示
            this.FormBorderStyle = FormBorderStyle.None;
         
            CreateInitLocationConfigureFile(); //检查LocationConfigure文件是否存在，否则创建一个
            LoadLocationConfigureFile(); //解析配置文件，读取相应的数据
            this.label3.Location = new Point(0,Screen.GetBounds(this).Bottom-30); //设置LABEL3控件的位置

            LoadXML(strNameListFile); //加载XML文件         
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
           
            try
            {
                xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);
                XmlElement xmlElement = xmlDocument.DocumentElement;
                xmlNodeList = xmlElement.ChildNodes;
            }
            catch (XmlException)
            {
                MessageBox.Show("在 XML 中没有加载或解析错误.");
                errorModeFlag = true;
                return;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(strNameListFile + " 文档没有找到,请检查并确认文件存在!");
                errorModeFlag = true;
            }
            
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
            if (errorModeFlag == true)
            {
                MessageBox.Show("请根据软件启动时的提示，解决错误问题后再试！");
                this.Close();
            }
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

        private void Write(LUCKMANINFO man) //将抽中的人员信息写入LuckMan.txt文件
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
           // this.label3.Text = "Mouse Click Down. " + mouseClickCouter.ToString() + " times!";
        }

        private void OnMouseMove(object sender, MouseEventArgs e) // 鼠标移动的消息处理
        {
            if (configModeFlag == true)
            {
                if (mouseClickCouter == 1)
                {
                    selectLable.Location = new Point(e.X, e.Y);
                    XmlDocument xmlDoc = new XmlDocument(); //保存新坐标到文件
                    string filePath = @"LocationConfigrue.xml";
                    xmlDoc.Load(filePath);
                    
                    if (selectLable.Name == label1.Name)
                    {
                        xmlDoc.SelectNodes("/配置/部门/坐标/X").Item(0).InnerText = e.X.ToString();
                        xmlDoc.SelectNodes("/配置/部门/坐标/Y").Item(0).InnerText = e.Y.ToString();
                    }
                    else if(selectLable.Name == label2.Name)
                    {
                        xmlDoc.SelectNodes("/配置/名字/坐标/X").Item(0).InnerText = e.X.ToString();
                        xmlDoc.SelectNodes("/配置/名字/坐标/Y").Item(0).InnerText = e.Y.ToString();
                    }
                    xmlDoc.Save(filePath);
                }
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
           // this.label3.Text = "Mouse Click Down. " + mouseClickCouter.ToString() + " times!";
        }

        private void CreateInitLocationConfigureFile()
        {
            string filePath = @"LocationConfigrue.xml";
            if (!File.Exists(filePath)) //文件不存在
            {
                XmlDocument xml = new XmlDocument();
                XmlDeclaration decl = xml.CreateXmlDeclaration("1.0", "utf-8", null);
                xml.AppendChild(decl); //创建声明
                XmlElement rootEle = xml.CreateElement("配置");
                xml.AppendChild(rootEle); //创建根节点

                XmlElement childElemnet1 = xml.CreateElement("部门"); //创建位置根下Label1子节点
                XmlElement childElement1_location = xml.CreateElement("坐标");
                XmlElement childElemnet1_location_x = xml.CreateElement("X");
                XmlElement childElement1_location_y = xml.CreateElement("Y");
                childElemnet1_location_x.InnerText = 100.ToString();
                childElement1_location_y.InnerText = 100.ToString();
                XmlElement childElement1_color = xml.CreateElement("字体颜色");
                XmlElement childElement1_color_R = xml.CreateElement("R");
                XmlElement childElement1_color_G = xml.CreateElement("G");
                XmlElement childElement1_color_B = xml.CreateElement("B");
                childElement1_color_R.InnerText = 255.ToString();
                childElement1_color_G.InnerText = 255.ToString();
                childElement1_color_B.InnerText = 255.ToString();
                XmlElement childElement1_font = xml.CreateElement("字体");
                XmlElement childElement1_font_name = xml.CreateElement("字体名");
                XmlElement childElement1_font_size = xml.CreateElement("字体大小pt");
                childElement1_font_name.InnerText = "楷体";
                childElement1_font_size.InnerText = "36";
                rootEle.AppendChild(childElemnet1);
                childElemnet1.AppendChild(childElement1_location);
                childElement1_location.AppendChild(childElemnet1_location_x);
                childElement1_location.AppendChild(childElement1_location_y);
                childElemnet1.AppendChild(childElement1_color);
                childElement1_color.AppendChild(childElement1_color_R);
                childElement1_color.AppendChild(childElement1_color_G);
                childElement1_color.AppendChild(childElement1_color_B);
                childElemnet1.AppendChild(childElement1_font);
                childElement1_font.AppendChild(childElement1_font_name);
                childElement1_font.AppendChild(childElement1_font_size);

                XmlElement childElemnet2 = xml.CreateElement("名字"); //创建位置根下Label2子节点
                XmlElement childElement2_location = xml.CreateElement("坐标");
                XmlElement childElemnet2_location_x = xml.CreateElement("X");
                XmlElement childElement2_location_y = xml.CreateElement("Y");
                childElemnet2_location_x.InnerText = 100.ToString();
                childElement2_location_y.InnerText = 300.ToString();
                XmlElement childElement2_color = xml.CreateElement("字体颜色");
                XmlElement childElement2_color_R = xml.CreateElement("R");
                XmlElement childElement2_color_G = xml.CreateElement("G");
                XmlElement childElement2_color_B = xml.CreateElement("B");
                childElement2_color_R.InnerText = 255.ToString();
                childElement2_color_G.InnerText = 255.ToString();
                childElement2_color_B.InnerText = 255.ToString();
                XmlElement childElement2_font = xml.CreateElement("字体");
                XmlElement childElement2_font_name = xml.CreateElement("字体名");
                XmlElement childElement2_font_size = xml.CreateElement("字体大小pt");
                childElement2_font_name.InnerText = "楷体";
                childElement2_font_size.InnerText = "36";               
                rootEle.AppendChild(childElemnet2);
                childElemnet2.AppendChild(childElement2_location);
                childElement2_location.AppendChild(childElemnet2_location_x);
                childElement2_location.AppendChild(childElement2_location_y);
                childElemnet2.AppendChild(childElement2_color);
                childElement2_color.AppendChild(childElement2_color_R);
                childElement2_color.AppendChild(childElement2_color_G);
                childElement2_color.AppendChild(childElement2_color_B);
                childElemnet2.AppendChild(childElement2_font);
                childElement2_font.AppendChild(childElement2_font_name);
                childElement2_font.AppendChild(childElement2_font_size);

                XmlElement childElementImage = xml.CreateElement("背景图片");
                rootEle.AppendChild(childElementImage);
                XmlElement childElementImage_BKimage = xml.CreateElement("img");
                childElementImage_BKimage.InnerText = "image.jpg";
                childElementImage.AppendChild(childElementImage_BKimage);

                XmlElement childElementNameList = xml.CreateElement("人员名单");
                rootEle.AppendChild(childElementNameList);
                XmlElement childElementNameList_file = xml.CreateElement("file");
                childElementNameList_file.InnerText = "UserInfoLib.xml";
                childElementNameList.AppendChild(childElementNameList_file);
                xml.Save(filePath);//保存文件
            }
        }

        private void LoadLocationConfigureFile() //加载文件中的参数并进行一些初始化操作
        {
            string filePath = @"LocationConfigrue.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList xmlList = xmlDoc.ChildNodes; 
            string strX_label1,strY_label1,strFont_label1,strFontSize_label1, strColorR_label1, strColorG_label1, strColorB_label1; //加载LABEL1(部门)控件的参数
            strX_label1 = xmlDoc.SelectNodes("/配置/部门/坐标/X").Item(0).InnerText;
            strY_label1 = xmlDoc.SelectNodes("/配置/部门/坐标/Y").Item(0).InnerText;
            strFont_label1 = xmlDoc.SelectNodes("/配置/部门/字体/字体名").Item(0).InnerText;
            strFontSize_label1 = xmlDoc.SelectNodes("/配置/部门/字体/字体大小pt").Item(0).InnerText;
            strColorR_label1 = xmlDoc.SelectNodes("/配置/部门/字体颜色/R").Item(0).InnerText;
            strColorG_label1 = xmlDoc.SelectNodes("/配置/部门/字体颜色/G").Item(0).InnerText;
            strColorB_label1 = xmlDoc.SelectNodes("/配置/部门/字体颜色/B").Item(0).InnerText;
            this.label1.Location = new Point(Convert.ToInt32(strX_label1), Convert.ToInt32(strY_label1));
            this.label1.Font = new Font(strFont_label1, Convert.ToInt32(strFontSize_label1));
            Color c1 = new Color();
            c1 = Color.FromArgb(Convert.ToByte(strColorR_label1), Convert.ToByte(strColorG_label1), Convert.ToByte(strColorB_label1));
            this.label1.ForeColor = c1;


            string strX_label2, strY_label2, strFont_label2, strFontSize_label2, strColorR_label2, strColorG_label2, strColorB_label2;//加载LABEL2(名字)控件的参数
            strX_label2 = xmlDoc.SelectNodes("/配置/名字/坐标/X").Item(0).InnerText;
            strY_label2 = xmlDoc.SelectNodes("/配置/名字/坐标/Y").Item(0).InnerText;
            strFont_label2 = xmlDoc.SelectNodes("/配置/名字/字体/字体名").Item(0).InnerText;
            strFontSize_label2 = xmlDoc.SelectNodes("/配置/名字/字体/字体大小pt").Item(0).InnerText;
            this.label2.Location = new Point(Convert.ToInt32(strX_label2), Convert.ToInt32(strY_label2));
            this.label2.Font = new Font(strFont_label2, Convert.ToInt32(strFontSize_label2));
            strColorR_label2 = xmlDoc.SelectNodes("/配置/名字/字体颜色/R").Item(0).InnerText;
            strColorG_label2 = xmlDoc.SelectNodes("/配置/名字/字体颜色/G").Item(0).InnerText;
            strColorB_label2 = xmlDoc.SelectNodes("/配置/名字/字体颜色/B").Item(0).InnerText;
            Color c2 = new Color();
            c2 = Color.FromArgb(Convert.ToByte(strColorR_label2), Convert.ToByte(strColorG_label2), Convert.ToByte(strColorB_label2));
            this.label2.ForeColor = c2;

            strNameListFile = xmlDoc.SelectNodes("/配置/人员名单/file").Item(0).InnerText;//读取人员名单的文件名称
                        
            strImageBK = xmlDoc.SelectNodes("/配置/背景图片/img").Item(0).InnerText;  //加载背景图片参数     
            if (File.Exists(strImageBK)) //如果有背景图片，加载背景图片
            {
                try
                {
                    this.BackgroundImage = Image.FromFile(strImageBK);
                }

                catch (FileNotFoundException)
                {
                    MessageBox.Show("没有找到背景图片，请检查重试");
                }
            }

            if (File.Exists("LuckMan.txt"))  //文件存在
            {
                File.Delete("LuckMan.txt"); //删除原LUCKMAN.TXT文件并重新生成新的LuckMan.txt文件
                using (StreamWriter sw = File.CreateText("LuckMan.txt"))
                {
                    sw.WriteLine("README");
                    sw.WriteLine("该文本仅保存本次随机抽选结果！");
                    sw.WriteLine("================================");
                }
            }
        }

    }
}
