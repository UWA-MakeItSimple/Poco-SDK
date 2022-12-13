using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
namespace Poco.Utils
{
    public class HierarchyTranslator
    {
        static StringBuilder sb;
        public static string HierarchyToStr(Dictionary<string, object> h)
        {
            sb = new StringBuilder();
            sb = DfsToStr(h);
            return sb.ToString();
        }



        static string node_start = "{{";

        private static StringBuilder DfsToStr(Dictionary<string, object> nodeDic)
        {
            if (nodeDic == null || nodeDic.Count == 0)
            {
                return null;
            }

            StringBuilder left_sb = new StringBuilder(node_start);

            Dictionary<string, object> objPayload = nodeDic["payload"] as Dictionary<string, object>;

            AppendQuotationStr(left_sb, objPayload["name"].ToString());
            left_sb.Append(",");

            AppendQuotationStr(left_sb, objPayload["visible"].ToString());
            left_sb.Append(",");

            AppendFloatArr(left_sb, (float[])objPayload["pos"]);
            left_sb.Append(",");

            AppendFloatArr(left_sb, (float[])objPayload["size"]);
            left_sb.Append(",");

            AppendFloatArr(left_sb, (float[])objPayload["anchorPoint"]);
            left_sb.Append("}");


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
                StringBuilder childLeft_sb = new StringBuilder(",[");
                int childCnt = 0;
                for(int i=0;i< chldList.Count; i++)
                {
                    object o = chldList[i];

                    StringBuilder childStr_sb = DfsToStr((Dictionary<string, object>)o);
                    if(childStr_sb != null && childStr_sb.Length!=0)
                    {
                        childCnt++;
                        if (i != 0) childLeft_sb.Append(",");
                        childLeft_sb.Append(childStr_sb);
                    }
                }

                if(childCnt > 0)
                {
                    left_sb.Append(childLeft_sb);
                    left_sb.Append("]");
                }
            }

            left_sb.Append("}");
            return left_sb;

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

