using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SOA.Service;
using log4net;
using QMS.Service;
using System.Threading;
using System.Net;
using System.Drawing;
using System.Data;

namespace QMS.ExportSisPic
{
    
    public class ExportSisPic : MarshalByRefObject
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static bool terminate = false;
        private static Thread thread;
        private bool bIsRunFullCheck = false; //是否从普系表全部重新效验数据报警
        public ExportSisPic()
        {
            terminate = false;
            thread = new Thread(new ThreadStart(Run));
            //STA：表示Thread将被创建并进入一个单线程单元，我猜想STA应该是Single Thread Apartment的首字母简拼；
            //MTA：表示Thread将被创建并进入一个多线程单元，
            //Unknown，表示没有设置线程的单元状态
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Priority = ThreadPriority.Normal;
            thread.Start();
            logger.InfoFormat("批量导出表检图片程序启动");
        }
        SurfaceQueryArgs args = new SurfaceQueryArgs()
        {
            MongoConn = "mongodb://172.27.36.9:40000",
           MongodbName = "hrmdb",
            MongotbName = "surface1580",
            ProcessNo = "HSM2"
        };
             
        private void Run()
        {
            bool flag = true;
            while(flag)
            {
                List<List<string>> detailList = new List<List<string>>();
                List<string> textList = new List<string>();
                textList =  Read("D://test.txt");
                detailList = listDisparchar(textList);
               
                List<IList<CoilSurfaceDefect>> list = new List<IList<CoilSurfaceDefect>>();
                for (int i = 0; i < detailList.Count; i++)
                {
                    list.Add(getCoilsList(args,detailList[i][0]));
                }


                WebClient web = new WebClient();
                string name = "";
                for (int i = 0; i < list.Count; i++)
                {
                   name = drawAndDownloadImage(list[i][0].Deffects, detailList[i][1], web);
                    if (name != null && name != "")
                    {
                       
                        logger.Info("----------------输出excel----------------");
                        DataTable dt = List2DataTable.ListToDataTable(list[i][0].Deffects);
                        ExportExcel.dataTableToCsv(dt, string.Format(@"d:\data\{0}.xls", name));
                    }
                }

                flag = false;
                logger.Info("！！！！！！！！！！！全部操作已经结束！！！！！！！！！！！！！！！");
                
            }
            
        }


        /**
         * 读取指定文件下的txt文件
         **/
        private List<string> Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            List<string> textList = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                textList.Add(line);
                logger.Info("-------------读取文件完成-----------------------");
            }
            return textList;
        }

        /**
         * 对读取到的字符串list做处理
         **/
        private List<List<string>> listDisparchar(List<string> list){
            List<List<string>> detaiList = new List<List<string>>();
            List<string> list2 = null;
            string[] stringArr;
            for (int i = 0; i < list.Count; i++)
            {
                list2 = new List<string>();
                stringArr = list[i].Split('/');
                for (int j = 0; j < stringArr.Length; j++)
                {
                    list2.Add(stringArr[j]);
                }
                detaiList.Add(list2);
                logger.Info("----------------文本数据处理完毕-----------------");
            }
            return detaiList;
        }

        /**
         * 查询所有的表检信息
         **/
        private IList<CoilSurfaceDefect> getCoilsList(SurfaceQueryArgs args,string matID)
        {
            args.MongodbName = "hrmdb";
            args.MongotbName = "surface1580";
            args.ProcessNo = "HSM2";
            args.IsExCoil = true;
            args.MatId = matID;
            IInspection service = ServiceContainer.GetService<IInspection>();
            IList<CoilSurfaceDefect> list = service.GetDefectByMatId(args);
            logger.Info("----------------表检数据已经查询完毕----------------");
            return list;
        }

        /***
         * 绘制图片并保存
         **/
        private string drawAndDownloadImage(IList<CoilDeffect> coilDeffects, string defectClassID, WebClient web)
        {
            string imagename2 = "";
                logger.Info("----------------开始处理图片-----------------");
            for (int i = 0, len = coilDeffects.Count; i < len; i++)
            {
                try
                {
                    string url = coilDeffects[i].DEFECTIMAGEURL;
                    if (args.ProcessNo == "HSM1")
                    {
                        if (!url.Contains("hsm1"))
                        {
                            url = url.Replace("hrmdb", "hrmdb/hsm1");
                        }
                    }
                    if (defectClassID != null && defectClassID != "")
                    {
                        if (defectClassID == (coilDeffects[i].DEFECTCLASSID).ToString())
                        {
                            string imagename = url.Substring(url.LastIndexOf("/") + 1);
                            string filename = string.Format("Data\\{0}\\{1}\\{2}", imagename.Substring(0, 9), coilDeffects[i].DEFECTCLASSID, imagename);

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
                            logger.Info(string.Format("----------------图片{0}下载完毕--------------", imagename));
                            stream.Flush();
                            stream.Close();
                        }
                    }
                    else if (defectClassID == "")
                    {
                        string imagename = url.Substring(url.LastIndexOf("/") + 1);
                        string filename = string.Format("Data\\{0}\\{1}\\{2}", imagename.Substring(0, 9), coilDeffects[i].DEFECTCLASSID, imagename);

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
                        logger.Info(string.Format("----------------图片{0}下载完毕--------------", imagename));
                        stream.Flush();
                        stream.Close();
                    }
                } catch (Exception ex)
                {
                    logger.Info(string.Format("图片下载失败"));
                }
                string url1 = coilDeffects[i].DEFECTIMAGEURL;
                if (url1!=null&&url1!="") {
                    imagename2 = url1.Substring(url1.LastIndexOf("/") + 1);
                    imagename2 = imagename2.Substring(0,10);
                }
            }
            return imagename2;
        }

        
    }
}
