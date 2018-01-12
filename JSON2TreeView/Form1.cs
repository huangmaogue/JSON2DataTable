using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSON2TreeView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string file = @"..\JSON.TXT";
            //string file = @"z:\setting.cfg";
            string file = @"z:\RePrint.txt";
            if (File.Exists(file))
            {
                StreamReader stream = new StreamReader(file, Encoding.Default);
                string json = stream.ReadToEnd();
                stream.Close();

                DataTable JsonTB = Json2DataTable.Get("JSON1", json, treeView);
                dataGridView.DataSource = JsonTB;
            }
        }

        private void ParseString(int width)
        {
        }

        public bool IsChina(string CString)
        {
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = false;
                }
                else
                {
                    BoolValue = true;
                }
                MessageBox.Show(CString.Substring(i, 1) + " : " + BoolValue.ToString());
            }
            return BoolValue;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(StringWrap("Sid Meier's Starships加文明帝國：超越地球 同捆組合", 25, 3));
        }



        /// <summary>
        /// 設定字串從特定長度加入換行字元，長度以半形字元計算
        /// maxLineLen : 換行長度，fullCharLen : 全形字換算成幾個半形字
        /// </summary>
        public static string StringWrap(string str, int maxLineLen, int fullCharLen)
        {
            string str2 = string.Empty;
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(str.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128))) count++;
                else count = count + fullCharLen;

                str2 = str2 + str.Substring(i, 1);
                if (count >= maxLineLen)
                {
                    count = 0;
                    str2 = str2 + Environment.NewLine;
                }
            }
            return str2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string file = @"z:\RePrint.txt";
            if (File.Exists(file))
            {
                StreamReader stream = new StreamReader(file, Encoding.Default);
                string json = stream.ReadToEnd();
                stream.Close();

                JObject obj = JObject.Parse(json);
                //Console.WriteLine(obj["mycard_ticket"]["Result"]["Cards"]["ProductNote"].ToString());
                //Console.WriteLine(obj["mycard_ticket"]["Result"]["Cards"]["OrderInfo"].ToString());
                Console.WriteLine(obj["Result"]["Cards"]["ProductNote"].ToString());
                Console.WriteLine(obj["Result"]["Cards"]["OrderInfo"].ToString());
                Console.WriteLine(((JArray)obj["mycard_ticket"]["Result"]["Cards"]["ProductNote"]).Count);
                Console.WriteLine(((JArray)obj["mycard_ticket"]["Result"]["Cards"]["OrderInfo"]).Count);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string file = @"z:\setting.cfg";
            if (File.Exists(file))
            {
                StreamReader stream = new StreamReader(file, Encoding.UTF8);
                string json = stream.ReadToEnd();
                stream.Close();

                JObject ez2351 = JObject.Parse(json);
                JArray jar = (JArray)ez2351["Server"]["Mycard"]["category"];
                jar = (JArray)jar[0]["sub_category"];
                jar = (JArray)jar[1]["products"];

                JObject myCardRequest = (JObject)ez2351["Server"]["Key_3"]["Service"]["supplier"][1]["production"][0]["action"][0]["option"][0]["method"][0];
                JObject myCardReprint = (JObject)ez2351["Server"]["Key_3"]["Service"]["supplier"][1]["production"][0]["action"][0]["option"][1]["method"][0];
                Console.WriteLine(myCardRequest.ToString());
                Console.WriteLine(myCardReprint.ToString());

                JObject req = JObject.FromObject(new
                {
                    mycard_ticket = JObject.FromObject(new
                    {
                        ServiceCode = myCardRequest["code"],
                        EDCID = ez2351["Server"]["Merchant"]["EDCID"],
                        EDCMac = ez2351["Server"]["Merchant"]["EDCMac"],
                        AppName = "WeCanPay_Payment",
                        Quantity = 1,
                        ItemNumber = "photos",
                        Account = "photos",
                    })
                });
                Console.WriteLine(req.ToString());
                Console.WriteLine("TEST : " + Convert.ToString(req["mycard_ticket"]["xAppName"]).Replace("\\\"",""));
            }
            //System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("\r\n");
            //System.Text.RegularExpressions.MatchCollection mc = r.Matches("abcd" + Environment.NewLine + "cd");
            //int linesCount = mc.Count;
            //Console.WriteLine(linesCount.ToString());
        }

        //把DataTable轉成JSON字串
        public void DataTable2JSON()
        {
            DataTable article = new DataTable();
            article.Columns.Add(new DataColumn("COL1", typeof(string)));
            article.Columns.Add(new DataColumn("COL2", typeof(string)));
            article.Columns.Add(new DataColumn("COL3", typeof(string)));
            article.Columns.Add(new DataColumn("COL4", typeof(string)));

            article.Rows.Add(new string[] { "DATA1", "DATA2", "DATA3", "DATA4" });
            article.Rows.Add(new string[] { "DATA11", "DATA22", "DATA33", "DATA44" });
            article.Rows.Add(new string[] { "DATA111", "DATA222", "DATA333", "DATA444" });

            DataTable label = new DataTable();
            label.Columns.Add(new DataColumn("LABELID", typeof(string)));
            label.Columns.Add(new DataColumn("PANELID", typeof(string)));
            label.Columns.Add(new DataColumn("TEMPLATE", typeof(string)));

            label.Rows.Add(new string[] { "55", "66", "abcd" });
            label.Rows.Add(new string[] { "77", "88", "ab--" });

            ////將DataTable轉成JSON字串
            //string jstr = JsonConvert.SerializeObject(tb, Formatting.Indented);

            //JObject _Data = JObject.FromObject(new
            //{

            //});

            ////JSON字串顯示在畫面上
            //Console.WriteLine(jstr);
            string[] filter = new string[] { "UUID", "LAST_UPDATE", "EXT0", "EXT1", "EXT2" };
            article = article.DefaultView.ToTable(true, article.Columns.Cast<DataColumn>().Select(s => s.ColumnName).Where(w => !filter.Contains(w)).ToArray());

            var dic = new Dictionary<string, string>();
            var dic2 = new Dictionary<string, string>();
            for (int i = 0; i < article.Rows.Count; i++)
            {
                JArray jary = new JArray();
                for (int j = 0; j < label.Rows.Count; j++)
                {
                    dic2.Clear();
                    foreach (DataColumn col2 in label.Columns) dic2.Add(col2.ColumnName, label.Rows[j][col2.ColumnName].ToString());
                    string str2 = JsonConvert.SerializeObject(dic2);
                    Console.WriteLine(str2);
                    jary.Add(JsonConvert.DeserializeObject(str2));
                    //jobj2.AddAfterSelf(str2);
                    Console.WriteLine(jary);
                }
                dic.Clear();
                foreach (DataColumn col in article.Columns) dic.Add(col.ColumnName, article.Rows[i][col.ColumnName].ToString());
                string str1 = JsonConvert.SerializeObject(dic);
                JObject jobj = JObject.FromObject(new
                {
                    Article = JObject.Parse(str1),
                    Label = new JArray()
                });
                string test = JsonConvert.SerializeObject(jobj);
                string test2 = JsonConvert.SerializeObject(Convert.ToString(jobj));
                Console.WriteLine(test);
            }
            //Console.WriteLine(Convert.ToString(jobj));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string> { "abc", "def", "ghi" };
            Console.WriteLine(JsonConvert.SerializeObject(list));

            //DataTable2JSON();
        }
    }
}
