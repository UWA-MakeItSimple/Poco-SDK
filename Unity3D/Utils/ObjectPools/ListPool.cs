using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poco.Utils
{
    class ListPool<T> : ObjectPool<List<T>>
    {
        public int DefaultContainerSize = 10;

        public override List<T> GetObj()
        {
            _state = State.Used;

            if (freshObjects.Count == 0)
            {
                List<T> obj = new List<T>(DefaultContainerSize);
                usedObjects.Add(obj);
                ObjCount++;
                return obj;
            }
            else
            {
                List<T> obj = freshObjects.Pop();
                usedObjects.Add(obj);
                return obj;
            }

        }


        public override List<T> GetObj(int size)
        {
            _state = State.Used;

            if (freshObjects.Count == 0)
            {
                List<T> obj = new List<T>(size);
                usedObjects.Add(obj);
                ObjCount++;
                return obj;
            }
            else
            {
                List<T> obj = freshObjects.Pop();
                usedObjects.Add(obj);
                return obj;
            }

        }



        public override void ReleaseAll()
        {
            _state = State.Released;

            //LogUtil.ULogDev("ListPool usedObjects count: " + usedObjects.Count);
            foreach (var obj in usedObjects)
            {
                obj.Clear();
                freshObjects.Push(obj);
            }
            usedObjects.Clear();


        }
    }

    class ListPoolManager
    {
        public static void ReleasePools()
        {
            ListPool_GameObject.Ins.ReleaseAll();
            ListPool_object.Ins.ReleaseAll();
            ListPool_str.Ins.ReleaseAll();            
			ListPool_Component.Ins.ReleaseAll();

        }
    }

    class ListPool_GameObject : ListPool<GameObject>
    {
        public static ListPool_GameObject Ins = new ListPool_GameObject();
    }

    class ListPool_Component : ListPool<Component>
    {
        public static ListPool_Component Ins = new ListPool_Component();
    }

    class ListPool_object : ListPool<object>
    {
        public static ListPool_object Ins = new ListPool_object();
    }

    class ListPool_str : ListPool<string>
    {
        public static ListPool_str Ins = new ListPool_str();
    }



}
