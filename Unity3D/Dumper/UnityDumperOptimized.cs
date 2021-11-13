using System;
using System.Collections.Generic;
using UnityEngine;
using Poco.Utils;

namespace Poco
{
    public class UnityDumperOptimized : Singleton<UnityDumperOptimized>, IDumper<GameObject>
    {

        //List<GameObject> firstLvlNodes;


        public UnityDumperOptimized()
        {
            //firstLvlNodes = new List<GameObject>(30);
        }


        //protected override Dictionary<string, object> dfsDump(AbstractNode node, bool onlyVisibleNode)
        //{
        //    UWASDKAgent.PushSample("UDmpOptmzd.dumpHImpl(node,only)");
        //    LogUtil.ULogDev("UnityDmpOptimized.dumpHierarchyImpl");

        //    if (node == null)
        //    {
        //        return null;
        //    }

        //    Dictionary<string, object> payload = node.enumerateAttrs();
        //    Dictionary<string, object> result = new Dictionary<string, object>();
        //    string name = (string)node.getAttr("name");
        //    result.Add("name", name);
        //    result.Add("payload", payload);

        //    List<object> children = new List<object>();
        //    foreach (AbstractNode child in node.getChildren())
        //    {
        //        if (!onlyVisibleNode || (bool)child.getAttr("visible"))
        //        {
        //            var childResult = dfsDump(child, onlyVisibleNode, false);
        //            if (childResult != null)
        //                children.Add(childResult);
        //        }
        //    }
        //    if (children.Count > 0)
        //    {
        //        result.Add("children", children);
        //    }
        //    UWASDKAgent.PopSample();
        //    return result;
        //}

        public Dictionary<string, object> DumpHierarchy(bool onlyVisibleNode)
        {
            List<GameObject> firstLevelNodes = GetFirstLevelNodes();


            //CreateRoot

            Dictionary<string, object> payload = RootNodeGrabber.Instance.enumerateAttrs();
            //Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, object> result = DicPoolSO3.Ins.GetObj();
            object name = "";
            payload.TryGetValue("name", out name);
            result["name"] = name;
            result["payload"] = payload;


            int depth = 0;
            //List<object> children = new List<object>();
            List<object> children = ListPool_object.Ins.GetObj();
            foreach (GameObject go in firstLevelNodes)
            {

                var childResult = dfsDump(go, onlyVisibleNode, false, depth);

                if (childResult != null)
                    children.Add(childResult);
            }

            if (children.Count > 0)
            {
                result["children"] = children;
            }

            return result;
        }
        //DumpHierarchy called by RootNode
        //protected override Dictionary<string, object> dfsDump(AbstractNode node, bool onlyVisibleNode)
        //{
        //    UWASDKAgent.PushSample("UDmpOptmzd.dumpHImpl(node,only)");
        //    LogUtil.ULogDev("UnityDmpOptimized.dumpHierarchyImpl");

        //    if (node == null)
        //    {
        //        return null;
        //    }

        //    Dictionary<string, object> payload = node.enumerateAttrs();
        //    Dictionary<string, object> result = new Dictionary<string, object>();
        //    string name = (string)node.getAttr("name");
        //    result.Add("name", name);
        //    result.Add("payload", payload);

        //    List<object> children = new List<object>();
        //    foreach (AbstractNode child in node.getChildren())
        //    {
        //        if (!onlyVisibleNode || (bool)child.getAttr("visible"))
        //        {
        //            var childResult = dfsDump(child, onlyVisibleNode, false);
        //            if (childResult != null)
        //                children.Add(childResult);
        //        }
        //    }
        //    if (children.Count > 0)
        //    {
        //        result.Add("children", children);
        //    }
        //    UWASDKAgent.PopSample();
        //    return result;
        //}

        protected Dictionary<string, object> dfsDump(GameObject go, bool onlyVisibleNode, bool protectedByParent, int depth)
        {
            UWASDKAgent.PushSample("UDmpOptmzd.dumpHImpl(node,only,pBP)");

            LogUtil.ULogDev("UnityDmpOptimized.dumpHierarchyImpl");

            if (go == null)
            {
                return null;
            }

            bool protectChildren = false;

            UWASDKAgent.PushSample("UDmpOptmzd.Filter");
            bool shouldVisit = Filter(go, protectedByParent, out protectChildren, depth);
            UWASDKAgent.PopSample();

            if (!shouldVisit)
                return null;

            //List<object> children = new List<object>();
            List<object> children = ListPool_object.Ins.GetObj();

            depth++;

            foreach (Transform trans in go.transform)
            {
                GameObject child = trans.gameObject;
                if (!onlyVisibleNode || UnityNodeGrabberOptimized.GameObjectVisible(child) )
                {
                    var childResult = dfsDump(child, onlyVisibleNode, protectChildren, depth);
                    if (childResult != null)
                        children.Add(childResult);
                }
            }


            UnityNodeGrabberOptimized.Instance.GrabNode(go);
            Dictionary<string, object> payload = UnityNodeGrabberOptimized.Instance.enumerateAttrs();
            string name = (string)UnityNodeGrabberOptimized.Instance.getAttr("name");


            //Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, object> result = DicPoolSO3.Ins.GetObj();

            result["name"] = name;
            result["payload"] = payload;

            if (children.Count > 0)
            {
                result["children"] = children;
            }
            UWASDKAgent.PopSample();
            return result;
        }

        /// <summary>
        /// Return false if node should be pruned
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool Filter(GameObject go,bool protectedByParent, out bool protectChildren, int depth)
        {

            string name = go.name;

            //Strong protect judge
            //---------------------------------------------------------------------

            UWASDKAgent.PushSample("UDmpOptmzd.StringProtect");
            bool tag1 = StrongProtect(go, name);
            UWASDKAgent.PopSample();

            if (tag1)
            {
                protectChildren = true;
                return true;
            }
            else
            {
                if (protectedByParent)
                    protectChildren = true;
                else
                    protectChildren = false;
            }

            //---------------------------------------------------------------------
            //Weak protection judge
            UWASDKAgent.PushSample("UDmpOptmzd.WeakProtect");
            bool tag2 = WeakProtect(go, name, protectedByParent, depth);
            UWASDKAgent.PopSample();

            if (tag2) {

                UWASDKAgent.PushSample("UDmpOptmzd.BlackListContain");
                bool tag3 = BlackListContain(name);
                UWASDKAgent.PopSample();

                // String black list
                if (tag3)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                //Weak black list
                return false;
            }
            //---------------------------------------------------------------------
        }


        /// <summary>
        /// WhiteList check
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool StrongProtect(GameObject go, string name)
        {
            if (UnityNodeGrabberOptimized.Instance.IsUIPanel(go))
            {
                return true;
            }

            if (Config.Instance.strongWhiteList.Contains(name))
            {
                return true;
            }
            return false;
        }

        private bool WeakProtect(GameObject go, string name, bool protectedByParent, int depth)
        {

            if (depth == 0)
                return true;

            if (protectedByParent)
                return true;

            if (Config.Instance.weakWhiteList.Contains(name))
            {
                return true;
            }

            //if (((UnityNodeOptimized)node).IsUINode())
            //{
            //    return true;
            //}
            return false;
        }

        private bool BlackListContain(string name)
        {
            if (Config.Instance.blackList.Contains(name))
                return true;
            else
                return false;
        }

        public virtual List<float> getPortSize()
        {
            return null;
        }

        //public AbstractNode GetRoot()
        //{
        //    LogUtil.ULogDev("UnityDumperOptimized.getRoot");
        //    return new RootNodeOptimized();
        //}

        public List<GameObject> GetFirstLevelNodes()
        {

            //List<GameObject> firstLvlNodes = new List<GameObject>();
            List<GameObject> firstLvlNodes = ListPool_GameObject.Ins.GetObj();


            foreach (GameObject obj in Transform.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.transform.parent == null)
                {
                    firstLvlNodes.Add(obj);
                }
            }
            return firstLvlNodes;
        }

        public List<float> GetPortSize()
        {
            throw new NotImplementedException();
        }


    }

    //public class RootNodeOptimized : AbstractNode
    //{
    //    //private List<AbstractNode> children = null;

    //    public RootNodeOptimized()
    //    {
    //        UWASDKAgent.PushSample("RootNodeOptimized.ctor");

    //        LogUtil.ULogDev("RootNodeOptimized.ctor");

         
    //        UWASDKAgent.PopSample();
    //    }

    //    public override List<AbstractNode> getChildren() //<Modified> 
    //    {
    //        List<AbstractNode> children = new List<AbstractNode>();
         
    //    }
    //}
}


