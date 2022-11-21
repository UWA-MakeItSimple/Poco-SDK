using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poco.Utils;


namespace Poco
{
    //public class AbstractDumper : IDumper
    //{
    //    public virtual AbstractNode getRoot()
    //    {
    //        return null;
    //    }

    //    //public Dictionary<string, object> dumpHierarchy()
    //    //{
    //    //    return dumpHierarchyImpl(getRoot(), true);
    //    //}
    //    public virtual Dictionary<string, object>  dumpHierarchy(bool onlyVisibleNode)
    //    {
    //        UWASDKAgent.PushSample("AbstractDumper.BeforeGetRoot");
    //        AbstractNode abstractNode = getRoot();
    //        UWASDKAgent.PopSample();

    //        LogUtil.ULogDev("AbstractDumper.dumpHierarchy");
    //        return dfsDump(abstractNode, onlyVisibleNode);
    //    }

    //    protected virtual Dictionary<string, object> dfsDump(AbstractNode node, bool onlyVisibleNode)
    //    {
    //        //throw new Exception("dumpHierarchyImpl Not implement");
    //        LogUtil.ULogDev("AbstractDumper.dumpHierarchyImpl");

    //        if (node == null)
    //        {
    //            return null;
    //        }

    //        Dictionary<string, object> payload = node.enumerateAttrs();
    //        Dictionary<string, object> result = new Dictionary<string, object>();
    //        string name = (string)node.getAttr("name");
    //        result.Add("name", name);
    //        result.Add("payload", payload);

    //        List<object> children = new List<object>();
    //        foreach (AbstractNode child in node.getChildren())
    //        {
    //            if (!onlyVisibleNode || (bool)child.getAttr("visible"))
    //            {
    //                children.Add(dfsDump(child, onlyVisibleNode));
    //            }
    //        }
    //        if (children.Count > 0)
    //        {
    //            result.Add("children", children);
    //        }
    //        return result;
    //    }

    //    public virtual List<float> getPortSize()
    //    {
    //        return null;
    //    }
    //}

    public interface IDumper<T>
    {
        //AbstractNode GetRoot();

        List<T> GetFirstLevelNodes();

        //Dictionary<string, object> dumpHierarchy();
        Dictionary<string, object> DumpHierarchy(bool onlyVisibleNode);

        List<float> GetPortSize();
    }
}
