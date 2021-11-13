//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Poco.Utils;

//namespace Poco
//{
//    public class UnityDumperOptimized : AbstractDumper
//    {
//        public override Dictionary<string, object> dumpHierarchy(bool onlyVisibleNode)
//        {
//            UWASDKAgent.PushSample("AbstractDumper.BeforeGetRoot");
//            AbstractNode abstractNode = getRoot();
//            UWASDKAgent.PopSample();

//            LogUtil.ULogDev("AbstractDumper.dumpHierarchy");
//            return dfsDump(abstractNode, onlyVisibleNode, false);
//        }
//        //DumpHierarchy called by RootNode
//        //protected override Dictionary<string, object> dfsDump(AbstractNode node, bool onlyVisibleNode)
//        //{
//        //    UWASDKAgent.PushSample("UDmpOptmzd.dumpHImpl(node,only)");
//        //    LogUtil.ULogDev("UnityDmpOptimized.dumpHierarchyImpl");

//        //    if (node == null)
//        //    {
//        //        return null;
//        //    }

//        //    Dictionary<string, object> payload = node.enumerateAttrs();
//        //    Dictionary<string, object> result = new Dictionary<string, object>();
//        //    string name = (string)node.getAttr("name");
//        //    result.Add("name", name);
//        //    result.Add("payload", payload);

//        //    List<object> children = new List<object>();
//        //    foreach (AbstractNode child in node.getChildren())
//        //    {
//        //        if (!onlyVisibleNode || (bool)child.getAttr("visible"))
//        //        {
//        //            var childResult = dfsDump(child, onlyVisibleNode, false);
//        //            if (childResult != null)
//        //                children.Add(childResult);
//        //        }
//        //    }
//        //    if (children.Count > 0)
//        //    {
//        //        result.Add("children", children);
//        //    }
//        //    UWASDKAgent.PopSample();
//        //    return result;
//        //}

//        protected Dictionary<string, object> dfsDump(AbstractNode node, bool onlyVisibleNode, bool protectedByParent)
//        {
//            UWASDKAgent.PushSample("UDmpOptmzd.dumpHImpl(node,only,pBP)");

//            LogUtil.ULogDev("UnityDmpOptimized.dumpHierarchyImpl");

//            if (node == null)
//            {
//                return null;
//            }

//            Dictionary<string, object> payload = node.enumerateAttrs();

//            Dictionary<string, object> result = new Dictionary<string, object>();
//            string name = (string)node.getAttr("name");
//            result.Add("name", name);
//            result.Add("payload", payload);

//            if (protectedByParent)
//            {
//                (node as UnityNodeOptimized).protectedByParent = true;
//            }

//            bool protectChildren = false;

//            UWASDKAgent.PushSample("UDmpOptmzd.Filter");
//            bool shouldVisit = Filter(node, out protectChildren);
//            UWASDKAgent.PopSample();

//            if (!shouldVisit)
//                return null;

//            List<object> children = new List<object>();
//            foreach (AbstractNode child in node.getChildren())
//            {
//                if (!onlyVisibleNode || (bool)child.getAttr("visible"))
//                {
//                    var childResult = dfsDump(child, onlyVisibleNode, protectChildren);
//                    if (childResult != null)
//                        children.Add(childResult);
//                }
//            }
//            if (children.Count > 0)
//            {
//                result.Add("children", children);
//            }
//            UWASDKAgent.PopSample();
//            return result;
//        }

//        /// <summary>
//        /// Return false if node should be pruned
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        private bool Filter(AbstractNode node, out bool protectChildren)
//        {
//            //Strong protect judge
//            //---------------------------------------------------------------------

//            UWASDKAgent.PushSample("UDmpOptmzd.StringProtect");
//            bool tag1 = StrongProtect(node);
//            UWASDKAgent.PopSample();

//            if (tag1)
//            {
//                protectChildren = true;
//                return true;
//            }
//            else
//            {
//                if (((UnityNodeOptimized)node).protectedByParent)
//                    protectChildren = true;
//                else
//                    protectChildren = false;
//            }

//            //---------------------------------------------------------------------
//            //Weak protection judge
//            UWASDKAgent.PushSample("UDmpOptmzd.WeakProtect");
//            bool tag2 = WeakProtect(node);
//            UWASDKAgent.PopSample();

//            if (tag2) {

//                UWASDKAgent.PushSample("UDmpOptmzd.BlackListContain");
//                bool tag3 = BlackListContain(node);
//                UWASDKAgent.PopSample();

//                // String black list
//                if (tag3)
//                {
//                    return false;
//                }
//                else
//                {
//                    return true;
//                }
//            }
//            else
//            {
//                //Weak black list
//                return false;
//            }
//            //---------------------------------------------------------------------
//        }


//        /// <summary>
//        /// WhiteList check
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        private bool StrongProtect(AbstractNode node)
//        {
//            string name = ((UnityNodeOptimized)node).name;

//            if (node.IsUIPanel())
//            {
//                return true;
//            }

//            if (Config.Instance.strongWhiteList.Contains(name))
//            {
//                return true;
//            }
//            return false;
//        }

//        private bool WeakProtect(AbstractNode node)
//        {

//            if (node.isFirstLayer) return true;

//            if ((node as UnityNodeOptimized).protectedByParent)
//                return true;

//            string name = ((UnityNodeOptimized)node).name;
//            if (Config.Instance.weakWhiteList.Contains(name))
//            {
//                return true;
//            }

//            //if (((UnityNodeOptimized)node).IsUINode())
//            //{
//            //    return true;
//            //}
//            return false;
//        }

//        private bool BlackListContain(AbstractNode node)
//        {
//            string name = ((UnityNodeOptimized)node).name;

//            if (Config.Instance.blackList.Contains(name))
//                return true;
//            else
//                return false;

//        }

//        public virtual List<float> getPortSize()
//        {
//            return null;
//        }

//        public AbstractNode getRoot()
//        {
//            LogUtil.ULogDev("UnityDumperOptimized.getRoot");
//            return new RootNodeOptimized();
//        }
//    }

//    public class RootNodeOptimized : AbstractNode
//    {
//        //private List<AbstractNode> children = null;

//        public RootNodeOptimized()
//        {
//            UWASDKAgent.PushSample("RootNodeOptimized.ctor");

//            LogUtil.ULogDev("RootNodeOptimized.ctor");

         
//            UWASDKAgent.PopSample();
//        }

//        public override List<AbstractNode> getChildren() //<Modified> 
//        {
//            List<AbstractNode> children = new List<AbstractNode>();
//            foreach (GameObject obj in Transform.FindObjectsOfType(typeof(GameObject)))
//            {
//                if (obj.transform.parent == null)
//                {
//                    AbstractNode child = new UnityNodeOptimized(obj);
//                    child.isFirstLayer = true;
//                    children.Add(child);
//                }
//            }
//            return children;
//        }
//    }
//}


