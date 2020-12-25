using BestHTTP.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basra.Client
{
    class MyBestHttpOutput : ILogOutput
    {
        public void Write(Loglevels level, string logEntry)
        {

            var newLineIndices = new int[]
            {
                logEntry.IndexOf("\"tid\":"),
                logEntry.IndexOf("\"div\":"),
                logEntry.IndexOf("\"msg\":"),
                logEntry.IndexOf("\"stack\":"),
                logEntry.IndexOf("\"ctxs\":"),
            };

            for (int i = 0; i < newLineIndices.Length; i++)
            {
                logEntry = logEntry.Insert(newLineIndices[i], "\n\n\n");
            }

            switch (level)
            {
                case Loglevels.All:
                case Loglevels.Information:
                    UnityEngine.Debug.Log(logEntry);
                    break;

                case Loglevels.Warning:
                    UnityEngine.Debug.LogWarning(logEntry);
                    break;

                case Loglevels.Error:
                case Loglevels.Exception:
                    UnityEngine.Debug.LogError(logEntry);
                    break;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
