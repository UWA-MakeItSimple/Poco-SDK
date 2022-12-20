using System;
using System.Collections.Generic;
using UnityEngine;
using Poco.Utils;


namespace Poco
{
	public interface INodeGrabber
	{
		object GetAttr(string attrName);
		//void setAttr(string attrName, object val);

		string GetName(GameObject go);

		Dictionary<string, object> GetPayload(GameObject go, string name, List<string> components, Renderer renderer);
		Dictionary<string, object> GetPayload(GameObject go);

		//Dictionary<string, object> enumerateAttrs();
		bool IsUIPanel(GameObject go, List<string> components);
	}
	 
	public class RootNodeGrabber : Poco.Utils.Singleton<RootNodeGrabber>, INodeGrabber
	{

		static Dictionary<string, object> rootAttrs = new Dictionary<string, object>() {
			{ "name", "<Root>" },
			{ "type", "Root" },
			{ "visible", true },
			{ "pos", new float[2]{ 0.0f, 0.0f } },
			{ "size", new float[2]{ 0.0f, 0.0f } },
			{ "scale", new float[2]{ 1.0f, 1.0f } },
			{ "anchorPoint", new float[2]{ 0.5f, 0.5f } },
			{ "zOrders", new Dictionary<string, float> (){ { "global", 0 }, { "local", 0 } } }
		};

		//public Dictionary<string, object> enumerateAttrs()
		//{
		//	return rootAttrs;
		//}

        public object GetAttr(string attrName)
        {
            throw new NotImplementedException();
        }

        public string GetName(GameObject go)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> GetPayload(GameObject go)
        {
			return rootAttrs;
		}

        public Dictionary<string, object> GetPayload(GameObject go, string name,  List<string> components, Renderer renderer)
        {
            throw new NotImplementedException();
        }

        public bool IsUIPanel()
        {
            throw new NotImplementedException();
        }

        public bool IsUIPanel(GameObject go, List<string> components)
        {
            throw new NotImplementedException();
        }

        //public void setAttr(string attrName, object val)
        //{
        //    throw new NotImplementedException();
        //}

    }

}


