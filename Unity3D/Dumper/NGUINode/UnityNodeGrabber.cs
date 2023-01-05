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
        private Bounds bounds;
        private Rect rect;
        private Vector2 objectPos;
        private List<string> components;

        public string name;
        public bool protectedByParent = false;

        public UnityNodeGrabber()
        {

        }

        public void GrabNode(GameObject obj)
        {
            gameObject = obj;
            name = obj.name;
            camera = GetCamera();
            bounds = NGUIMath.CalculateAbsoluteWidgetBounds(gameObject.transform);
            rect = BoundsToScreenSpace(bounds);
            objectPos = WorldToGUIPoint(bounds.center);
            components = GameObjectAllComponents();
        }

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

        public object GetAttr(string attrName)
        {
            switch (attrName)
            {
                case "name":
                    return gameObject.name;
                case "type":
                    return GuessObjectTypeFromComponentNames(components);
                case "visible":
                    return GameObjectVisible(components);
                case "pos":
                    return GameObjectPosInScreen(objectPos);
                case "size":
                    return GameObjectSizeInScreen(rect);
                case "scale":
                    return new List<float>() { 1.0f, 1.0f };
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


        public Dictionary<string, object> GetPayload()
        {

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

            return payload;
        }

        private string GuessObjectTypeFromComponentNames(List<string> components)
        {
            List<string> cns = new List<string>(components);
            cns.Reverse();
            foreach (string name in cns)
            {
                if (TypeNames.ContainsKey(name))
                {
                    return TypeNames[name];
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

        public static bool GameObjectVisible(GameObject go)
        {
            bool result;

            if (go.activeInHierarchy)
            {
                bool drawcall = go.GetComponent<UIDrawCall>() != null;

                bool light = go.GetComponent<Light>() != null;
                // bool mesh = components.Contains ("MeshRenderer") && components.Contains ("MeshFilter");
                bool particle = go.GetComponent<ParticleSystem>() != null && go.GetComponent<ParticleSystemRenderer>() != null;
                if (light || particle || drawcall)
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





        private bool GameObjectClickable()
        {
            UIButton button = gameObject.GetComponent<UIButton>();
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            return button && button.isEnabled && boxCollider ? true : false;
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

        private List<string> GameObjectAllComponents()
        {
            List<string> components = new List<string>();
            Component[] allComponents = gameObject.GetComponents<Component>();
            if (allComponents != null)
            {
                foreach (Component ac in allComponents)
                {
                    if (ac != null)
                    {
                        components.Add(ac.GetType().Name);
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
            Dictionary<string, float> zOrders = new Dictionary<string, float>() {
                { "global", 0f },
                { "local", -1 * CameraViewportPoint }
            };
            return zOrders;
        }

        private float[] GameObjectPosInScreen(Vector2 objectPos)
        {
            float[] pos = { objectPos.x / (float)Screen.width, objectPos.y / (float)Screen.height };
            return pos;
        }

        private float[] GameObjectSizeInScreen(Rect rect)
        {
            float[] size = { rect.width / (float)Screen.width, rect.height / (float)Screen.height };
            return size;
        }

        private float[] GameObjectAnchorInScreen(Rect rect, Vector2 objectPos)
        {
            float[] defaultValue = { 0.5f, 0.5f };
            float[] anchor = { (objectPos.x - rect.xMin) / rect.width, (objectPos.y - rect.yMin) / rect.height };
            if (Double.IsNaN(anchor[0]) || Double.IsNaN(anchor[1]))
            {
                return defaultValue;
            }
            else if (Double.IsPositiveInfinity(anchor[0]) || Double.IsPositiveInfinity(anchor[1]))
            {
                return defaultValue;
            }
            else if (Double.IsNegativeInfinity(anchor[0]) || Double.IsNegativeInfinity(anchor[1]))
            {
                return defaultValue;
            }
            else
            {
                return anchor;
            }
        }

        private string GetImageSourceTexture()
        {
            UISprite sprite = gameObject.GetComponent<UISprite>();
            if (sprite != null)
            {
                return sprite.spriteName;
            }

            UITexture texture = gameObject.GetComponent<UITexture>();
            if (texture != null && texture.mainTexture != null)
            {
                return texture.mainTexture.name;
            }

            return null;
        }

        private Rect BoundsToScreenSpace(Bounds bounds)
        {
            Vector3 cen;
            Vector3 ext;
            Renderer renderer = gameObject.GetComponent<Renderer>();
            cen = renderer ? renderer.bounds.center : bounds.center;
            ext = renderer ? renderer.bounds.extents : bounds.extents;
            Vector2[] extentPoints = new Vector2[8] {
                WorldToGUIPoint (new Vector3 (cen.x - ext.x, cen.y - ext.y, cen.z - ext.z)),
                WorldToGUIPoint (new Vector3 (cen.x + ext.x, cen.y - ext.y, cen.z - ext.z)),
                WorldToGUIPoint (new Vector3 (cen.x - ext.x, cen.y - ext.y, cen.z + ext.z)),
                WorldToGUIPoint (new Vector3 (cen.x + ext.x, cen.y - ext.y, cen.z + ext.z)),
                WorldToGUIPoint (new Vector3 (cen.x - ext.x, cen.y + ext.y, cen.z - ext.z)),
                WorldToGUIPoint (new Vector3 (cen.x + ext.x, cen.y + ext.y, cen.z - ext.z)),
                WorldToGUIPoint (new Vector3 (cen.x - ext.x, cen.y + ext.y, cen.z + ext.z)),
                WorldToGUIPoint (new Vector3 (cen.x + ext.x, cen.y + ext.y, cen.z + ext.z))
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

        public bool IsUIPanel()
        {
            if (gameObject.GetComponent<UIPanel>() != null)
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
