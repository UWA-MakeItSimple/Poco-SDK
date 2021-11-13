using System;
using System.Collections.Generic;
using UnityEngine;
using Poco.Utils;


namespace Poco
{
	public interface INodeGrabber
	{

		object getAttr(string attrName);
		void setAttr(string attrName, object val);

		Dictionary<string, object> enumerateAttrs();

		bool IsUIPanel(GameObject go);
	}
	 



	public class RootNodeGrabber : Singleton<RootNodeGrabber>, INodeGrabber
	{

		static Dictionary<string, object> rootAttrs = new Dictionary<string, object>() {
			{ "name", "<Root>" },
			{ "type", "Root" },
			{ "visible", true },
			{ "pos", new List<float> (){ 0.0f, 0.0f } },
			{ "size", new List<float> (){ 0.0f, 0.0f } },
			{ "scale", new List<float> (){ 1.0f, 1.0f } },
			{ "anchorPoint", new List<float> (){ 0.5f, 0.5f } },
			{ "zOrders", new Dictionary<string, object> (){ { "local", 0 }, { "global", 0 } } }
		};



		public Dictionary<string, object> enumerateAttrs()
		{
			return rootAttrs;
		}


        public object getAttr(string attrName)
        {
            throw new NotImplementedException();
        }

        public bool IsUIPanel()
        {
            throw new NotImplementedException();
        }

        public bool IsUIPanel(GameObject go)
        {
            throw new NotImplementedException();
        }

        public void setAttr(string attrName, object val)
        {
            throw new NotImplementedException();
        }


        //public virtual bool IsUINode()
        //{

        //	throw new NotImplementedException();
        //}

    }

}


