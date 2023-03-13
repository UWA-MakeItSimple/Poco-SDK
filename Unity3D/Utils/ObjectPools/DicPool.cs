using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Poco.Utils
{
    class DicPool<K, V> : ObjectPool<Dictionary<K, V>>
    {


        public int DefaultContainerSize = 10;

        public override Dictionary<K, V> GetObj()
        {
            _state = State.Used;

            if (freshObjects.Count == 0)
            {
                //LogUtil.ULogDev("freshObjects.Count == 0");
                Dictionary<K, V> obj = new Dictionary<K, V>(DefaultContainerSize);
                usedObjects.Add(obj);
                ObjCount++;
                return obj;
            }
            else
            {
                //LogUtil.ULogDev("freshObjects.Count != 0");

                Dictionary<K, V> obj = freshObjects.Pop();
                usedObjects.Add(obj);
                return obj;
            }
        }

        public override Dictionary<K, V> GetObj(int size)
        {
            _state = State.Used;

            if (freshObjects.Count == 0)
            {
                //LogUtil.ULogDev("freshObjects.Count == 0");
                Dictionary<K, V> obj = new Dictionary<K, V>(size);
                usedObjects.Add(obj);
                ObjCount++;
                return obj;
            }
            else
            {
                //LogUtil.ULogDev("freshObjects.Count != 0");

                Dictionary<K, V> obj = freshObjects.Pop();
                usedObjects.Add(obj);
                return obj;
            }
        }




        //public Dictionary<K, V> GetObj(int size)
        //{
        //    _state = State.Used;

        //    if (freshObjects.Count == 0)
        //    {
        //        Dictionary<K, V> obj = new Dictionary<K, V>(size);
        //        usedObjects.Add(obj);
        //        ObjCount++;
        //        return obj;
        //    }
        //    else
        //    {
        //        Dictionary<K, V> obj = freshObjects.Pop();
        //        usedObjects.Add(obj);
        //        return obj;
        //    }
        //}


        //public override void RestoreObj(Dictionary<K, V> obj)
        //{
        //    usedObjects.Remove(obj);
        //    obj.Clear();
        //    freshObjects.Push(obj);
        //}

        public override void ReleaseAll()
        {
            _state = State.Released;

            //LogUtil.ULogDev("DicPool usedObjects count: " + usedObjects.Count);
            foreach (var obj in usedObjects)
            {
                obj.Clear();
                freshObjects.Push(obj);
            }
            usedObjects.Clear();


        }

    }


    class DicPoolManager
    {
        public static void ReleasePools()
        {
            //UnityEngine.Debug.Log("DicPoolManager. ReleasePools -----------------------------");

            DicPoolSF2.Ins.ReleaseAll();
            DicPoolSO3.Ins.ReleaseAll();
            DicPoolSO4.Ins.ReleaseAll();
            DicPoolSO16.Ins.ReleaseAll();

        }
    }

    class DicPoolSO3 : DicPool<string, object>
    {
        DicPoolSO3()
        {
            DefaultContainerSize = 3;
        }


        public static DicPoolSO3 Ins = new DicPoolSO3();
    }

    class DicPoolSO4 : DicPool<string, object>
    {
        DicPoolSO4()
        {
            DefaultContainerSize = 4;
        }


        public static DicPoolSO4 Ins = new DicPoolSO4();
    }


    class DicPoolSO16 : DicPool<string, object>
    {
        DicPoolSO16()
        {
            DefaultContainerSize = 16;
        }


        public static DicPoolSO16 Ins = new DicPoolSO16();
    }

    class DicPoolSF2 : DicPool<string, float>
    {
        DicPoolSF2()
        {
            DefaultContainerSize = 2;
        }


        public static DicPoolSF2 Ins = new DicPoolSF2();
    }
}
