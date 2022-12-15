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

            Init();
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


            object nameObj = null;

            int attrCnt = 0;

            objPayload.TryGetValue("name", out nameObj);
            if(nameObj!=null)
            {
                AppendQuotationStr(sb, nameObj.ToString());
                attrCnt++;
            }

            object visibleObj = null;
            objPayload.TryGetValue("visible", out visibleObj);
            if (visibleObj != null)
            {
                if (attrCnt > 0) sb.Append(",");

                AppendQuotationStr(sb, visibleObj.ToString());
                attrCnt++;
            }

            object posObj = null;
            objPayload.TryGetValue("pos", out posObj);
            if (posObj != null)
            {
                if (attrCnt > 0) sb.Append(",");

                AppendFloatArr(sb, (float[])posObj);
                attrCnt++;
            }

            object sizeObj = null;
            objPayload.TryGetValue("size", out sizeObj);
            if (sizeObj != null)
            {
                if (attrCnt > 0) sb.Append(",");

                AppendFloatArr(sb, (float[])sizeObj);
                attrCnt++;
            }

            object anchorPointObj = null;
            objPayload.TryGetValue("anchorPoint", out anchorPointObj);
            if (anchorPointObj != null)
            {
                if (attrCnt > 0) sb.Append(",");

                AppendFloatArr(sb, (float[])anchorPointObj);
                attrCnt++;
            }

            object zOrdersObj = null;
            objPayload.TryGetValue("zOrders", out zOrdersObj);
            if (zOrdersObj != null)
            {
                if (attrCnt > 0) sb.Append(",");

                AppendzOrdersArr(sb, (Dictionary<string, float>)zOrdersObj);
                attrCnt++;
            }
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



        // -1 -0.5 0 0.5 1 1.5 2 2.5 3 3.5 
        // 0    1  2  3  4  5  6  7  8  9 

        private static void Init()
        {
            ArrToStrDic = new string[10, 10];
            ArrToStrDic[0, 0] = "[-1,-1]";
            ArrToStrDic[1, 1] = "[-0.5,-0.5]";
            ArrToStrDic[2, 2] = "[0,0]";
            ArrToStrDic[3, 3] = "[0.5,0.5]";
            ArrToStrDic[4, 4] = "[1,1]";
            ArrToStrDic[5, 5] = "[1.5,1.5]";
            ArrToStrDic[6, 6] = "[2,2]";
            ArrToStrDic[0, 0] = "[-1,-1]";
            for(int i=0; i<5; i++)
            {
                for(int j=0; j<5; j++)
                {
                    ArrToStrDic[i, j] = string.Format("[{0},{1}]", IndexToV(i), IndexToV(j));
                }
            }

        }

        static string[,] ArrToStrDic = null;
        private static int VToIndex(float v)
        {
            if (v == -1) return 0;
            if (v == -0.5f) return 1;
            if (v == 0) return 2;
            if (v == 0.5f) return 3;
            if (v == 1) return 4;
            if (v == 1.5f) return 5;
            if (v == 2) return 6;
            if (v == 2.5f) return 7;
            if (v == 3) return 8;
            if (v == 3.5f) return 9;

            return -1;
        }
        private static float IndexToV(int ind)
        {
            if (ind == 0) return -1;
            if (ind == 1) return -0.5f;
            if (ind == 2) return 0;
            if (ind == 3) return 0.5f;
            if (ind == 4) return 1;
            if (ind == 5) return 1.5f;
            if (ind == 6) return 2;
            if (ind == 7) return 2.5f;
            if (ind == 8) return 3;
            if (ind == 9) return 3.5f;

            return -1;
        }

        private static bool TryArrToStr(float x, float y, out string Str)
        {
            Str = "";
            int ind_x = VToIndex(x);
            int ind_y = VToIndex(y);
            if(ind_x != -1 && ind_y!=-1)
            {
                Str = ArrToStrDic[ind_x, ind_y];
                return true;
            }else
            {
                return false;
            }

        }



        private static void AppendFloatArr(StringBuilder m_sb, float[] arr)
        {

            if (arr==null||arr.Length !=2)
            {
                return;
            }

            string str = null;
            bool flag = TryArrToStr(arr[0], arr[1], out str);
            if(flag)
            {
                m_sb.Append(str);
            }
            else
            {
                m_sb.Append('[');
                m_sb.Append(arr[0]);
                m_sb.Append(',');
                m_sb.Append(arr[1]);
                m_sb.Append(']');
            }



        }

        private static void AppendQuotationStr(StringBuilder m_sb, string str)
        {
            m_sb.Append('\"');
            m_sb.Append(str);
            m_sb.Append('\"');
        }

        private static void AppendzOrdersArr(StringBuilder m_sb, Dictionary<string, float> zOrdersDic)
        {
            string str = null;
            bool flag = TryArrToStr(zOrdersDic["global"], zOrdersDic["local"], out str);
            if (flag)
            {
                m_sb.Append(str);
            }else
            {

                m_sb.Append('[');
                m_sb.Append(zOrdersDic["global"]);
                m_sb.Append(',');
                m_sb.Append(zOrdersDic["local"]);
                m_sb.Append(']');
            }

        }

    }

}

