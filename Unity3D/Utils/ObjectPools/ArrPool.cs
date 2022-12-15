using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poco.Utils
{
    internal class ArrPool<T> : ObjectPool<T[]>
    {
        public int DefaultContainerSize = 2;

        public override T[] GetObj()
        {
            _state = State.Used;

            if (freshObjects.Count == 0)
            {
                T[] obj = new T[DefaultContainerSize];
                usedObjects.Add(obj);
                ObjCount++;
                return obj;
            }
            else
            {
                T[] obj = freshObjects.Pop();
                usedObjects.Add(obj);
                return obj;
            }

        }


        public override T[] GetObj(int size)
        {
            throw new Exception("Not Imp");

        }



        public override void ReleaseAll()
        {
            _state = State.Released;

            //LogUtil.ULogDev("ListPool usedObjects count: " + usedObjects.Count);
            foreach (var obj in usedObjects)
            {
                
                freshObjects.Push(obj);
            }
            usedObjects.Clear();
        }
    }

    class ArrPoolManager
    {
        public static void ReleasePools()
        {
            ArrPool_float.Ins.ReleaseAll();

        }
    }

    class ArrPool_float : ArrPool<float>
    {
        public static ArrPool_float Ins = new ArrPool_float();

        public override void ReleaseAll()
        {
            _state = State.Released;

            //LogUtil.ULogDev("ListPool usedObjects count: " + usedObjects.Count);
            foreach (var obj in usedObjects)
            {
                obj[0] = 0f;
                obj[1] = 0f;
                freshObjects.Push(obj);
            }
            usedObjects.Clear();
        }

    }
}
