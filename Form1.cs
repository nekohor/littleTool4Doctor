using QMS.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace QMS.ExportSisPic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public SurfaceQueryArgs Args { get; set; }



        /*
       * 下载图片同时对图片进行标注
       * Author:温广杰
       * Date:2019-07-24
       */
        private void drawAndDownloadImage(IList<CoilDeffect> coilDeffects, string baseDir, WebClient web)
        {
            try
            {
                for (int i = 0, len = coilDeffects.Count; i < len; i++)
                {
                    string url = coilDeffects[i].DEFECTIMAGEURL;
                    if (Args.ProcessNo == "HSM1")
                    {
                        if (!url.Contains("hsm1"))
                        {
                            url = url.Replace("hrmdb", "hrmdb/hsm1");
                        }
                    }

                    string imagename = url.Substring(url.LastIndexOf("/") + 1);
                    string filename = string.Format("{0}\\{1}\\{2}", baseDir, coilDeffects[i].DEFECTCLASSID, imagename);

                    Stream stream = web.OpenRead(url);
                    Bitmap image = new Bitmap(stream);

                    //给图片做标记
                    int x0 = (int)coilDeffects[i].ROIX0;
                    int y0 = (int)coilDeffects[i].ROIY0;
                    int x1 = (int)coilDeffects[i].ROIX1;
                    int y1 = (int)coilDeffects[i].ROIY1;

                    if (x1 > x0)
                    {
                        Graphics g = Graphics.FromImage(image);
                        Pen redPen = new Pen(Color.Red, 1);
                        g.DrawRectangle(redPen, x0, y0, x1 - x0, y1 - y0);
                        g.DrawImage(image, new Point(0, 0));
                    }

                    //下载图片开始
                    string dirPath = filename.Substring(0, filename.LastIndexOf("\\"));
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    image.Save(filename);
                    stream.Flush();
                    stream.Close();

                    //更新控件数据
                 
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void textBox4Coils_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
