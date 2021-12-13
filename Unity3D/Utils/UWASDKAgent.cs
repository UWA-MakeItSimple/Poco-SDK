using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;


namespace Poco.Utils
{
    public class UWASDKAgent
    {
        [Conditional("POCO_UWA_PROFILER_ENABLE")]
        public static void PushSample(string sampleName)
        {
            UWAEngine.PushSample(sampleName);
        }

        [Conditional("POCO_UWA_PROFILER_ENABLE")]
        public static void PopSample()
        {
            UWAEngine.PopSample();
        }


    }

}

