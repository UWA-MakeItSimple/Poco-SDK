using System;
using System.Collections.Generic;

namespace Poco.Utils
{
    abstract class ObjectPool<T>  where T : new()
    {
        public int Capacity = 30;
        public int ObjCount = 0;

        protected Stack<T> freshObjects;
        protected HashSet<T> usedObjects;

        public enum State
        {
            Fresh,
            Used,
            Released
        }

        protected State _state;
        State state
        {
            get
            {
                return _state;
            }
        }

        //Buffer: objects
        //Buffer: used objects

        public ObjectPool()
        {
            _state = State.Fresh;
            freshObjects = new Stack<T>(Capacity);
            usedObjects = new HashSet<T>();
        }

        public abstract T GetObj();

        //Clear and restore
        //public abstract void RestoreObj(T obj);


        public void Init()
        {
            if(_state == State.Used)
            {
                throw new Exception("Pool has not been released");
            }

            _state = State.Fresh;
        }



        public abstract void ReleaseAll();


    }
}
