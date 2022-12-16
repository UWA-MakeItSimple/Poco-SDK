using Poco;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Poco.TcpServer;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Poco.Utils;
using UJson = UWA.LitJson;


namespace Poco
{

    public class PocoManager : MonoBehaviour
    {
        
        public const string versionCode = "UWA-2.0";
        public int port = 5001;
        private bool mRunning;
        public AsyncTcpServer server = null;
        private RPCParser rpc = null;
        private SimpleProtocolFilter prot = null;
        //private enum EDataMode { Detailed,  Optimized }


        //private IDumper<GameObject> dumperOriginal = new UnityDumper();
        //private UnityDumper dumperOptimized = new UnityDumper();

        private IDumper<GameObject> dumper = new UnityDumper();


        // = new UnityDumperOptimized();
        private ConcurrentDictionary<string, TcpClientState> inbox = new ConcurrentDictionary<string, TcpClientState>();
        private VRSupport vr_support = new VRSupport();
        private Dictionary<string, long> debugProfilingData = new Dictionary<string, long>() {
            { "dump", 0 },
            { "screenshot", 0 },
            { "handleRpcRequest", 0 },
            { "packRpcResponse", 0 },
            { "sendRpcResponse", 0 },
        };

        class RPC : Attribute
        {
        }



#if UNITY_EDITOR

        //string remoteCallMsg = "{\"method\": \"Dump\", \"params\": [true], \"jsonrpc\": \"2.0\", \"id\": \"10630993-7bd4-404c-b6b2-c99f8a26bb8f\"}";
        string remoteCallMsg = "{\"method\": \"SetPruningEnabled\", \"params\": [true], \"jsonrpc\": \"2.0\", \"id\": \"10630993-7bd4-404c-b6b2-c99f8a26bb8f\"}";

        private void OnGUI()
        {
            GUIStyle btnStyle = new GUIStyle("Button");
            btnStyle.fontSize = 150;
            GUILayout.BeginArea(new Rect(0, 0, 1920, 1080));

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("DirectDump", btnStyle))
            {
                Config.Instance.optimizeDataEnabled = true;
                //Config.Instance.pruningEnabled = true;
                string result = (string)Dump(new List<object> { true });

                //JsonSerializerSettings settings = new JsonSerializerSettings()
                //{
                //    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii

                //};

                string response = rpc.formatResponse("123", result);
                Debug.Log(response);

                //int dataSize = 0;
                //byte[] bytes = prot.pack(response, out dataSize);

                //int len = response.Length;
                //byte[] size = BitConverter.GetBytes(len);
                //byte[] unpackedData = Poco.TcpServer.SimpleProtocolFilter.Slice(bytes, size.Length, bytes.Length);
                //string unpackedString = System.Text.Encoding.Default.GetString(unpackedData);
                //UnityEngine.Debug.Log(unpackedData);

                string timeTag = DateTime.Now.ToString("dd-HHmmss");
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("DumpData/DumpData-" + timeTag + ".json"))
                {
                    sw.Write(result);
                }
                UnityEngine.Debug.Log(result);




                Dictionary<string, object> dic = Deserialize.StrToDic(result);
                string res2 = Poco.Utils.HierarchyTranslator.HierarchyToStr(dic);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("DumpData/DumpData-" + timeTag + "2.json"))
                {
                    sw.Write(res2);
                }
                UnityEngine.Debug.Log(res2);
                DicPoolManager.ReleasePools();
                ListPoolManager.ReleasePools();
                ArrPoolManager.ReleasePools();
                //byte[] resBytes = System.Text.Encoding.Default.GetBytes(response);
                //UnityEngine.Debug.Log(((float)resBytes.Length) / 1024 / 1024);
            }

            if (GUILayout.Button("RpcDump", btnStyle))
            {
                string rst = rpc.HandleMessage(remoteCallMsg);

                Debug.Log(rst);
            }


            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

#elif UWA_POCO_DEBUG
        private void OnGUI()
        {
            GUIStyle btnStyle = new GUIStyle("Button");
            btnStyle.fontSize = 150;
            GUILayout.BeginArea(new Rect(0, 0, 1920, 1080));

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Dump", btnStyle))
            { 
                //Config.Instance.pruningEnabled = true;
                object result = Dump(new List<object> { true });

                //JsonSerializerSettings settings = new JsonSerializerSettings()
                //{
                //    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii

                //};
                //settings.Formatting = Formatting.Indented;
                string response = rpc.formatResponse("123", result);

                //string timeTag = DateTime.Now.ToString("dd-HHmmss");
                //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("DumpData/DumpData-" + timeTag +"-" +srlzr.GetType().Name + ".json"))
                //{
                //    sw.Write(response);
                //}


                //byte[] resBytes = System.Text.Encoding.Default.GetBytes(response);
                //UnityEngine.Debug.Log(((float)resBytes.Length) / 1024 / 1024);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
#endif

        void Awake()
        {
            UWASDKAgent.PushSample("PocoManager.Awake");

            LogUtil.ULogDev("PocoManager awake");


            Application.runInBackground = true;
            DontDestroyOnLoad(this);
            prot = new SimpleProtocolFilter();
            rpc = new RPCParser();
            rpc.addRpcMethod("isVRSupported", vr_support.isVRSupported);
            rpc.addRpcMethod("isVrSupported", vr_support.isVRSupported);
            rpc.addRpcMethod("hasMovementFinished", vr_support.IsQueueEmpty);
            rpc.addRpcMethod("RotateObject", vr_support.RotateObject);
            rpc.addRpcMethod("ObjectLookAt", vr_support.ObjectLookAt);
            rpc.addRpcMethod("Screenshot", Screenshot);
            rpc.addRpcMethod("GetScreenSize", GetScreenSize);
            rpc.addRpcMethod("Dump", Dump);
            rpc.addRpcMethod("GetDebugProfilingData", GetDebugProfilingData);
            rpc.addRpcMethod("SetText", SetText);
            rpc.addRpcMethod("GetSDKVersion", GetSDKVersion);

            rpc.addRpcMethod("SetBlackList", SetBlackList);
            rpc.addRpcMethod("SetWhiteList", SetWhiteList);
            rpc.addRpcMethod("SetPruningEnabled", SetPruningEnabled);
            rpc.addRpcMethod("SetOptimizeDataEnabled", SetOptimizeDataEnabled);
            rpc.addRpcMethod("SetBlockedAttributes", SetBlockedAttributes);
            rpc.addRpcMethod("CollectWeakWhitelist", CollectWeakWhitelist);
            rpc.addRpcMethod("GetDumpInfo", GetDumpInfo);



            mRunning = true;

            for (int i = 0; i < 5; i++)
            {
                this.server = new AsyncTcpServer(port + i);
                this.server.Encoding = Encoding.UTF8;
                this.server.ClientConnected +=
                    new EventHandler<TcpClientConnectedEventArgs>(server_ClientConnected);
                this.server.ClientDisconnected +=
                    new EventHandler<TcpClientDisconnectedEventArgs>(server_ClientDisconnected);
                this.server.DatagramReceived +=
                    new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(server_Received);
                try
                {
                    this.server.Start();
                    LogUtil.ULogDev(string.Format("Tcp server started and listening at {0}", server.Port));
                    break;
                }
                catch (SocketException e)
                {
                    Debug.Log(string.Format("Tcp server bind to port {0} Failed!", server.Port));
                    Debug.Log("--- Failed Trace Begin ---");
                    Debug.LogError(e);
                    Debug.Log("--- Failed Trace End ---");
                    // try next available port
                    this.server = null;
                }
            }
            if (this.server == null)
            {
                Debug.LogError(string.Format("Unable to find an unused port from {0} to {1}", port, port + 5));
            }

            vr_support.ClearCommands();
            UWASDKAgent.PopSample();
        }

        static void server_ClientConnected(object sender, TcpClientConnectedEventArgs e)
        {
            Debug.Log(string.Format("TCP client {0} has connected.",
                e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        static void server_ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            Debug.Log(string.Format("TCP client {0} has disconnected.",
               e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void server_Received(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            Debug.Log(string.Format("Client : {0} --> {1}",
                e.Client.TcpClient.Client.RemoteEndPoint.ToString(), e.Datagram.Length));
            TcpClientState internalClient = e.Client;
            string tcpClientKey = internalClient.TcpClient.Client.RemoteEndPoint.ToString();
            inbox.AddOrUpdate(tcpClientKey, internalClient, (n, o) =>
            {
                return internalClient;
            });
        }

        TextAsset largetTxtAst = null;

        [RPC]
        private object Dump(List<object> param)
        {

            UWASDKAgent.PushSample("PocoManager.Dump");


            if (dumper == null)
                throw new Exception("Dumper has not been initialized");

            var onlyVisibleNode = true;
            if (param.Count > 0)
            {
                onlyVisibleNode = (bool)param[0];
            }
            var sw = new Stopwatch();
            sw.Start();
            Dictionary<string, object>  h = dumper.DumpHierarchy(onlyVisibleNode);
            debugProfilingData["dump"] = sw.ElapsedMilliseconds;

            LogUtil.ULogDev("Dump Method executed");

            if(Config.Instance.optimizeDataEnabled)
            {
                //string res = Poco.Utils.HierarchyTranslator.HierarchyToStr(h);
                string res = Poco.Utils.HierarchyTranslator.HierarchyToStr(h);

                UWASDKAgent.PopSample();

                return res;

            }
            else
            {
                UWASDKAgent.PopSample();
                return h;
            }

        }

        [RPC]
        private object Screenshot(List<object> param)
        {
            var sw = new Stopwatch();
            sw.Start();

            var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            tex.Apply(false);
            byte[] fileBytes = tex.EncodeToJPG(80);
            var b64img = Convert.ToBase64String(fileBytes);
            debugProfilingData["screenshot"] = sw.ElapsedMilliseconds;
            return new object[] { b64img, "jpg" };
        }

        [RPC]
        private object GetScreenSize(List<object> param)
        {
            return new float[] { Screen.width, Screen.height };
        }

        public void stopListening()
        {
            mRunning = false;
            server?.Stop();
        }

        [RPC]
        private object GetDebugProfilingData(List<object> param)
        {
            return debugProfilingData;
        }

        [RPC]
        private object SetText(List<object> param)
        {
            var instanceId = Convert.ToInt32(param[0]);
            var textVal = param[1] as string;
            foreach (var go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.GetInstanceID() == instanceId)
                {
                    return UnityNodeGrabber.SetText(go, textVal);
                }
            }
            return false;
        }

        [RPC]
        private object GetSDKVersion(List<object> param)
        {
            return versionCode;
        }


        #region UWA_POCO_API
        [RPC]
        private object SetBlackList(List<object> param)
        {
            LogUtil.ULogDev("SetBlackList");

            HashSet<string> bl = new HashSet<string>();

            foreach (var p in param)
            {
                bl.Add((string)p);
            }

            LogUtil.ULogDev("BlackList: ", bl);
            Config.Instance.blackList = bl;

            return "SetBlackList: " + bl.Count;
        }

        [RPC]
        private object SetWhiteList(List<object> param)
        {
            LogUtil.ULogDev("SetWhiteList");

            HashSet<string> wl = new HashSet<string>();

            foreach (var p in param)
            {
                wl.Add((string)p);
            }

            LogUtil.ULogDev("WhiteList: ", wl);
            Config.Instance.strongWhiteList = wl;

            return "SetWhiteList: " + wl.Count;
        }

        [RPC]
        private object SetPruningEnabled(List<object> param)
        {
            var value = true;
            if (param.Count > 0)
            {
                value = (bool)param[0];
            }

            Config.Instance.pruningEnabled = value;

            return "SetPruningEnabled " + value;
        }

        [RPC]
        private object SetOptimizeDataEnabled(List<object> param)
        {
            var value = true;
            if (param.Count > 0)
            {
                value = (bool)param[0];
            }

            Config.Instance.optimizeDataEnabled = value;

            return "SetOptimizeDataEnabled " + value;
        }

        [RPC]
        private object SetBlockedAttributes(List<object> param)
        {
            LogUtil.ULogDev("SetBlockedAttributes");

            HashSet<string> ba = new HashSet<string>();

            foreach (var p in param)
            {
                string tmp = (string)p;
                if (!Config.Instance.AttrCannotBlock.Contains(tmp))
                {
                    ba.Add(tmp);
                }
            }

            Config.Instance.blockedAttributes = ba;

            return "SetBlockedAttributes: " + ba.Count;
        }
            

        [RPC]
        private object CollectWeakWhitelist(List<object> param)
        {

            foreach (GameObject obj in Transform.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.transform.parent == null)
                {

                    if (obj.HasUIInChildren())
                        Config.Instance.weakWhiteList.Add(obj.name);

                }
            }

            LogUtil.ULogDev("WeakWhiteList: ", Config.Instance.weakWhiteList);

            return "CollectWeakWhitelist succeeded";

        }

        [RPC]
        private object GetDumpInfo(List<object> param)
        {

            object result = Dump(new List<object> { true });

            return result;

        }

        #endregion


        void Update()
        {
            UWASDKAgent.PushSample("PocoManager.Update");
            foreach (TcpClientState client in inbox.Values)
            {
                List<string> msgs = client.Prot.swap_msgs();

                foreach(string msg in msgs)
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var t0 = sw.ElapsedMilliseconds;
                    string response = rpc.HandleMessage(msg);

                    var t1 = sw.ElapsedMilliseconds;

                    int dataSize = 0;
                    byte[] bytes = prot.pack(response, out dataSize);
                    var t2 = sw.ElapsedMilliseconds;

                    server.Send(client.TcpClient, bytes, dataSize);
                    var t3 = sw.ElapsedMilliseconds;

                    debugProfilingData["handleRpcRequest"] = t1 - t0;
                    debugProfilingData["packRpcResponse"] = t2 - t1;
                    TcpClientState internalClientToBeThrowAway;
                    string tcpClientKey = client.TcpClient.Client.RemoteEndPoint.ToString();
                    inbox.TryRemove(tcpClientKey, out internalClientToBeThrowAway);

                    DicPoolManager.ReleasePools();
                    ListPoolManager.ReleasePools();
                    ArrPoolManager.ReleasePools();


                }

            }

            vr_support.PeekCommand();
            UWASDKAgent.PopSample();

        }

        //private void OnGUI()
        //{
        //    GUIStyle style = new GUIStyle("Button");
        //    style.fontSize = 300;
        //    if(GUILayout.Button("Send", style))
        //    {

        //        foreach (TcpClientState client in inbox.Values)
        //        {

        //            var sw = new Stopwatch();
        //            sw.Start();

        //            var res = Resources.Load("Hierarchy-timeout") as TextAsset;

        //            server.Send(client.TcpClient, res.text);
        //            var t3 = sw.ElapsedMilliseconds;

        //            TcpClientState internalClientToBeThrowAway;
        //            string tcpClientKey = client.TcpClient.Client.RemoteEndPoint.ToString();
        //            inbox.TryRemove(tcpClientKey, out internalClientToBeThrowAway);
        //        }
        //    }
        //}

        void OnApplicationQuit()
        {
            // stop listening thread
            stopListening();
        }

        void OnDestroy()
        {
            // stop listening thread
            stopListening();
        }

    }


    public class RPCParser
    {
        public delegate object RpcMethod(List<object> param);

        protected Dictionary<string, RpcMethod> RPCHandler = new Dictionary<string, RpcMethod>();
        //private JsonSerializerSettings settings = new JsonSerializerSettings()
        //{
        //    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        //};

        public string HandleMessage(string json)
        {
            //Debug.Log(json);

            UWASDKAgent.PushSample("PocoManager.HandleMessage");

            //LitJson.JsonReader rd = new LitJson.JsonReader(json);

            UJson.JsonData data = UJson.JsonMapper.ToObject(json);

            
            if (data.ContainsKey("method"))
            {

                string method = null;
                try
                {
                    method = data["method"].ToString();

                }catch(Exception e)
                {
                    throw e;
                }


                List<object> paramList = new List<object>();

                if (data.ContainsKey("params"))
                {
                    UJson.JsonData json_params = data["params"];

                    for(int i=0; i< json_params.Count; i++)
                    {
                        UJson.JsonData json_para = json_params[i];
                        if(json_para.IsBoolean)
                        {
                            bool tmp = (bool)(json_params[i]);
                            paramList.Add(tmp);
                        }
                        else if (json_para.IsInt)
                        {
                            int tmp = (int)(json_params[i]);
                            paramList.Add(tmp);
                        }
                        else if (json_para.IsDouble)
                        {
                            float tmp = (float)(double)(json_params[i]);
                            paramList.Add(tmp);
                        }
                        else if(json_para.IsString)
                        {
                            string tmp = (string)(json_params[i]);
                            paramList.Add(tmp);
                        }
                    }
                }

                object idAction = null;
                if (data.ContainsKey("id"))
                {
                    // if it have id, it is a request
                    idAction = data["id"].ToString();
                }

                LogUtil.ULogDev(string.Format("HandleMsg: Method-{0}, params-{1}, id-{2}", method, paramList, idAction));

                string response = null;
                object result = null;
                try
                {
                    
                    result = RPCHandler[method](paramList);
                }
                catch (Exception e)
                {
                    // return error response
                    Debug.Log(e);
                    response = formatResponseError(idAction, null, e);
                    return response;
                }

                // return result response
                if(result.GetType() == typeof(string))
                {
   
                    //自己拼接字符串的方式，由于双引号问题导致数据无效，此处还是先使用原先的序列化方式。
                    response = formatResponse(idAction, (object)result);

                }
                else
                {
                    response = formatResponse(idAction, result);

                }

                if (response.Length < 10240)
                    LogUtil.ULogDev("Response: " + response);
                else
                    LogUtil.ULogDev("Response: larger than 10240 chars");

                UWASDKAgent.PopSample();
                return response;

            }
            else
            {
                // do not handle response
                Debug.Log("ignore message without method");
                return null;
            }
        }

        // Call a method in the server
        public string formatRequest(string method, object idAction, List<object> param = null)
        {
            //Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> data = DicPoolSO4.Ins.GetObj();
            data["jsonrpc"] = "2.0";
            data["method"] = method;
            if (param != null)
            {
                //data["params"] = JsonConvert.SerializeObject(param, settings);
                data["params"] = UJson.JsonMapper.ToJson(param);
            }
            // if idAction is null, it is a notification
            if (idAction != null)
            {
                data["id"] = idAction;
            }
            //return JsonConvert.SerializeObject(data, settings);
            return JsonAgent.SerializeDic(data);
        }

        // Send a response from a request the server made to this client
        public string formatResponse(object idAction, object result)
        {

            //if(result.GetType() == typeof(string))
            //{
            //    throw new Exception("Invalid result");
            //}
            //else
            {
                //Dictionary<string, object> rpc = new Dictionary<string, object>();
                Dictionary<string, object> rpc = DicPoolSO3.Ins.GetObj();
                rpc["jsonrpc"] = "2.0";
                rpc["id"] = idAction;
                rpc["result"] = result;
                //return Utf8Json.JsonSerializer.ToJsonString(rpc);
                //return Newtonsoft.Json.JsonConvert.SerializeObject(rpc, settings);
                //return Newtonsoft.Json.JsonConvert.SerializeObject(rpc);

                return JsonAgent.SerializeDic(rpc); ;
                //return JsonAgent.SerializeResponse(rpc);
            }


        }
        StringBuilder responseSB = new StringBuilder();
        string responseHead = "{\"jsonrpc\":\"2.0\",\"id\":";
        public string formatResponse(object idAction, string result)
        {

            if (result.GetType() != typeof(string))
            {
                throw new Exception("Invalid result");
            }
            else
            {
                if (responseSB == null) responseSB = new StringBuilder();
                responseSB.Clear();

                responseSB.AppendFormat("{0}\"{1}\",\"result\":{2}", responseHead, idAction.ToString(), result);
                //Dictionary<string, object> rpc = new Dictionary<string, object>();
                //return Utf8Json.JsonSerializer.ToJsonString(rpc);
                //return Newtonsoft.Json.JsonConvert.SerializeObject(rpc, settings);
                //return Newtonsoft.Json.JsonConvert.SerializeObject(rpc);
                responseSB.Append("}");
                return responseSB.ToString();
                //return JsonAgent.SerializeResponse(rpc);
            }


        }


        public string formatResponseError(object idAction, IDictionary<string, object> data, Exception e)
        {
            //Dictionary<string, object> rpc = new Dictionary<string, object>();

            Dictionary<string, object> rpc = DicPoolSO3.Ins.GetObj();

            rpc["jsonrpc"] = "2.0";
            rpc["id"] = idAction;

            //Dictionary<string, object> errorDefinition = new Dictionary<string, object>();
            Dictionary<string, object> errorDefinition = DicPoolSO3.Ins.GetObj();
            errorDefinition["code"] = 1;
            errorDefinition["message"] = e.ToString();

            if (data != null)
            {
                errorDefinition["data"] = data;
            }

            rpc["error"] = errorDefinition;
            //return JsonConvert.SerializeObject(rpc, settings);
            return JsonAgent.SerializeDic(rpc);

        }


        public void addRpcMethod(string name, RpcMethod method)
        {
            RPCHandler[name] = method;
        }
    }
}