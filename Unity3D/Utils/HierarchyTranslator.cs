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
            sb.Clear();
            DfsToStr(sb, h);
            return sb.ToString();
        }



        static string node_start = "{{";

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
                sb.Append(",[");
                int childCnt = 0;
                for(int i=0;i< chldList.Count; i++)
                {
                    object o = chldList[i];
                    
                    bool got_subnode = DfsToStr(sb, (Dictionary<string, object>)o);
                    if(got_subnode)
                    {
                        childCnt++;
                        if(i!=chldList.Count-1) sb.Append(",");
                    }
                }

                if(childCnt > 0)
                {
                    sb.Append("]");
                }
                else
                {
                    sb.Remove(sb.Length - 2, 2);
                }
            }

            sb.Append("}");
            return true;

        }


        private static StringBuilder AppendFloatArr(StringBuilder m_sb, float[] arr)
        {

            if (arr==null||arr.Length !=2)
            {
                return null;
            }
            m_sb.AppendFormat("[{0},{1}]", arr[0], arr[1]);


            return m_sb;
        }

        private static StringBuilder AppendQuotationStr(StringBuilder m_sb, string str)
        {
            return m_sb.AppendFormat("\"{0}\"", str);
        }

    }

}

