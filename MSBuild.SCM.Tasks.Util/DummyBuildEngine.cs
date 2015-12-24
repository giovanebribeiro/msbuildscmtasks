using Microsoft.Build.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks
{
    public class DummyBuildEngine:IBuildEngine
    {
        #region logs
        private StringBuilder error_log = new StringBuilder();
        public string ErrorLog
        {
            get { return error_log.ToString(); }
        }

        private StringBuilder message_log = new StringBuilder();
        public string MessageLog
        {
            get { return message_log.ToString(); }
        }

        private StringBuilder warning_log = new StringBuilder();
        public string WarningLog
        {
            get { return warning_log.ToString(); }
        }

        public StringBuilder custom_log = new StringBuilder();
        public string CustomLog
        {
            get
            {
                return custom_log.ToString();
            }
        }

        #endregion

        #region counters
        private int error_count;
        public int ErrorCount
        {
            get { return error_count; }
        }

        private int message_count;
        public int MessageCount
        {
            get { return message_count; }
        }

        private int warning_count;
        public int WarningCount
        {
            get { return warning_count; }
        }

        private int custom_count;
        public int CustomCount
        {
            get { return custom_count; }
        }

        public int LogCount
        {
            get { return error_count + message_count + warning_count; }
        }
        #endregion

        public DummyBuildEngine()
        {
            error_count = 0;
            message_count = 0;
            warning_count = 0;
            custom_count = 0;
        }

        #region IBuildEngineMembers
        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return false;
        }

        public int ColumnNumberOfTaskNode
        {
            get { return 0; }
        }

        public bool ContinueOnError
        {
            get { return false; }
        }

        public int LineNumberOfTaskNode
        {
            get { return 0; }
        }

        public string ProjectFileOfTaskNode
        {
            get { return string.Empty; }
        }   
        #endregion

        #region members
        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            error_count++;
            error_log.AppendLine(e.Message);
            Console.WriteLine("Error: {0}", e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            warning_count++;
            warning_log.AppendLine(e.Message);
            Console.WriteLine("Warning: {0}", e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            message_count++;
            message_log.AppendLine(e.Message);
            Console.WriteLine("Message: {0}", e.Message);
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            custom_count++;
            custom_log.AppendLine(e.Message);
            Console.WriteLine("Custom: {0}", e.Message);
        }

        #endregion
    }
}
