using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poco
{
    class Config
    {
        public static Config Instance = new Config();

#if UWA_POCO_DEBUG
        public bool pruningEnabled = true;
#else
        /// <summary>
        /// 一键开启或关闭剪枝功能
        /// </summary>
        public bool pruningEnabled = false;
#endif

        public HashSet<string> blackList = new HashSet<string>();
        public HashSet<string> weakWhiteList = new HashSet<string>();
        public HashSet<string> strongWhiteList = new HashSet<string>();
        public HashSet<string> blockedAttributes = new HashSet<string>();
    }
}