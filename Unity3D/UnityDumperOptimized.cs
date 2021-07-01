using System;
using System.Collections.Generic;
using UnityEngine;
using Poco.Utils;

namespace Poco
{
    public class UnityDumperOptimized : AbstractDumper
    {

        //DumpHierarchy called by RootNode
        protected override Dictionary<string, object> dumpHierarchyImpl(AbstractNode node, bool onlyVisibleNode)
        {
            LogUtil.ULogDev("UnityDumperOptimized.dumpHierarchyImpl");

            if (node == null)
            {
                return null;
            }

            Dictionary<string, object> payload = node.enumerateAttrs();
            Dictionary<string, object> result = new Dictionary<string, object>();
            string name = (string)node.getAttr("name");
            result.Add("name", name);
            result.Add("payload", payload);

            List<object> children = new List<object>();
            foreach (AbstractNode child in node.getChildren())
            {
                if (!onlyVisibleNode || (bool)child.getAttr("visible"))
                {
                    var childResult = dumpHierarchyImpl(child, onlyVisibleNode, false);
                    if (childResult != null)
                        children.Add(childResult);
                }
            }
            if (children.Count > 0)
            {
                result.Add("children", children);
            }
            return result;
        }

        protected Dictionary<string, object> dumpHierarchyImpl(AbstractNode node, bool onlyVisibleNode, bool protectedByParent)
        {
            LogUtil.ULogDev("UnityDumperOptimized.dumpHierarchyImpl");

            if (node == null)
            {
                return null;
            }

            Dictionary<string, object> payload = node.enumerateAttrs();
            Dictionary<string, object> result = new Dictionary<string, object>();
            string name = (string)node.getAttr("name");
            result.Add("name", name);
            result.Add("payload", payload);


            if (protectedByParent)
            {
                (node as UnityNodeOptimized).protectedByParent = true;
            }

            bool protectChildren = false;

            bool shouldVisit = Filter(node, out protectChildren);

            if (!shouldVisit)
                return null;

            List<object> children = new List<object>();
            foreach (AbstractNode child in node.getChildren())
            {
                if (!onlyVisibleNode || (bool)child.getAttr("visible"))
                {
                    var childResult = dumpHierarchyImpl(child, onlyVisibleNode, protectChildren);
                    if (childResult != null)
                        children.Add(childResult);
                }
            }
            if (children.Count > 0)
            {
                result.Add("children", children);
            }
            return result;
        }

        /// <summary>
        /// Return false if node should be pruned
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool Filter(AbstractNode node, out bool protectChildren)
        {
            //Strong protect judge
            //---------------------------------------------------------------------
            

            if (StrongProtect(node))
            {
                protectChildren = true;
                return true;
            }
            else
            {
                if (((UnityNodeOptimized)node).protectedByParent)
                    protectChildren = true;
                else
                    protectChildren = false;

            }


            //---------------------------------------------------------------------
            //Weak protection judge
            if (WeakProtect(node))
            {


                // String black list
                if (BlackListContain(node))
                    return false;
                else
                    return true;
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
        private bool StrongProtect(AbstractNode node)
        {
            string name = ((UnityNodeOptimized)node).name;

            if (node.IsUIPanel())
            {
                return true;

            }

            if (Config.Instance.strongWhiteList.Contains(name))
            {
                return true;
            }

            return false;
        }

        private bool WeakProtect(AbstractNode node)
        {

            if ((node as UnityNodeOptimized).protectedByParent)
                return true;

            string name = ((UnityNodeOptimized)node).name;
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

        private bool BlackListContain(AbstractNode node)
        {
            string name = ((UnityNodeOptimized)node).name;

            if (Config.Instance.blackList.Contains(name))
                return true;
            else
                return false;

        }

        public virtual List<float> getPortSize()
        {
            return null;
        }

        public override AbstractNode getRoot()
        {
            LogUtil.ULogDev("UnityDumperOptimized.getRoot");
            return new RootNodeOptimized();
        }
    }

    public class RootNodeOptimized : AbstractNode
    {
        private List<AbstractNode> children = null;

        public RootNodeOptimized()
        {
            LogUtil.ULogDev("RootNodeOptimized.ctor");

            children = new List<AbstractNode>();
            foreach (GameObject obj in Transform.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.transform.parent == null)
                {
                    children.Add(new UnityNodeOptimized(obj));
                }
            }
        }

        public override List<AbstractNode> getChildren() //<Modified> 
        {
            return children;
        }
    }
}


