using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poco.Utils
{


    public class Deserialize
    {


        const char left1 = '{';
        const char right1 = '}';
        const char left2 = '[';
        const char right2 = ']';

        static Stack<char> stk1 = new Stack<char>();
        static Stack<char> stk2 = new Stack<char>();
        /// <summary>
        /// start 为所要匹配的括号的下一个字符
        /// </summary>
        /// <returns>匹配的右括号的下标</returns>
        static int GetMatchingBracket(char leftBracket, string str, int start, int end)
        {
            char rightBracket;
            switch (leftBracket)
            {
                case '{': rightBracket = '}';
                    break;
                case '[': rightBracket = ']';
                    break;
                case '\"': rightBracket = '\"';
                    break;
                default:
                    throw new System.Exception();
            }

            int matchingInd = start;
            stk1.Push(leftBracket);
            for (int i = start; i < end; i++)
            {
                char c = str[i];

                if(stk1.Count > 0 && stk1.Peek() == '\"')
                {
                    if (c == '\"')
                    {
                        stk1.Pop();
                        if (leftBracket == '\"' && stk1.Count == 0) return i;
                    }

                    continue;
                }
                if (c == '\"') stk1.Push(c);
                if (c == leftBracket) stk1.Push(c);
                if (c == rightBracket)
                {
                    stk1.Pop();

                }
                if (stk1.Count == 0)
                {
                    matchingInd = i;
                    break;
                }
            }
            return matchingInd;
        }

        static HashSet<char> leftSymbol = new HashSet<char> { '\"', '[', '{' };
        //static HashSet<char> rightSymbol = new HashSet<char>{ '\"', ']', ']' };
        /// <summary>
        /// 用逗号分割第一级别元素
        /// </summary>
        /// <returns></returns>
        static List<string> SplitItemByComma(string str, int start, int end)
        {
            List<string> strList = new List<string>();

            int leftPtr = start;
            int rightPtr = leftPtr;
            //char targetSymbol = '0';
            bool jump = false;

            int i = start;
            while (i<end)
            {

                char tmp = str[i];
                if (!jump && leftSymbol.Contains(str[i]))
                {
                    //stk2.Push(str[i]);

                    int j = GetMatchingBracket(str[i], str, i + 1, end);

                    jump = true;
                    i = j;
                    continue;
                }

                if (jump) jump = false;

                //if (waitTargetSymbol && targetSymbol == str[i])
                //{
                //    waitTargetSymbol = false;
                //    //char tmp = stk2.Pop();
                //    continue;
                //}

                //if (!waitTargetSymbol)
                {
                    if (str[i] == ',')
                    {

                        rightPtr = i;
                        string itemStr = str.Substring(leftPtr, rightPtr - leftPtr);
                        strList.Add(itemStr);
                        leftPtr = i + 1;

                    }

                    if (i == end - 1)
                    {
                        rightPtr = i + 1;
                        string itemStr = str.Substring(leftPtr, rightPtr - leftPtr);
                        strList.Add(itemStr);
                    }
                }
                i++;
            }

            return strList;
        }



        //static HashSet<char> leftSymbol = new HashSet<char>{ '\"', '[', '{' };
        ////static HashSet<char> rightSymbol = new HashSet<char>{ '\"', ']', ']' };
        ///// <summary>
        ///// 用逗号分割第一级别元素
        ///// </summary>
        ///// <returns></returns>
        //static List<string> SplitItemByComma(string str, int start, int end)
        //{
        //    List<string> strList = new List<string>();

        //    int leftPtr = start;
        //    int rightPtr = leftPtr;
        //    char targetSymbol = '0';
        //    bool waitTargetSymbol = false;
        //    for (int i = start; i < end; i++)
        //    {

        //        char tmp = str[i];
        //        if (!waitTargetSymbol && leftSymbol.Contains(str[i]))
        //        {
        //            //stk2.Push(str[i]);

        //            switch (str[i])
        //            {
        //                case '\"': targetSymbol = '\"'; break;
        //                case '[': targetSymbol = ']'; break;
        //                case '{': targetSymbol = '}'; break;
        //            }

        //            waitTargetSymbol = true;
        //            continue;
        //        }

        //        if (waitTargetSymbol && targetSymbol == str[i])
        //        {
        //            waitTargetSymbol = false;
        //            //char tmp = stk2.Pop();


        //        }

        //        if (!waitTargetSymbol)
        //        {
        //            if (str[i] == ',')
        //            {

        //                rightPtr = i;
        //                string itemStr = str.Substring(leftPtr, rightPtr - leftPtr);
        //                strList.Add(itemStr);
        //                leftPtr = i + 1;

        //            }

        //            if (i == end - 1)
        //            {
        //                rightPtr = i + 1;
        //                string itemStr = str.Substring(leftPtr, rightPtr - leftPtr);
        //                strList.Add(itemStr);
        //            }
        //        }
        //    }

        //    return strList;
        //}

        public static HashSet<string> blockAttrs = new HashSet<string> {  };

        /// <summary>
        /// Start End 遵循左闭右开原则
        /// </summary>
        /// <param name="nodeStr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Dictionary<string, object> StrToDic(string nodeStr)
        {
            if (nodeStr == null)
            {
                return null;
            }

            Dictionary<string, object> nodeDic = new Dictionary<string, object>();


            string left = "{{";

            //parse payload
            int payloadStart = 2;
            int payloadEnd = GetMatchingBracket('{', nodeStr, 2, nodeStr.Length);

            List<string> payloadList = SplitItemByComma(nodeStr, payloadStart, payloadEnd);

            Dictionary<string, object> payloadDic = new Dictionary<string, object>();

            int attrCnt = 0;
            if (!blockAttrs.Contains("name"))
            {
                payloadDic["name"] = TrimQuatation(payloadList[attrCnt++]);
            }
            if (!blockAttrs.Contains("visible"))
                payloadDic["visible"] = StrToBool(payloadList[attrCnt++]);

            if (!blockAttrs.Contains("pos"))
                payloadDic["pos"] = StrToFloatArr(payloadList[attrCnt++]);

            if (!blockAttrs.Contains("size"))
                payloadDic["size"] = StrToFloatArr(payloadList[attrCnt++]);

            if (!blockAttrs.Contains("anchorPoint"))
                payloadDic["anchorPoint"] = StrToFloatArr(payloadList[attrCnt++]);

            if (!blockAttrs.Contains("zOrders"))
                payloadDic["zOrders"] = StrToZOrdersDic(payloadList[attrCnt++]);

            nodeDic["name"] = TrimQuatation(payloadList[0]);
            nodeDic["payload"] = payloadDic;

            if (payloadEnd == nodeStr.Length - 1 - 1) return nodeDic;

            bool valid = false;
            if(nodeStr[payloadEnd+1] == ',' & nodeStr[payloadEnd + 2] == '[')
            {
                valid = true;
            }

            if (!valid) throw new System.Exception("Invalid str");

            int childrenStart = payloadEnd +  3;
            int childrenEnd = nodeStr.Length -2;


            List<string> childrenStrList = SplitItemByComma(nodeStr, childrenStart, childrenEnd);

            List<object> childList = new List<object>();
            for(int i=0; i<childrenStrList.Count; i++)
            {
                childList.Add(StrToDic(childrenStrList[i]));
            }

            if(childList.Count != 0) nodeDic["children"] = childList;

            return nodeDic;

        }

        static private string TrimQuatation(string str)
        {
            bool valid = str[0] == '\"' && str[str.Length - 1] == '\"';
            if (!valid) throw new System.Exception("TrimQuatation::Invalid str");
            return str.Substring(1, str.Length - 2);

        }

        static private bool StrToBool(string str)
        {
            if (str == "\"True\"") return true;
            if (str == "\"False\"") return false;

            throw new System.Exception("StrToBool::invalid str");
        }

        static  private float[] StrToFloatArr(string str)
        {
            string tmp = str.Substring(1, str.Length - 2);
            string[] tmpArr  = tmp.Split(',');
            float n1 = float.Parse(tmpArr[0]);
            float n2 = float.Parse(tmpArr[1]);
            float[] arr = new float[] { n1, n2 };
            return arr;
        }
        static private Dictionary<string, float> StrToZOrdersDic(string str)
        {
            string tmp = str.Substring(1, str.Length - 2);
            string[] tmpArr = tmp.Split(',');
            float n1 = float.Parse(tmpArr[0]);
            float n2 = float.Parse(tmpArr[1]);
            Dictionary<string, float> dic = new Dictionary<string, float>();
            dic["global"] = n1;
            dic["local"] = n2;

            return dic;
        }

    }
}