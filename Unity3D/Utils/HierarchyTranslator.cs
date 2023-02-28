using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using Cysharp.Text;
using System.Buffers;
using System;

namespace Poco.Utils
{
    public class HierarchyTranslator
    {
        //static StringBuilder sb = new StringBuilder();
        static Utf8ValueStringBuilder sb;

        static HierarchyTranslator()
        {
            Init();
        }

        //public static string HierarchyToStr(Dictionary<string, object> h)
        //{

        //    string result;

        //    //if(sb == null)
        //    //{
        //    //    //sb = new StringBuilder();
        //    //    sb = ZString.CreateUtf8StringBuilder();
        //    //}

        //    //sb.Clear();
        //    //sb = ZString.CreateUtf8StringBuilder(false);
        //    sb.Clear();
            
        //    DfsToStr(ref sb, h);
        //    int len = sb.Length;

        //    //var mem = sb.AsMemory();
        //    //result = mem.ToArray();


        //    //StringBuilder sb = new StringBuilder();


        //    //MemoryStream ms = new MemoryStream();
        //    //ms.Write("test");


        //    result = sb.ToString();

        //    return result;
        //}

        public static ReadOnlySpan<byte> HierarchyToStr(Dictionary<string, object> h)
        {
            
            string result;

            //if(sb == null)
            //{
            //    //sb = new StringBuilder();
            //    sb = ZString.CreateUtf8StringBuilder();
            //}

            //sb.Clear();
            //sb = ZString.CreateUtf8StringBuilder(false);
            sb.Clear();

            DfsToStr(ref sb, h);
            int len = sb.Length;

            var sp = sb.AsSpan();
            //byte[] result2 = mem.ToArray();
            //int len2 = mem.Length;

            //Debug.Log(len + "------" + len2);

            //StringBuilder sb = new StringBuilder();


            //MemoryStream ms = new MemoryStream();
            //ms.Write("test");
            //Utf8ValueStringBuilder sb2  = ZString.CreateUtf8StringBuilder(false);

            //sb2.Clear();
            //sb.Append(sb2);
            
            //result = sb.ToString();

            return sp;
        }



        private static bool DfsToStr(ref Utf8ValueStringBuilder sb, Dictionary<string, object> nodeDic)
        {
            if (nodeDic == null || nodeDic.Count == 0)
            {
                UnityEngine.Debug.LogError("nodeDic is null");
                return false;
            }

            sb.Append("{{");

            Dictionary<string, object> objPayload = nodeDic["payload"] as Dictionary<string, object>;



            int attrCnt = 0;

            if(!Config.Instance.blockedAttributes.Contains("name"))
            {
                object nameObj = null;
                objPayload.TryGetValue("name", out nameObj);
                if (nameObj != null)
                {
                    AppendQuotationStr(ref sb, nameObj.ToString());
                }else
                {
                    throw new Exception("Failed to get attr name");
                }
                attrCnt++;
            }


            if (!Config.Instance.blockedAttributes.Contains("visible"))
            {

                if (attrCnt > 0) sb.Append(",");

                object visibleObj = null;
                objPayload.TryGetValue("visible", out visibleObj);
                if (visibleObj != null)
                {
                    //AppendQuotationStr(ref sb, visibleObj.ToString());
                    AppendBoolStr(ref sb, (bool)visibleObj);
                }
                else
                {
                    throw new Exception("Failed to get attr visible");
                }

                attrCnt++;
            }


            if (!Config.Instance.blockedAttributes.Contains("pos"))
            {
                if (attrCnt > 0) sb.Append(",");

                object posObj = null;
                objPayload.TryGetValue("pos", out posObj);
                if (posObj != null)
                {
                    AppendFloatArr(ref sb, (float[])posObj);
                }else
                {
                    throw new Exception("Failed to get attr pos");
                }
                attrCnt++;
            }


            if (!Config.Instance.blockedAttributes.Contains("size"))
            {

                if (attrCnt > 0) sb.Append(",");

                object sizeObj = null;
                objPayload.TryGetValue("size", out sizeObj);
                if (sizeObj != null)
                {
                    AppendFloatArr(ref sb, (float[])sizeObj);
                }else
                {
                    throw new Exception("Failed to get attr size");
                }
                attrCnt++;
            }


            if (!Config.Instance.blockedAttributes.Contains("anchorPoint"))
            {
                if (attrCnt > 0) sb.Append(",");

                object anchorPointObj = null;
                objPayload.TryGetValue("anchorPoint", out anchorPointObj);
                if (anchorPointObj != null)
                {

                    AppendFloatArr(ref sb, (float[])anchorPointObj);
                }else
                {
                    throw new Exception("Failed to get attr anchorPoint");
                }
                attrCnt++;
            }


            if (!Config.Instance.blockedAttributes.Contains("zOrders"))
            {
                if (attrCnt > 0) sb.Append(",");


                object zOrdersObj = null;
                objPayload.TryGetValue("zOrders", out zOrdersObj);
                if (zOrdersObj != null)
                {

                    AppendzOrdersArr(ref sb, (Dictionary<string, float>)zOrdersObj);
                }else
                {
                    throw new Exception("Failed to get attr zOrders");
                }

                attrCnt++;
            }

            if (!Config.Instance.blockedAttributes.Contains("text"))
            {
                if (attrCnt > 0) sb.Append(",");

                object textObj = null;
                objPayload.TryGetValue("text", out textObj);
                if (textObj != null)
                {
                    AppendQuotationStr(ref sb, textObj.ToString());
                }
                else
                {
                    sb.Append("\\\"\\\"");
                }
                attrCnt++;

            }


            if (!Config.Instance.blockedAttributes.Contains("texture"))
            {
                if (attrCnt > 0) sb.Append(",");

                object textureObj = null;
                objPayload.TryGetValue("texture", out textureObj);
                if (textureObj != null)
                {

                    AppendQuotationStr(ref sb, textureObj.ToString());
                    attrCnt++;
                }
                else
                {
                    sb.Append("\\\"\\\"");
                    attrCnt++;
                }
            }

            if (!Config.Instance.blockedAttributes.Contains("_instanceId"))
            {
                if (attrCnt > 0) sb.Append(",");

                object iIdObj = null;
                objPayload.TryGetValue("_instanceId", out iIdObj);
                if (iIdObj != null)
                {
                    sb.Append(iIdObj.ToString());
                }
                else
                {
                    //do nothing
                }
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

                        bool got_subnode = DfsToStr(ref sb, (Dictionary<string, object>)o);
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

        public static void Init()
        {
            sb = ZString.CreateUtf8StringBuilder(false);

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



        private static void AppendFloatArr(ref Utf8ValueStringBuilder m_sb, float[] arr)
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
                //LogUtil.ULogDev(arr[0] + ", " +arr[1]);
                m_sb.Append('[');
                m_sb.Append(arr[0].ToString());
                m_sb.Append(',');
                m_sb.Append(arr[1].ToString());
                m_sb.Append(']');
            }



        }
        static string Str_True = "\\\"True\\\"";
        static string Str_False = "\\\"False\\\"";
        private static void AppendBoolStr(ref Utf8ValueStringBuilder m_sb, bool v)
        {
            if (v)
            {
                m_sb.Append(Str_True);
            }
            else
            {
                m_sb.Append(Str_False);

            }
        }


        //private static void AppendintStr(ref Utf8ValueStringBuilder m_sb, string str)
        //{
        //    m_sb.Append(str);
        //}

        private static void AppendQuotationStr(ref Utf8ValueStringBuilder m_sb, string str)
        {
            m_sb.Append("\\\"");
            m_sb.Append(str);
            m_sb.Append("\\\"");
        }

        private static void AppendzOrdersArr(ref Utf8ValueStringBuilder m_sb, Dictionary<string, float> zOrdersDic)
        {
            string str = null;
            bool flag = TryArrToStr(zOrdersDic["global"], zOrdersDic["local"], out str);
            if (flag)
            {
                m_sb.Append(str);
            }else
            {
                //LogUtil.ULogDev(zOrdersDic["global"] + ", " + zOrdersDic["local"]);

                m_sb.Append('[');
                m_sb.Append(zOrdersDic["global"].ToString());
                m_sb.Append(',');
                m_sb.Append(zOrdersDic["local"].ToString());
                m_sb.Append(']');
            }

        }

    }

}

