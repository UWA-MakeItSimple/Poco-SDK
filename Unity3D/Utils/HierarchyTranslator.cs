using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
namespace Poco.Utils
{
    public class HierarchyTranslator
    {
        static StringBuilder sb = new StringBuilder();

        

        public static string HierarchyToStr(Dictionary<string, object> h)
        {
            string result;

            if(sb == null)
            {
                sb = new StringBuilder();
            }

            sb.Clear();
            //Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();
            //StringBuilder sb = new StringBuilder();
            {
                DfsToStr(sb, h);
                
            }
            result = sb.ToString();
            return result;
        }




        private static bool DfsToStr(StringBuilder sb, Dictionary<string, object> nodeDic)
        {
            if (nodeDic == null || nodeDic.Count == 0)
            {
                return false;
            }

            sb.Append("{{");

            Dictionary<string, object> objPayload = nodeDic["payload"] as Dictionary<string, object>;

            AppendQuotationStr(sb, objPayload["name"].ToString());
            sb.Append(",");

            AppendQuotationStr(sb, objPayload["visible"].ToString());
            sb.Append(",");

            AppendFloatArr(sb, (float[])objPayload["pos"]);
            sb.Append(",");

            AppendFloatArr(sb, (float[])objPayload["size"]);
            sb.Append(",");

            AppendFloatArr(sb, (float[])objPayload["anchorPoint"]);
            sb.Append("}");


            //left_sb.AppendFormat("{0},{1},{2},{3},{4}}", name, visible, pos, size, anchorPoint);

            //left_sb.Append(name_sb);
            //left_sb.Append(",");
            //left_sb.Append(visible_sb);
            //left_sb.Append(",");
            //left_sb.Append(pos_sb);
            //left_sb.Append(",");
            //left_sb.Append(size_sb);
            //left_sb.Append(",");
            //left_sb.Append(anchorPoint_sb);
            

            object childListObj = null;

            nodeDic.TryGetValue("children", out childListObj);

            List<object> chldList = childListObj as List<object>;

            if (chldList != null && chldList.Count != 0)
            {
                if (chldList[0] != null)
                {
                    sb.Append(",[");
                    int childCnt = 0;
                    for (int i = 0; i < chldList.Count; i++)
                    {
                        object o = chldList[i];

                        bool got_subnode = DfsToStr(sb, (Dictionary<string, object>)o);
                        if (got_subnode)
                        {
                            childCnt++;
                            if (i != chldList.Count - 1) sb.Append(",");
                        }
                    }

                    sb.Append("]");
                }
                
            }

            sb.Append("}");
            return true;

        }


        private static void AppendFloatArr(StringBuilder m_sb, float[] arr)
        {

            if (arr==null||arr.Length !=2)
            {
                return;
            }
            m_sb.AppendFormat("[{0},{1}]", arr[0], arr[1]);


        }

        private static void AppendQuotationStr(StringBuilder m_sb, string str)
        {
            m_sb.AppendFormat("\"{0}\"", str);
        }

    }

}

