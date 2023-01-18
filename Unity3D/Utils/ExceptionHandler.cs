using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace Poco.Utils
{
    public class ExceptionHandler: Poco.Utils.Singleton<ExceptionHandler>
    {
        private bool opened = false;
        struct ExceptionInfo
        {
            public string condition;
            public string stackTrace;
            public ExceptionInfo(string condition, string stackTrace)
            {
                this.condition = condition;
                this.stackTrace = stackTrace;
            }
        }

        List<ExceptionInfo> exceptionInfos = new List<ExceptionInfo>(5);

        public void Open()
        {
            if (opened) return;

            Application.logMessageReceived += ExceptionLogCallback;
            opened = true;
        }

        public void Close()
        {
            if (!opened) return;

            Application.logMessageReceived -= ExceptionLogCallback;
            opened = false;
        }


        public void ExceptionLogCallback(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {

                exceptionInfos.Add(new ExceptionInfo(condition, stackTrace));
            }
        }

        StringBuilder ExceptionInfoSB = new StringBuilder();
        public string CatchException(bool CatStackTrace = false)
        {
            if (!opened) return null;

            if (exceptionInfos != null && exceptionInfos.Count > 0)
            {
                string res;
                for (int i = 0; i < exceptionInfos.Count; i++)
                {
                    ExceptionInfo info = exceptionInfos[i];

                    ExceptionInfoSB.Append("{\"");
                    ExceptionInfoSB.Append(info.condition);
                    ExceptionInfoSB.Append("\"");
                    if (CatStackTrace)
                    {

                        ExceptionInfoSB.Append("#,#\"");
                        ExceptionInfoSB.Append(info.stackTrace);
                        ExceptionInfoSB.Append("\"");

                    }
                    ExceptionInfoSB.Append("}");
                    ExceptionInfoSB.Append("#;#");

                }

                res = ExceptionInfoSB.ToString();
                ExceptionInfoSB.Clear();
                exceptionInfos.Clear();
                return res;
            }
            else
            {
                return null;
            }
        }

    }

}
