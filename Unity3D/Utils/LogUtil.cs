using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

namespace Poco.Utils
{
    class LogUtil
    {

        //public static LogUtil Ins = new LogUtil();

        /// <summary>
        /// UnityEngine.Debug.Log only for developing
        /// </summary>
        [Conditional("UWA_POCO_DEBUG")]
        public static void ULogDev(string msg)
        {
            UnityEngine.Debug.Log("[ UWA_POCO_DEBUG ] " + msg);
        }


        [Conditional("UWA_POCO_DEBUG")]
        public static void ULogDev(string des, List<string> list)
        {
            UnityEngine.Debug.Log("[ UWA_POCO_DEBUG ] " + des);

            foreach(var item in list)
            {
                UnityEngine.Debug.Log("[ UWA_POCO_DEBUG ] " + item);
            }
        }


        [Conditional("UWA_POCO_DEBUG")]
        public static void ULogDev(string des, HashSet<string> set)
        {
            UnityEngine.Debug.Log("[ UWA_POCO_DEBUG ] " + des);

            foreach (var item in set)
            {
                UnityEngine.Debug.Log("[ UWA_POCO_DEBUG ] " + item);
            }
        }


        public static void ULog()
        {

        }


    }
}

