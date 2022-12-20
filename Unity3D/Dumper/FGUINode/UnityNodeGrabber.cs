using System;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Poco.Utils;

namespace Poco
{
    public class UnityNodeGrabber : Singleton<UnityNodeGrabber>, INodeGrabber
    {
        public static string DefaultTypeName = "GameObject";
        private GameObject gameObject;
        private Renderer renderer;
        private DisplayObject displayObject;
        private GObject gObject;
        private List<string> components;
        private Camera camera;
        private Camera[] allCams;

        public string name;
        public bool protectedByParent = false;
        private DisplayObjectInfo doInfo = null;
        //private UIPanel uipanelComp;
        static float[] floatOneOne = new float[2] { 1.0f, 1.0f };
        static float[] floatZeroZero = new float[] { 0.0f, 0.0f };

        public UnityNodeGrabber()
        {

        }

        static bool CompNameDicInited = false;
        static Dictionary<Type, string> CompNameDic = new Dictionary<Type, string>();

        public void Init()
        {
            allCams = Camera.allCameras;

            if (CompNameDicInited == false)
            {
                //CompNameDic.TryAdd(typeof(Transform), "Transform");
                //CompNameDic.TryAdd(typeof(GUIWrapper), "GUIWrapper");
                //CompNameDic.TryAdd(typeof(Camera), "Camera");
                //CompNameDic.TryAdd(typeof(FlareLayer), "FlareLayer");
                //CompNameDic.TryAdd(typeof(AudioListener), "AudioListener");
                //CompNameDic.TryAdd(typeof(Animator), "Animator");
                //CompNameDic.TryAdd(typeof(SpriteRenderer), "SpriteRenderer");
                //CompNameDic.TryAdd(typeof(ParticleSystem), "ParticleSystem");
                //CompNameDic.TryAdd(typeof(ParticleSystemRenderer), "ParticleSystemRenderer");
                //CompNameDic.TryAdd(typeof(MeshCollider), "MeshCollider");
                //CompNameDic.TryAdd(typeof(EventSystem), "EventSystem");
                //CompNameDic.TryAdd(typeof(StandaloneInputModule), "StandaloneInputModule");
                //CompNameDic.TryAdd(typeof(TouchInputModule), "TouchInputModule");
                //CompNameDic.TryAdd(typeof(BaseInput), "BaseInput");
                //CompNameDic.TryAdd(typeof(RectTransform), "RectTransform");
                //CompNameDic.TryAdd(typeof(Canvas), "Canvas");
                //CompNameDic.TryAdd(typeof(GraphicRaycaster), "GraphicRaycaster");
                //CompNameDic.TryAdd(typeof(CanvasScaler), "CanvasScaler");
                //CompNameDic.TryAdd(typeof(PanelManager), "PanelManager");
                //CompNameDic.TryAdd(typeof(CanvasRenderer), "CanvasRenderer");
                //CompNameDic.TryAdd(typeof(Image), "Image");
                //CompNameDic.TryAdd(typeof(TiltWindow), "TiltWindow");
                //CompNameDic.TryAdd(typeof(Text), "Text");
                //CompNameDic.TryAdd(typeof(Button), "Button");
                //CompNameDic.TryAdd(typeof(PocoManager), "PocoManager");
                //CompNameDic.TryAdd(typeof(ApplicationManager), "ApplicationManager");
                CompNameDicInited = true;
            }
        }


        //public void GrabNode(GameObject obj)
        //{


        //    LogUtil.ULogDev("UnityNodeOptimized.ctor");

        //    gameObject = obj;
        //    camera = Camera.main;
        //    foreach (var cam in Camera.allCameras)
        //    {
        //        // skip the main camera
        //        // we want to use specified camera first then fallback to main camera if no other cameras
        //        // for further advanced cases, we could test whether the game object is visible within the camera
        //        if (cam == Camera.main)
        //        {
        //            continue;
        //        }
        //        if ((cam.cullingMask & (1 << gameObject.layer)) != 0)
        //        {
        //            camera = cam;
        //        }
        //    }

        //    uipanelComp = gameObject.GetComponent<UIPanel>();
        //    doInfo = gameObject.GetComponent<DisplayObjectInfo>();
        //    if (doInfo != null)
        //    {
        //        displayObject = doInfo.displayObject;
        //        gObject = displayObject.gOwner;
        //    }

        //    name = (gObject==null || string.IsNullOrEmpty(gObject.name)) ? gameObject.name : gObject.name;


        //    components = GameObjectAllComponents();
        //}

        //public override AbstractNode getParent()
        //{
        //    GameObject parentObj = gameObject.transform.parent.gameObject;
        //    return new UnityNode(parentObj);
        //}

        //public override List<AbstractNode> getChildren()
        //{
        //    List<AbstractNode> children = new List<AbstractNode>();

        //    if (gObject != null)
        //    {
        //        if (!(gObject is GComponent))
        //            return children;

        //        Container container = ((GComponent)gObject).container;
        //        int cnt = container.numChildren;
        //        for (int i = 0; i < cnt; i++)
        //            children.Add(new UnityNodeOptimized(container.GetChildAt(i).gameObject));
        //    }
        //    else
        //    {
        //        foreach (Transform child in gameObject.transform)
        //        {
        //            children.Add(new UnityNodeOptimized(child.gameObject));
        //        }
        //    }

        //    return children;
        //}

        public string GetName(GameObject go, DisplayObjectInfo doInfo)
        {
            string n;
            if (doInfo != null)
            {
                displayObject = doInfo.displayObject;
                gObject = displayObject.gOwner;

                if(gObject != null)
                {
                    n = string.IsNullOrEmpty(gObject.name) ? go.name : gObject.name;
                    return n;

                }
            }

            n = go.name;
            return n;

        }


        public object GetAttr(string attrName)
        {
            if (gObject != null)
            {
                switch (attrName)
                {
                    case "name":
                        return this.name;
                    case "type":
                        return gObject.GetType().Name.Substring(1);
                    case "visible":
                        return gObject.onStage && gObject.visible;
                    case "pos":
                        {
                            Vector2 vec2 = gObject.LocalToGlobal(Vector2.zero);

                            float[] floatArr = ArrPool_float.Ins.GetObj();
                            floatArr[0] = vec2.x / (float)Screen.width;
                            floatArr[1] = vec2.y / (float)Screen.height;
                            return floatArr;
                        }
                    case "size":
                        {
                            Rect rect = gObject.TransformRect(new Rect(0, 0, gObject.width, gObject.height), null);

                            float[] floatArr = ArrPool_float.Ins.GetObj();
                            floatArr[0] = rect.width / (float)Screen.width;
                            floatArr[1] = rect.height / (float)Screen.height;
                            return floatArr;
                        }
                    case "scale":
                        return floatOneOne;
                    case "anchorPoint":
                        {
                            float[] floatArr = ArrPool_float.Ins.GetObj();
                            floatArr[0] = gObject.pivotX;
                            floatArr[1] = gObject.pivotY;
                            return floatArr;
                        }
                    case "zOrders":
                        return GameObjectzOrders();
                    case "clickable":
                        return gObject.touchable;
                    case "text":
                        return gObject.text;
                    case "components":
                        return components;
                    case "texture":
                        return (gObject.displayObject != null && gObject.displayObject.graphics != null && gObject.displayObject.graphics.texture != null && gObject.displayObject.graphics.texture.nativeTexture != null) ? gObject.displayObject.graphics.texture.nativeTexture.name : null;
                    case "tag":
                        return GameObjectTag();
                    case "layer":
                        return GameObjectLayerName();
                    case "_ilayer":
                        return GameObjectLayer();
                    case "_instanceId":
                        return gameObject.GetInstanceID();
                    default:
                        return null;
                }
            }
            else
            {
                switch (attrName)
                {
                    case "name":
                        return this.name;
                    case "type":
                        return displayObject != null ? displayObject.GetType().Name : DefaultTypeName;
                    case "visible":
                        return GameObjectVisible(gameObject, doInfo, renderer, components);
                    case "pos":
                        return floatZeroZero;
                    case "size":
                        return floatZeroZero;
                    case "scale":
                        return floatOneOne;
                    case "anchorPoint":
                        return floatZeroZero;
                    case "zOrders":
                        {
                            Dictionary<string, float> zOrders = DicPoolSF2.Ins.GetObj();
                            zOrders["global"] = 0;
                            zOrders["local"] = 0;
                            return zOrders;
                        }
                    case "clickable":
                        return false;
                    case "components":
                        return components;
                    case "tag":
                        return GameObjectTag();
                    case "layer":
                        return GameObjectLayerName();
                    case "_ilayer":
                        return GameObjectLayer();
                    case "_instanceId":
                        return gameObject.GetInstanceID();
                    default:
                        return null;
                }
            }
        }

        private List<string> attrbutesNames = new List<string>
        {
            "name",
            "type",
            "visible",
            "pos",
            "size",
            "scale",
            "anchorPoint",
            "zOrders",
            "clickable",
            "text",
            "components",
            "texture",
            "tag",
            "_ilayer",
            "layer",
            "_instanceId"
        };

        public Dictionary<string, object> GetPayload(GameObject go,string name, List<string> components, DisplayObjectInfo doInfo, Renderer renderer)
        {

            LogUtil.ULogDev("UnityNodeOptimized.ctor");
            gObject = null;
            displayObject = null;


            this.renderer = renderer;
            gameObject = go;
            this.name = name;
            camera = Camera.main;
            foreach (var cam in allCams)
            {
                // skip the main camera
                // we want to use specified camera first then fallback to main camera if no other cameras
                // for further advanced cases, we could test whether the game object is visible within the camera
                if (cam == Camera.main)
                {
                    continue;
                }
                if ((cam.cullingMask & (1 << gameObject.layer)) != 0)
                {
                    camera = cam;
                }
            }

            //this.renderer = renderer;


            //if (components.Contains("RectTransform"))
            //{
            //    rectTransform = gameObject.GetComponent<RectTransform>();
            //}


            this.doInfo = doInfo;

            if (doInfo != null)
            {
                displayObject = doInfo.displayObject;
                gObject = displayObject.gOwner;
            }

            this.components = components;

            Dictionary<string, object> payload = DicPoolSO16.Ins.GetObj();

            foreach (string attrName in attrbutesNames)
            {

                if(Config.Instance.blockedAttributes != null && !Config.Instance.blockedAttributes.Contains(attrName))
                {
                    object attr = GetAttr(attrName);
                    if (attr != null)
                    {
                        payload[attrName] = attr;
                    }
                }
                else
                {
                    object attr = GetAttr(attrName);
                    if (attr != null)
                    {
                        payload[attrName] = attr;
                    }
                }


            }
            gameObject = null;
            return payload;
        }

        public static bool GameObjectVisible(GameObject go, DisplayObjectInfo doInfo, Renderer renderer, List<string> components)
        {
            DisplayObject dobj = null;
            GObject gobj = null;
            if (doInfo != null)
            {
                dobj = doInfo.displayObject;
                gobj = dobj.gOwner;
            }

            if (gobj != null)
            {
                return gobj.onStage && gobj.visible;
            }else if ( dobj != null)
            {
                return dobj.stage != null && dobj.visible;
            }else
            {
                bool result;
                if (go.activeInHierarchy)
                {
                    bool light = components.Contains("Light");
                    // bool mesh = components.Contains ("MeshRenderer") && components.Contains ("MeshFilter");
                    bool particle = components.Contains("ParticleSystem") && components.Contains("ParticleSystemRenderer");
                    if (light || particle)
                    {
                        result = false;
                    }
                    else
                    {
                        result = renderer ? renderer.isVisible : true;
                    }
                }
                else
                {
                    result = false;
                }
                return result;
            }


        }
        //public static bool GameObjectVisible(GameObject go)
        //{
        //    bool result;

        //    if (go.activeInHierarchy)
        //    {
        //        bool light = go.GetComponent<Light>() != null;
        //        // bool mesh = components.Contains ("MeshRenderer") && components.Contains ("MeshFilter");
        //        bool particle = go.GetComponent<ParticleSystem>() != null && go.GetComponent<ParticleSystemRenderer>() != null;
        //        if (light || particle)
        //        {
        //            result = false;
        //        }
        //        else
        //        {
        //            Renderer rdr = go.GetComponent<Renderer>();
        //            if (rdr != null)
        //                result = rdr.isVisible;
        //            else
        //                result = true;
        //        }
        //    }
        //    else
        //    {
        //        result = false;
        //    }
        //    UWASDKAgent.PopSample();
        //    return result;
        //}

        private int GameObjectLayer()
        {
            return gameObject.layer;
        }

        private string GameObjectLayerName()
        {
            return LayerMask.LayerToName(gameObject.layer);
        }

        private string GameObjectTag()
        {
            string tag;
            try
            {
                tag = !gameObject.CompareTag("Untagged") ? gameObject.tag : null;
            }
            catch (UnityException e)
            {
                tag = null;
            }
            return tag;
        }

        public List<string> GameObjectAllComponents(GameObject tmpGo)
        {
            //List<string> components = new List<string>();
            List<string> components = ListPool_str.Ins.GetObj();
            Component[] allComponents = tmpGo.GetComponents<Component>();
            if (allComponents != null)
            {
                foreach (Component ac in allComponents)
                {
                    if (ac != null)
                    {
                        Type tp = ac.GetType();
                        if (CompNameDic.ContainsKey(tp))
                        {
                            components.Add(CompNameDic[tp]);

                        }
                        else
                        {
                            string compName = tp.Name;
                            components.Add(compName);
                            CompNameDic[tp] = compName;
                        }
                        //components.Add(ac.name);
                    }
                }
            }
            return components;
        }
        private Dictionary<string, float> GameObjectzOrders()
        {
            float CameraViewportPoint = 0;
            if (camera != null)
            {
                CameraViewportPoint = Math.Abs(camera.WorldToViewportPoint(gameObject.transform.position).z);
            }

            Dictionary<string, float> zOrders = DicPoolSF2.Ins.GetObj();

            zOrders["global"] = 0f;
            zOrders["local"] = -1 * CameraViewportPoint;
            return zOrders;
        }

        public static bool SetText(GameObject go, string textVal)
        {
            if (go != null)
            {
                var info = go.GetComponent<DisplayObjectInfo>();
                if (info != null && info.displayObject.gOwner != null)
                {
                    info.displayObject.gOwner.text = textVal;
                    return true;
                }
            }
            return false;
        }

        //public override bool IsUINode()
        //{
        //    if (doInfo != null)
        //        return true;

        //    if (uipanelComp != null)
        //    {
        //        return true;
        //    }


        //    if (gameObject.layer == LayerMask.NameToLayer("UI"))
        //    {
        //        return true;
        //    }

        //    return false;
        //}


        public bool IsUIPanel()
        {
            if(gameObject.GetComponent<UIPanel>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUIPanel(GameObject go, List<string> components)
        {
            if (components.Contains("UIPanel"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<string, object> GetPayload(GameObject go, string name, List<string> components, Renderer renderer)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> GetPayload(GameObject go)
        {
            throw new NotImplementedException();
        }
    }

    static class GameObjectExtension
    {
        public static bool HasUIInChildren(this GameObject go)
        {
            Component[] comps = go.GetComponentsInChildren<UIPanel>();
            if (comps.Length > 0)
                return true;
            else
                return false;
        }
    }


}
