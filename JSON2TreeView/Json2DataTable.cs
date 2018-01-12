using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Data;

namespace JSON2TreeView
{
    public class Json2DataTable
    {
        public static DataTable Get(string name, string json)
        {
            TreeView treeView = new TreeView();
            Json2TreeView(name, json, treeView);

            List<string> JColumns = new List<string>();
            DefineColumns(0, name, treeView.Nodes, JColumns);

            JColumns.Sort();
            DataTable JsonTB = new DataTable();
            foreach (string col in JColumns) JsonTB.Columns.Add(new DataColumn(col, typeof(string)));

            InsertDataRow(0, name, treeView.Nodes, JsonTB, new string[JsonTB.Columns.Count]);

            //JsonTB = JsonTB.AsEnumerable().GroupBy(r => new { Col1 = r["Col1"], Col2 = r["Col2"] }).Select(g => g.OrderBy(r => r["01"]).First()).CopyToDataTable();
            //JsonTB = JsonTB.AsEnumerable().GroupBy(r => JColumns).Select(g => g.OrderBy(r => r["04:type"]).First()).CopyToDataTable();

            return JsonTB;
        }

        public static DataTable Get(string name, string json, TreeView treeView)
        {
            Json2TreeView(name, json, treeView);

            List<string> JColumns = new List<string>();
            DefineColumns(0, name, treeView.Nodes, JColumns);

            JColumns.Sort();
            DataTable JsonTB = new DataTable();
            foreach (string col in JColumns) JsonTB.Columns.Add(new DataColumn(col, typeof(string)));

            InsertDataRow(0, name, treeView.Nodes, JsonTB, new string[JsonTB.Columns.Count]);

            //JsonTB = JsonTB.AsEnumerable().GroupBy(r => new { Col1 = r["Col1"], Col2 = r["Col2"] }).Select(g => g.OrderBy(r => r["01"]).First()).CopyToDataTable();
            //JsonTB = JsonTB.AsEnumerable().GroupBy(r => JColumns).Select(g => g.OrderBy(r => r["04:type"]).First()).CopyToDataTable();

            return JsonTB;
        }

        private static void Json2TreeView(string name, string json, System.Windows.Forms.TreeView treeView)
        {
            if (json.Equals(null) || json.Equals(string.Empty)) { return; }
            var @object = JObject.Parse(json);
            AddObjectNodes(@object, name, treeView.Nodes);
        }

        private static void AddObjectNodes(JObject @object, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);
            foreach (var property in @object.Properties())
            {
                AddTokenNodes(property.Value, property.Name, node.Nodes);
            }
        }

        private static void AddArrayNodes(JArray array, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);

            for (var i = 0; i < array.Count; i++)
            {
                AddTokenNodes(array[i], string.Format("[{0}]", i), node.Nodes);
            }
        }

        private static void AddTokenNodes(JToken token, string name, TreeNodeCollection parent)
        {
            if (token is JValue)
            {
                parent.Add(new TreeNode(string.Format("{0}: {1}", name, ((JValue)token).Value)));
            }
            else if (token is JArray)
            {
                AddArrayNodes((JArray)token, name, parent);
            }
            else if (token is JObject)
            {
                AddObjectNodes((JObject)token, name, parent);
            }
        }

        private static void DefineColumns(int rank, string name, TreeNodeCollection nodes, List<string> JColumns)
        {
            if (nodes.Count > 0)
            {
                string colName;
                bool chk = false;
                rank++;
                for (int i = 0; i < nodes.Count; i++)
                {
                    chk = false;
                    if (nodes[i].Text.IndexOf(':') == -1)
                    {
                        colName = rank.ToString().PadLeft(2, '0');
                        for (int j = 0; j < JColumns.Count; j++)
                        {
                            if (JColumns[j].Equals(colName))
                            {
                                chk = true;
                                break;
                            }
                        }
                        if (!chk) JColumns.Add(colName);
                    }
                    else
                    {
                        colName = rank.ToString().PadLeft(2, '0') + ":" + nodes[i].Text.Split(':')[0];
                        for (int j = 0; j < JColumns.Count; j++)
                        {
                            if (JColumns[j].Equals(colName))
                            {
                                chk = true;
                                break;
                            }
                        }
                        if (!chk) JColumns.Add(colName);
                    }
                    DefineColumns(rank, nodes[i].Text, nodes[i].Nodes, JColumns);
                }
            }
        }

        private static void InsertDataRow(int rank, string value, TreeNodeCollection nodes, DataTable JsonTB, string[] jData)
        {
            if (nodes.Count > 0)
            {
                string colName;
                rank++;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Text.IndexOf(':') == -1)
                    {
                        colName = rank.ToString().PadLeft(2, '0');
                        for (int j = 0; j < JsonTB.Columns.Count; j++)
                        {
                            if (JsonTB.Columns[j].ToString().Equals(colName))
                            {
                                jData[j] = nodes[i].Text;
                                break;
                            }
                        }
                    }
                    else
                    {
                        colName = rank.ToString().PadLeft(2, '0') + ":" + nodes[i].Text.Split(':')[0];
                        for (int j = 0; j < JsonTB.Columns.Count; j++)
                        {
                            if (JsonTB.Columns[j].ToString().Equals(colName))
                            {
                                for (int k = 0; k < JsonTB.Columns.Count; k++)
                                {
                                    if (int.Parse(JsonTB.Columns[k].ToString().Split(':')[0]) >= rank) jData[k] = string.Empty;
                                }
                                jData[j] = nodes[i].Text.Split(':')[1];
                                JsonTB.Rows.Add(jData);
                                break;
                            }
                        }
                    }
                    if (nodes[i].Nodes.Count > 0) InsertDataRow(rank, nodes[i].Text, nodes[i].Nodes, JsonTB, jData);
                }
            }
        }
    }
}
