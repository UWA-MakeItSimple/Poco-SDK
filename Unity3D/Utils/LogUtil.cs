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

        public static void ULog()
        {

        }


    }
}

