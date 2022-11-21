namespace Poco.Utils
{ 

    //public abstract class StatefulSingleton<T> where T : new()
    //{
    //    public static readonly T Instance = new T();
    //    //public abstract void Init();
    //    public abstract void Release();
    //}

    public abstract class Singleton<T> where T : new()
    {
        public static readonly T Instance = new T();
    }


}