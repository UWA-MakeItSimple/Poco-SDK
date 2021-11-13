using System;
using System.Collections.Generic;

namespace Poco.Utils
{
    class ObjectPool<T> where T:new()
    {

        //Buffer: objects
        //Buffer: used objects

        T GetObj()
        {
            T obj = new T();
            return obj;
        }


        void RestoreObj()
        {
            throw new Exception("Not Imp");

        }

        void ReleaseAll()
        {


            UnityEngine.Debug.Log("未归还的对象数量：--------------------------");

            throw new Exception("Not Imp");
        }

    }
}
