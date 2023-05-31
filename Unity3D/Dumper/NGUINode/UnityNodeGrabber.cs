using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Poco.Utils;


namespace Poco
{
    public class UnityNodeGrabber : Poco.Utils.Singleton<UnityNodeGrabber>, INodeGrabber
    {
        public static Dictionary<string, string> TypeNames = new Dictionary<string, string>() {
            { "UI2DSprite", "UI2DSprite" },
            { "UI2DSpriteAnimation", "UI2DSpriteAnimation" },
            { "UIAnchor", "UIAnchor" },
            { "UIAtlas", "UIAtlas" },
            { "UICamera", "UICamera" },
            { "UIFont", "UIFont" },
            { "UIInput", "UIInput" },
            { "UILabel", "UILabel" },
            { "UILocalize", "UILocalize" },
            { "UIOrthoCamera", "UIOrthoCamera" },
            { "UIPanel", "UIPanel" },
            { "UIRoot", "UIRoot" },
            { "UISprite", "UISprite" },
            { "UISpriteAnimation", "UISpriteAnimation" },
            { "UISpriteData", "UISpriteData" },
            { "UIStretch", "UIStretch" },
            { "UITextList", "UITextList" },
            { "UITexture", "UITexture" },
            { "UITooltip", "UITooltip" },
            { "UIViewport", "UIViewport" },
            { "Camera", "Camera" },
            { "Transform", "Node" },
        };
        public static string DefaultTypeName = "GameObject";
        private GameObject gameObject;
        private Camera camera;
        private Camera[] allCams;

        private Bounds bounds;
        private Rect rect;
        private Vector2 objectPos;
        private List<string> components;

        static float[] floatOneOne = new float[2] { 1.0f, 1.0f };
        static float[] floatZeroZero = new float[2] { 0.0f, 0.0f };

        public string name;
        public bool protectedByParent = false;

        static bool CompNameDicInited = false;
        static Dictionary<Type, string> CompNameDic = new Dictionary<Type, string>();
        public UnityNodeGrabber()
        {

        }
        public void Init()
        {
            allCams = Camera.allCameras;

            if (CompNameDicInited == false)
            {
                //CompNameDic.TryAdd(typeof(Transform), "Transform");
                //CompNameDic.TryAdd(typeof(Camera), "Camera");

                CompNameDicInited = true;
            }
        }
        //public void GrabNode(GameObject obj)
        //{
        //    gameObject = obj;
        //    name = obj.name;
        //    camera = GetCamera();
        //    bounds = NGUIMath.CalculateAbsoluteWidgetBounds(gameObject.transform);
        //    rect = BoundsToScreenSpace(bounds);
        //    objectPos = WorldToGUIPoint(bounds.center);
        //    components = GameObjectAllComponents();
        //}

        //public override AbstractNode getParent()
        //{
        //    GameObject parentObj = gameObject.transform.parent.gameObject;
        //    return new UnityNodeOptimized(parentObj);
        //}

        //public override List<AbstractNode> getChildren()
        //{
        //    List<AbstractNode> children = new List<AbstractNode>();
        //    foreach (Transform child in gameObject.transform)
        //    {
        //        children.Add(new UnityNodeOptimized(child.gameObject));
        //    }
        //    return children;
        //}

        public string GetName(GameObject go)
        {
            return go.name;
        }

        public object GetAttr(string attrName)
        {
            switch (attrName)
            {
                case "name":
                    return name;
                case "type":
                    return GuessObjectTypeFromComponentNames(components);
                case "visible":
                    return GameObjectVisible(components);
                case "pos":
                    return GameObjectPosInScreen(objectPos);
                case "size":
                    return GameObjectSizeInScreen(rect);
                case "scale":
                    return floatOneOne;
                case "anchorPoint":
                    return GameObjectAnchorInScreen(rect, objectPos);
                case "zOrders":
                    return GameObjectzOrders();
                case "clickable":
                    return GameObjectClickable();
                case "text":
                    return GameObjectText();
                case "components":
                    return components;
                case "texture":
                    return GetImageSourceTexture();
                case "tag":
                    return GameObjectTag();
                case "_instanceId":
                    return gameObject.GetInstanceID();
                default:
                    return null;
            }

        }

        //public override Dictionary<string, object> enumerateAttrs()
        //{
        //    Dictionary<string, object> payload = GetPayload();
        //    Dictionary<string, object> ret = new Dictionary<string, object>();
        //    foreach (KeyValuePair<string, object> p in payload)
        //    {
        //        if (p.Value != null)
        //        {
        //            ret.Add(p.Key, p.Value);
        //        }
        //    }
        //    return ret;
        //}

        private Camera GetCamera()
        {
			// find the right camera when there are more than one UICamera

            // find the UIRoot it belongs to
			UIRoot root = this.gameObject.GetComponentInParent<UIRoot>();
            if (root != null)
            {
                UICamera[] uicams = root.gameObject.GetComponentsInChildren<UICamera>();

                // only one UICamera under UIRoot
                if (uicams.Length == 1 && uicams[0].cachedCamera != null)
                {
					return uicams[0].cachedCamera;
                }

                // more than one UICamera under UIRoot, select according to the layerMask
				if (uicams.Length > 1)
				{
					for (int i = 0; i < uicams.Length; i++)
					{
						if (uicams[i].cachedCamera != null && (uicams[i].cachedCamera.cullingMask & (1 << this.gameObject.layer) ) != 0)
						{
							return uicams[i].cachedCamera;
						}
					}
				}
            }
			
            // it seems that NGUI has it own camera culling mask.
            // so we don't need to test within which camera a game object is visible
            return UICamera.currentCamera != null ? UICamera.currentCamera : UICamera.mainCamera;
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
            "_instanceId"
        };


        public Dictionary<string, object>GetPayload(GameObject go, string name, List<string> components, Renderer renderer)
        {

            gameObject = go;
            this.name = name;
            camera = GetCamera();
            bounds = NGUIMath.CalculateAbsoluteWidgetBounds(gameObject.transform);
            rect = BoundsToScreenSpace(bounds, renderer);
            objectPos = WorldToGUIPoint(bounds.center);
            this.components = components;

            Dictionary<string, object> payload = DicPoolSO16.Ins.GetObj();

            foreach (var attrName in attrbutesNames)
            {
                if (!Config.Instance.blockedAttributes.Contains(attrName))
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

        private string GuessObjectTypeFromComponentNames(List<string> components)
        {

            for (int i = components.Count - 1; i >= 0; i--)
            {
                string tmp = components[i];
                if (TypeNames.ContainsKey(tmp))
                {
                    return TypeNames[tmp];
                }
            }

            return DefaultTypeName;
        }

        private bool GameObjectVisible(List<string> components)
        {
            if (gameObject.activeInHierarchy)
            {
                bool drawcall = components.Contains("UIDrawCall");
                bool light = components.Contains("Light");
                // bool mesh = components.Contains ("MeshRenderer") && components.Contains ("MeshFilter");
                bool particle = components.Contains("ParticleSystem") && components.Contains("ParticleSystemRenderer");
                return drawcall || light || particle ? false : true;
            }
            else
            {
                return false;
            }
        }

        public static bool GameObjectVisible(GameObject go, Renderer renderer, List<string> components)
        {
            bool result;

            if (go.activeInHierarchy)
            {
                

                bool drawcall = components.Contains("UIDrawCall");

                bool light = components.Contains("Light");
                //bool light = go.GetComponent<Light>() != null;
                // bool mesh = components.Contains ("MeshRenderer") && components.Contains ("MeshFilter");
                bool particle = components.Contains("ParticleSystem") && components.Contains("ParticleSystemRenderer");
                if (light || particle || drawcall)
                {
                    result = false;
                }
                else
                {
                    if (renderer != null)
                        result = renderer.isVisible;
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





        private bool GameObjectClickable()
        {
            if (components.Contains("UIButton"))
            {
                UIButton button = gameObject.GetComponent<UIButton>();

                if (components.Contains("BoxCollider"))
                {
                    BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
                    return button && button.isEnabled && boxCollider ? true : false;
                }
            }

            return false;

        }

        private string GameObjectText()
        {
			UILabel text = gameObject.GetComponent<UILabel>();
			return text ? text.text : null;

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

        public List<string> GameObjectAllComponents(GameObject tmpGo, out Component[] componetsArr)
        {
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

        private float[] GameObjectPosInScreen(Vector2 objectPos)
        {
            float[] pos = ArrPool_float.Ins.GetObj();
            pos[0] = objectPos.x / (float)Screen.width;
            pos[1] = objectPos.y / (float)Screen.height;
            return pos;
        }

        private float[] GameObjectSizeInScreen(Rect rect)
        {
            float[] size = ArrPool_float.Ins.GetObj();
            size[0] = rect.width / (float)Screen.width;
            size[1] = rect.height / (float)Screen.height;
            return size;
        }

        private float[] GameObjectAnchorInScreen(Rect rect, Vector2 objectPos)
        {
            float[] defaultValue = ArrPool_float.Ins.GetObj();
            defaultValue[0] = 0.5f;
            defaultValue[1] = 0.5f;

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

        private string GetImageSourceTexture()
        {

            if (components.Contains("UISprite"))
            {
                UISprite sprite = gameObject.GetComponent<UISprite>();
                if (sprite != null)
                {
                    return sprite.spriteName;
                }
            }

            if (components.Contains("UITexture"))
            {
                UITexture texture = gameObject.GetComponent<UITexture>();
                if (texture != null && texture.mainTexture != null)
                {
                    return texture.mainTexture.name;
                }
            }

            return null;
        }

        static Vector2[] extentPoints = new Vector2[8];
        private Rect BoundsToScreenSpace(Bounds bounds, Renderer renderer)
        {
            Vector3 cen;
            Vector3 ext;
            cen = renderer ? renderer.bounds.center : bounds.center;
            ext = renderer ? renderer.bounds.extents : bounds.extents;

            extentPoints[0] = WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z));
            extentPoints[1] = WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z));
            extentPoints[2] = WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z));
            extentPoints[3] = WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z));
            extentPoints[4] = WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z));
            extentPoints[5] = WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z));
            extentPoints[6] = WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z));
            extentPoints[7] = WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z));

            Vector2 min = extentPoints[0];
            Vector2 max = extentPoints[0];
            foreach (Vector2 v in extentPoints)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        private Vector2 WorldToGUIPoint(Vector3 world)
        {
            Vector2 screenPoint = Vector2.zero;
            if (camera != null)
            {
                screenPoint = camera.WorldToScreenPoint(world);
                screenPoint.y = (float)Screen.height - screenPoint.y;
            }
            return screenPoint;
        }

        public static bool SetText(GameObject go, string textVal)
        {
            if (go != null)
            {
                var inputField = go.GetComponent<UIInput>();
                if (inputField != null)
                {
                    // 这一行未测试，给输入框设置文本
                    inputField.text = textVal;
                    return true;
                }
            }
            return false;
        }

        //public override bool IsUINode()
        //{


        //    if (gameObject.GetComponent<RectTransform>() != null)
        //    {
        //        return true;
        //    }

        //    if (gameObject.layer == LayerMask.NameToLayer("UI"))
        //    {
        //        Debug.Log("LayerUI:  " + gameObject.name);
        //        return true;
        //    }


        //    return false;
        //}

        public bool IsUIPanel(GameObject go)
        {
            if (go.GetComponent<UIPanel>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public bool IsUIPanel()
        //{
        //    if (gameObject.GetComponent<UIPanel>() != null)
        //    {
        //        return true;

        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        public Dictionary<string, object> GetPayload(GameObject go)
        {
            throw new NotImplementedException();
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
