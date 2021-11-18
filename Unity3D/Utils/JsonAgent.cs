using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Poco.Utils
{


    public class JsonAgent
    {

        static StringBuilder sb = new StringBuilder();


        //General serializing method using ToJson()
        public static string SerializeDic(Dictionary<string, object> dic)
        {
            string res = LitJson.JsonMapper.ToJson(dic);
            return res;

        }

        //public static object DeserializeToDic(string json)
        //{
        //    object dic = LitJson.JsonMapper.ToObject(json);
        //    return dic;
        //}

        public static string SerializeDumpResponse(Dictionary<string, object> dic)
        {

            LitJson.JsonWriter writer = new LitJson.JsonWriter(sb);
            
            //writer.IndentValue = 2;
            //writer.PrettyPrint = true;
            // {
            writer.WriteObjectStart();

            // "name" : "Jerry"
            writer.WritePropertyName("jsonrpc");
            writer.Write((string)dic["jsonrpc"]);

            writer.WritePropertyName("id");
            writer.Write((string)dic["id"]);


            writer.WritePropertyName("result");

            {
                SerializeNode((Dictionary<string, object>)dic["result"], writer);
            }

            // }
            writer.WriteObjectEnd();

            string result = sb.ToString();
            sb.Clear();
            return result;
                
        }

        static string[] floatArrKeys = new string[]
        {
            "size",
            "pos",
            "scale",
            "anchorPoint",

        };

        static string[] stringArrKeys = new string[] { "components" };

        static string[] stringValuePayloadKeys = new string[]
        {
            "name",
            "type",
            "texture",
            "layer",
            "text",
            "tag"
        };

        static string[] boolValuePayloadKeys = new string[]
        {

            "visible",
            "clickable",

        };

        static string[] intValuePayloadKeys = new string[]
        {
                "_ilayer",
                "_instanceId",
        };

        static string zOrdersKey = "zOrders";


        static void SerializeNode(Dictionary<string, object> dic, LitJson.JsonWriter writer)
        {
            writer.WriteObjectStart();


            writer.WritePropertyName("name");
            writer.Write((string)dic["name"]);
            writer.WritePropertyName("payload");
            {
                writer.WriteObjectStart();

                foreach (var kvp in ((Dictionary<string, object>)dic["payload"]))
                {


                    writer.WritePropertyName(kvp.Key);

                    //switch(kvp.Key)
                    //{
                    //    case "name":
                    //    case "type":
                    //    case "texture":
                    //    case "layer":
                    //    case "text":
                    //    case "tag":
                    //        writer.Write((string)kvp.Value);
                    //        break;
                    //    case "visible":
                    //    case "clickable":
                    //        writer.Write((bool)kvp.Value);
                    //        break;
                    //    case "_ilayer":
                    //    case "_instanceId":
                    //        writer.Write((int)kvp.Value);
                    //        break;
                    //    case "size":
                    //    case "pos":
                    //    case "scale":
                    //    case "anchorPoint":
                    //        writer.WriteArrayStart();
                    //        float[] floatArr = kvp.Value as float[];

                    //        try
                    //        {
                    //            foreach (var item in floatArr)
                    //            {
                    //                writer.Write(item);
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            UnityEngine.Debug.LogError(kvp.Key);
                    //            throw ex;
                    //        }


                    //        writer.WriteArrayEnd();
                    //        break;
                    //    case "components":
                    //        writer.WriteArrayStart();
                    //        List<string> strArr = kvp.Value as List<string>;

                    //        foreach (var item in strArr)
                    //        {
                    //            writer.Write(item);
                    //        }


                    //        writer.WriteArrayEnd();
                    //        break;
                    //    case "zOrders":
                    //        Dictionary<string, float> dictZOrders = kvp.Value as Dictionary<string, float>;

                    //        writer.WriteObjectStart();

                    //        writer.WritePropertyName("global");
                    //        writer.Write(dictZOrders["global"]);

                    //        writer.WritePropertyName("local");
                    //        writer.Write(dictZOrders["local"]);

                    //        writer.WriteObjectEnd();
                    //        break;
                    //    default:
                    //        UnityEngine.Debug.LogError(kvp.Key);
                    //        throw new Exception("Unknown payload key");
                    //}


                    if (stringValuePayloadKeys.Contains(kvp.Key))
                    {
                        writer.Write((string)kvp.Value);

                    }
                    else if (boolValuePayloadKeys.Contains(kvp.Key))
                    {

                        writer.Write((bool)kvp.Value);

                    }
                    else if (intValuePayloadKeys.Contains(kvp.Key))
                    {

                        writer.Write((int)kvp.Value);

                    }
                    else if (floatArrKeys.Contains(kvp.Key))
                    {
                        writer.WriteArrayStart();
                        float[] list = kvp.Value as float[];

                        try
                        {
                            foreach (var item in list)
                            {
                                writer.Write(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogError(kvp.Key);
                            throw ex;
                        }


                        writer.WriteArrayEnd();
                    }
                    else if (stringArrKeys.Contains(kvp.Key))
                    {

                        writer.WriteArrayStart();
                        List<string> list = kvp.Value as List<string>;

                        foreach (var item in list)
                        {
                            writer.Write(item);
                        }


                        writer.WriteArrayEnd();

                    }
                    else if (zOrdersKey == kvp.Key)
                    {

                        Dictionary<string, float> dictZOrders = kvp.Value as Dictionary<string, float>;

                        writer.WriteObjectStart();

                        writer.WritePropertyName("global");
                        writer.Write(dictZOrders["global"]);

                        writer.WritePropertyName("local");
                        writer.Write(dictZOrders["local"]);

                        writer.WriteObjectEnd();
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(kvp.Key);
                        throw new Exception("Unknown payload key");
                    }

                }
                writer.WriteObjectEnd();

            }

            object children;

            dic.TryGetValue("children", out children);

            if (children != null && ((List<object>)children).Count != 0)
            {

                writer.WritePropertyName("children");
                {
                    writer.WriteArrayStart();

                    foreach (var child in (List<object>)children)
                    {
                        SerializeNode((Dictionary<string, object>)child, writer);
                    }


                    writer.WriteArrayEnd();
                }


            }

            writer.WriteObjectEnd();
        }

    }

}
