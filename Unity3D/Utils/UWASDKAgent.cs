using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;


namespace Poco
{
    public class UWASDKAgent
    {
        public static void PushSample(string sampleName)
        {
#if POCO_UWA_PROFILER_ENABLE			
            UWAEngine.PushSample(sampleName);
#endif
        }

        public static void PopSample()
        {
#if POCO_UWA_PROFILER_ENABLE			
            UWAEngine.PopSample();
#endif
        }


    }

}

