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


		Dictionary<string, object> GetPayload();

		//Dictionary<string, object> enumerateAttrs();
		bool IsUIPanel(GameObject go);
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
			{ "zOrders", new Dictionary<string, float> (){ { "local", 0 }, { "global", 0 } } }
		};

		//public Dictionary<string, object> enumerateAttrs()
		//{
		//	return rootAttrs;
		//}

        public object GetAttr(string attrName)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> GetPayload()
        {
			return rootAttrs;
		}

		public bool IsUIPanel()
        {
            throw new NotImplementedException();
        }

        public bool IsUIPanel(GameObject go)
        {
            throw new NotImplementedException();
        }

        //public void setAttr(string attrName, object val)
        //{
        //    throw new NotImplementedException();
        //}

    }

}


