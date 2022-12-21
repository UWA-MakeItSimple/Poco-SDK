using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Poco.Utils;
//using UnityEngine.GUI
using UnityEngine.EventSystems;
namespace Poco
{
    public class UnityNodeGrabber : Poco.Utils.Singleton<UnityNodeGrabber> , INodeGrabber
    {
        public static Dictionary<string, string> TypeNames = new Dictionary<string, string>() {
            { "Text", "Text" },
            { "Gradient Text", "Gradient.Text" },
            { "Image", "Image" },
            { "RawImage", "Raw.Image" },
            { "Mask", "Mask" },
            { "2DRectMask", "2D-Rect.Mask" },
            { "Button", "Button" },
            { "InputField", "InputField" },
            { "Toggle", "Toggle" },
            { "Toggle Group", "ToggleGroup" },
            { "Slider", "Slider" },
            { "ScrollBar", "ScrollBar" },
            { "DropDown", "DropDown" },
            { "ScrollRect", "ScrollRect" },
            { "Selectable", "Selectable" },
            { "Camera", "Camera" },
            { "RectTransform", "Node" },
        };
        public static string DefaultTypeName = "GameObject";

        //for optimization
        static float[] scale = new float[2]{ 1.0f, 1.0f };

        private GameObject gameObject;
        private Renderer renderer;
        private RectTransform rectTransform;
        private Rect rect;
        private Vector2 objectPos;
        private List<string> components;
        private Camera camera;
        private Camera[] allCams;

        public bool protectedByParent;
        public string name;

        static bool CompNameDicInited = false;
        static Dictionary<Type, string> CompNameDic = new Dictionary<Type, string>();

        public UnityNodeGrabber()
        {

        }

        public void Init()
        {
            allCams = Camera.allCameras;

            if(CompNameDicInited == false)
            {
                //CompNameDic.TryAdd(typeof(Transform), "Transform");
                ////CompNameDic.Add(typeof(GUIWrapper), "GUIWrapper");
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
        //    gameObject = obj;
        //    name = obj.name;
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

        //    renderer = gameObject.GetComponent<Renderer>();
        //    rectTransform = gameObject.GetComponent<RectTransform>();
        //    rect = GameObjectRect(renderer, rectTransform);
        //    objectPos = renderer ? WorldToGUIPoint(camera, renderer.bounds.center) : Vector2.zero;
        //    components = GameObjectAllComponents();
        //}

        public object GetAttr(string attrName)
        {

            UWASDKAgent.PushSample("UNodeOptmzd.getAttr");
            object result;
            switch (attrName)
            {
                case "name":
                    result = name;
                    break;
                case "type":
                    result =  GuessObjectTypeFromComponentNames(components);
                    break;
                case "visible":
                    result = GameObjectVisible(gameObject, renderer, components);
                    break;
                case "pos":
                    result = GameObjectPosInScreen(objectPos, renderer, rectTransform, rect);
                    break;
                case "size":
                    result = GameObjectSizeInScreen(rect, rectTransform);
                    break;
                case "scale":
                    result = scale;
                    break;
                case "anchorPoint":
                    result = GameObjectAnchorInScreen(renderer, rect, objectPos);
                    break;

                case "zOrders":
                    result = GameObjectzOrders();
                    break;
                case "clickable":
                    result = GameObjectClickable(components);
                    break;
                case "text":
                    result = GameObjectText();
                    break;
                case "components":
                    result = components;
                    break;
                case "texture":
                    result = GetImageSourceTexture();
                    break;
                case "tag":
                    result = GameObjectTag();
                    break;
                case "layer":
                    result = GameObjectLayerName();
                    break;
                case "_ilayer":
                    result = GameObjectLayer();
                    break;
                case "_instanceId":
                    result = gameObject.GetInstanceID();
                    break;
                default:
                    result = null;
                    break;
            }
            UWASDKAgent.PopSample();
            return result;
        }


        //public Dictionary<string, object> enumerateAttrs()
        //{
        //    UWASDKAgent.PushSample("UNodeOptmzd.enumerateAttrs");

        //    Dictionary<string, object> payload = GetPayload();

        //    UWASDKAgent.PopSample();
        //    return payload;
        //}



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


        public Dictionary<string, object> GetPayload(GameObject go,string name, List<string> components, Renderer renderer)
        {

            UWASDKAgent.PushSample("UNodeOptmzd.GetPayload");

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

            this.renderer = renderer;

            if (components.Contains("RectTransform"))
            {
                rectTransform = gameObject.GetComponent<RectTransform>();
            }

            rect = GameObjectRect(renderer, rectTransform);
            objectPos = renderer ? WorldToGUIPoint(camera, renderer.bounds.center) : Vector2.zero;
            this.components = components;

            //Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> payload = DicPoolSO16.Ins.GetObj();

            foreach(var attrName in attrbutesNames)
            {
                if(!Config.Instance.blockedAttributes.Contains(attrName))
                {
                    object attr = GetAttr(attrName);
                    if(attr!=null)
                    {
                        payload[attrName] = attr;
                    }
                }
            }

          
            UWASDKAgent.PopSample();
            gameObject = null;
            return payload;
        }

        private string GuessObjectTypeFromComponentNames(List<string> components)
        {
            UWASDKAgent.PushSample("UNodeOptmzd.GuessObjectTypeFromComponentNames");


            //cns.Reverse();
            foreach (string name in components)
            {
                if (TypeNames.ContainsKey(name))
                {
                    return TypeNames[name];
                }
            }
            UWASDKAgent.PopSample();
            return DefaultTypeName;
        }


        public static bool GameObjectVisible(GameObject go)
        {
            bool result;

            if (go.activeInHierarchy)
            {
                bool light = go.GetComponent<Light>() != null;
                // bool mesh = components.Contains ("MeshRenderer") && components.Contains ("MeshFilter");
                bool particle = go.GetComponent<ParticleSystem>() != null && go.GetComponent<ParticleSystemRenderer>() != null ;
                if (light || particle)
                {
                    result = false;
                }
                else
                {
                    Renderer rdr = go.GetComponent<Renderer>();
                    if (rdr != null)
                        result = rdr.isVisible;
                    else
                        result = true;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }




        public static bool GameObjectVisible(GameObject go, Renderer renderer, List<string> components)
        {
            UWASDKAgent.PushSample("UNodeOptmzd.GameObjectVisible");
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
            UWASDKAgent.PopSample();
            return result;

        }

        private int GameObjectLayer()
        {
            return gameObject.layer;
        }

        private string GameObjectLayerName()
        {
            return LayerMask.LayerToName(gameObject.layer);
        }

        private bool GameObjectClickable(List<string> components)
        {
            if (components.Contains("Button"))
            {

                Button button = gameObject.GetComponent<Button>();
                return button ? button.isActiveAndEnabled : false;
            }else
            {
                return false;
            }

        }

        private string GameObjectText()
        {
            if(components.Contains("Text"))
            {
                Text text = gameObject.GetComponent<Text>();
                return text ? text.text : null;
            }
            else
            {
                return null;
            }


        }

        private string GameObjectTag()
        {
            string tag;
            try
            {
                tag = !gameObject.CompareTag("Untagged") ? gameObject.tag : null;
            }
            catch (UnityException)
            {
                tag = null;
            }
            return tag;
        }


        public static HashSet<string> compTypes = new HashSet<string>();
        public List<string> GameObjectAllComponents(GameObject tmpGo, out Component[] componetsArr)
        {
            //List<string> components = new List<string>();
            List<string> components = ListPool_str.Ins.GetObj();
            Component[] allComponents = tmpGo.GetComponents<Component>();
            componetsArr = allComponents;
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

//#if UWA_POCO_DEBUG || UNITY_EDITOR
//            string s = "";
//            foreach(var c in components)
//            {
//                s += c;
//            }
//            LogUtil.ULogDev(s);
//#endif
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

        private Rect GameObjectRect(Renderer renderer, RectTransform rectTransform)
        {
            Rect rect = new Rect(0, 0, 0, 0);
            if (renderer)
            {
                rect = RendererToScreenSpace(camera, renderer);
            }
            else if (rectTransform)
            {
                rect = RectTransformToScreenSpace(rectTransform);
            }
            return rect;
        }

        private float[] GameObjectPosInScreen(Vector3 objectPos, Renderer renderer, RectTransform rectTransform, Rect rect)
        {
            UWASDKAgent.PushSample("UNodeOptmzd.GameObjectPosInScreen");

            float[] pos = ArrPool_float.Ins.GetObj();

            if (renderer)
            {
                // 3d object
                pos[0] = objectPos.x / (float)Screen.width;
                pos[1] = objectPos.y / (float)Screen.height;
            }
            else if (rectTransform)
            {
                // ui object (rendered on screen space, other render modes may be different)
                // use center pos for now
                Canvas rootCanvas = GetRootCanvas(gameObject);
                RenderMode renderMode = rootCanvas != null ? rootCanvas.renderMode : new RenderMode();
                switch (renderMode)
                {
                    case RenderMode.ScreenSpaceCamera:
                        //上一个方案经过实际测试发现还有两个问题存在
                        //1.在有Canvas Scaler修改了RootCanvas的Scale的情况下坐标的抓取仍然不对，影响到了ScreenSpaceCameram模式在不同分辨率和屏幕比例下识别的兼容性。
                        //2.RectTransformUtility转的 rectTransform.transform.position本质上得到的是RectTransform.pivot中心轴在屏幕上的坐标，如果pivot不等于(0.5,0.5)，
                        //那么获取到的position就不等于图形的中心点。
                        //试了一晚上，找到了解决办法。

                        //用MainCanvas转一次屏幕坐标
                        Vector2 position = RectTransformUtility.WorldToScreenPoint(rootCanvas.worldCamera, rectTransform.transform.position);
                        //注意: 这里的position其实是Pivot点在Screen上的坐标，并不是图形意义上的中心点,在经过下列玄学公式换算才是真的图形中心在屏幕的位置。
                        //公式内算上了rootCanvas.scaleFactor 缩放因子，经测试至少在Canvas Scaler.Expand模式下，什么分辨率和屏幕比都抓的很准，兼容性很强，其他的有待测试。
                        //由于得出来的坐标是左下角为原点，触控输入是左上角为原点，所以要上下反转一下Poco才能用,所以y坐标用Screen.height减去。
                        position.Set(
                            position.x - rectTransform.rect.width * rootCanvas.scaleFactor * (rectTransform.pivot.x - 0.5f),
                            Screen.height - (position.y - rectTransform.rect.height * rootCanvas.scaleFactor * (rectTransform.pivot.y - 0.5f))
                            );
                        pos[0] = position.x / Screen.width;
                        pos[1] = position.y / Screen.height;
                        break;
                    case RenderMode.WorldSpace:
                        Vector2 _pos = RectTransformUtility.WorldToScreenPoint(rootCanvas.worldCamera, rectTransform.transform.position);
                        pos[0] = _pos.x / Screen.width;
                        pos[1] = (Screen.height - _pos.y) / Screen.height;
                        break;
                    default:
                        pos[0] = rect.center.x / (float)Screen.width;
                        pos[1] = rect.center.y / (float)Screen.height;
                        break;
                }
            }
            UWASDKAgent.PopSample();
            return pos;
        }

        private Canvas GetRootCanvas(GameObject gameObject)
        {
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();
            // 如果unity版本小于unity5.5，就用递归的方式取吧，没法直接取rootCanvas
            // 如果有用到4.6以下版本的话就自己手动在这里添加条件吧
#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			if (canvas && canvas.isRootCanvas) {
				return canvas;
			} else {
				if (gameObject.transform.parent.gameObject != null) {
					return GetRootCanvas(gameObject.transform.parent.gameObject);
				} else {
					return null;
				}
			}
#else
            if (canvas && canvas.isRootCanvas)
            {
                return canvas;
            }
            else if (canvas)
            {
                return canvas.rootCanvas;
            }
            else
            {
                return null;
            }
#endif
        }

        private float[] GameObjectSizeInScreen(Rect rect, RectTransform rectTransform)
        {
            UWASDKAgent.PushSample("UNodeOptmzd.GameObjectSizeInScreen");

            float[] size = ArrPool_float.Ins.GetObj();
            if (rectTransform)
            {
                Canvas rootCanvas = GetRootCanvas(gameObject);
                RenderMode renderMode = rootCanvas != null ? rootCanvas.renderMode : new RenderMode();
                switch (renderMode)
                {
                    case RenderMode.ScreenSpaceCamera:
                        Rect _rect = RectTransformUtility.PixelAdjustRect(rectTransform, rootCanvas);
                        size[0] = _rect.width * rootCanvas.scaleFactor / (float)Screen.width;
                        size[1] = _rect.height * rootCanvas.scaleFactor / (float)Screen.height;
                        //LogUtil.ULogDev("GameObjectSizeInScreen - ScreenSpaceCamera");
                        break;
                    case RenderMode.WorldSpace:
                        Rect rect_ = rectTransform.rect;
                        RectTransform canvasTransform = rootCanvas.GetComponent<RectTransform>();
                        //size = new float[] { rect_.width / canvasTransform.rect.width, rect_.height / canvasTransform.rect.height };
                        size[0] = rect_.width / canvasTransform.rect.width;
                        size[1] = rect_.height / canvasTransform.rect.height;
                        //LogUtil.ULogDev("GameObjectSizeInScreen - WorldSpace");
                        break;
                    default:
                        //LogUtil.ULogDev("GameObjectSizeInScreen - default");
                        size[0] = rect.width / (float)Screen.width;
                        size[1] = rect.height / (float)Screen.height;
                        break;
                }
            }
            else
            {
                //LogUtil.ULogDev("GameObjectSizeInScreen - else");

                size[0] = rect.width / (float)Screen.width;
                size[1] = rect.height / (float)Screen.height;
            }
            UWASDKAgent.PopSample();

            //LogUtil.ULogDev("GameObjectSizeInScreen: " + gameObject.name + ":  " + size[0] + ", " +size[1]);

            return size;
        }

        private float[] GameObjectAnchorInScreen(Renderer renderer, Rect rect, Vector3 objectPos)
        {
            float[] defaultValue = ArrPool_float.Ins.GetObj();
            defaultValue[0] = 0.5f;
            defaultValue[1] = 0.5f;
            if (rectTransform)
            {
                Vector2 data = rectTransform.pivot;
                defaultValue[0] = data[0];
                defaultValue[1] = 1 - data[1];
                return defaultValue;
            }
            if (!renderer)
            {
                //<Modified> some object do not have renderer
                return defaultValue;
            }
            float f0 = (objectPos.x - rect.xMin) / rect.width;
            float f1 = (objectPos.y - rect.yMin) / rect.height; 
            if (Double.IsNaN(f0) || Double.IsNaN(f1))
            {
                return defaultValue;
            }
            else if (Double.IsPositiveInfinity(f0) || Double.IsPositiveInfinity(f1))
            {
                return defaultValue;
            }
            else if (Double.IsNegativeInfinity(f0) || Double.IsNegativeInfinity(f1))
            {
                return defaultValue;
            }
            else
            {
                float[] anchor = ArrPool_float.Ins.GetObj();
                anchor[0] = f0;
                anchor[1] = f1;
                return anchor;
            }
        }

        HashSet<string> ImageComps = new HashSet<string>() { "Image", "RawImage", "SpriteRenderer", "Renderer" };
        private string GetImageSourceTexture()
        {
            //bool hasImage = false;
            //foreach(var comp in components)
            //{
            //    if (ImageComps.Contains(comp))
            //        hasImage = true;
            //}


            //if (!hasImage) return null;

            if (components.Contains("Image"))
            {
                Image image = gameObject.GetComponent<Image>();
                if (image != null && image.sprite != null)
                {
                    return image.sprite.name;
                }
            }


            if (components.Contains("RawImage"))
            {
                RawImage rawImage = gameObject.GetComponent<RawImage>();
                if (rawImage != null && rawImage.texture != null)
                {
                    return rawImage.texture.name;
                }
            }

            if (components.Contains("SpriteRenderer"))
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    return spriteRenderer.sprite.name;
                }
            }

            //if (components.Contains("Renderer"))
            //{
            //    Renderer render = gameObject.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                return renderer.material.color.ToString();
            }
            //}

            return null;
        }

        protected static Vector2 WorldToGUIPoint(Camera camera, Vector3 world)
        {
            Vector2 screenPoint = Vector2.zero;
            if (camera != null)
            {
                screenPoint = camera.WorldToScreenPoint(world);
                screenPoint.y = (float)Screen.height - screenPoint.y;
            }
            return screenPoint;
        }

        protected static Rect RendererToScreenSpace(Camera camera, Renderer renderer)
        {
            Vector3 cen = renderer.bounds.center;
            Vector3 ext = renderer.bounds.extents;
            Vector2[] extentPoints = new Vector2[8] {
                WorldToGUIPoint (camera, new Vector3 (cen.x - ext.x, cen.y - ext.y, cen.z - ext.z)),
                WorldToGUIPoint (camera, new Vector3 (cen.x + ext.x, cen.y - ext.y, cen.z - ext.z)),
                WorldToGUIPoint (camera, new Vector3 (cen.x - ext.x, cen.y - ext.y, cen.z + ext.z)),
                WorldToGUIPoint (camera, new Vector3 (cen.x + ext.x, cen.y - ext.y, cen.z + ext.z)),
                WorldToGUIPoint (camera, new Vector3 (cen.x - ext.x, cen.y + ext.y, cen.z - ext.z)),
                WorldToGUIPoint (camera, new Vector3 (cen.x + ext.x, cen.y + ext.y, cen.z - ext.z)),
                WorldToGUIPoint (camera, new Vector3 (cen.x - ext.x, cen.y + ext.y, cen.z + ext.z)),
                WorldToGUIPoint (camera, new Vector3 (cen.x + ext.x, cen.y + ext.y, cen.z + ext.z))
            };
            Vector2 min = extentPoints[0];
            Vector2 max = extentPoints[0];
            foreach (Vector2 v in extentPoints)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        protected static Rect RectTransformToScreenSpace(RectTransform rectTransform)
        {
            Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
            Rect rect = new Rect(rectTransform.position.x, Screen.height - rectTransform.position.y, size.x, size.y);
            rect.x -= (rectTransform.pivot.x * size.x);
            rect.y -= ((1.0f - rectTransform.pivot.y) * size.y);
            return rect;
        }

        public static bool SetText(GameObject go, string textVal)
        {
            if (go != null)
            {
                var inputField = go.GetComponent<InputField>();
                if (inputField != null)
                {
                    inputField.text = textVal;
                    return true;
                }
            }
            return false;
        }
        //public override bool IsUINode()
        //{

        //    if (gameObject.layer == LayerMask.NameToLayer("UI"))
        //    {
        //        Debug.Log("LayerUI:  " + gameObject.name);
        //        return true;
        //    }

        //    if(gameObject.GetComponent<RectTransform>()!=null)
        //    {
        //        return true;
        //    }


        //    return false;
        //}


        public bool IsUIPanel(GameObject go,List<string> components)
        {


            if (components.Contains("Canvas"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<string, object> GetPayload(GameObject go)
        {
            throw new NotImplementedException();
        }

        public string GetName(GameObject go)
        {
            return go.name;
        }
    }

    static class GameObjectExtension
    {
        public static bool HasUIInChildren(this GameObject go)
        {
            Component[] comps = go.GetComponentsInChildren<Canvas>();
            if (comps.Length > 0)
                return true;
            else
                return false;
        }
    }

}
