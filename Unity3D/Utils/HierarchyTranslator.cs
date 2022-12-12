using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poco.Utils
{
    public class HierarchyTranslator
    {
        public static string HierarchyToStr(Dictionary<string, object> h)
        {

            return DfsToStr(h);
        }




        private static string DfsToStr(Dictionary<string, object> nodeDic)
        {
            if (nodeDic == null || nodeDic.Count == 0)
            {
                return null;
            }

            string left = "{{";

            Dictionary<string, object> objPayload = nodeDic["payload"] as Dictionary<string, object>;

            string name = QuotationStr(objPayload["name"].ToString());
            string visible = QuotationStr(objPayload["visible"].ToString());
            string pos = FloatArrToStr((float[])objPayload["pos"]);
            string size = FloatArrToStr((float[])objPayload["size"]);
            string anchorPoint = FloatArrToStr((float[])objPayload["anchorPoint"]);

            left = left + name + "," + visible + "," + pos + "," + size + "," + anchorPoint + "}";

            object childListObj = null;

            nodeDic.TryGetValue("children", out childListObj);

            List<object> chldList = childListObj as List<object>;

            if (chldList != null && chldList.Count != 0)
            {
                string childLeft = ",[";
                int childCnt = 0;
                for(int i=0;i< chldList.Count; i++)
                {
                    object o = chldList[i];

                    string childStr = DfsToStr((Dictionary<string, object>)o);
                    if(childStr!=null)
                    {
                        childCnt++;
                        if (i != 0) childLeft += ",";
                        childLeft += childStr;
                    }
                }

                if(childCnt > 0)
                {
                    string childRight = "]";
                    left = left + childLeft + childRight;
                }
            }

            string right = "}";
            return left + right;

        }


        private static string FloatArrToStr(float[] arr)
        {
            if(arr==null||arr.Length == null)
            {
                return null;
            }

            string left = "[";
            for(int i=0; i<arr.Length; i++)
            {
                if (i != 0) left += ",";

                left += arr[i].ToString();
            }

            string right = "]";
            return left + right;
        }

        private static string QuotationStr(string str)
        {
            string left = "\"";
            string right = left;
            left += str;
            return left + right;
        }

    }

}

