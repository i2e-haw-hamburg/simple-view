// ----------------------------------------------------------------
//  <author> Malte Eckhoff </author>
//  <datecreated> 03.04.2015 </datecreated>
//  <date> 14.06.2015 </date>
// 
//  <copyright file="UnityVS.BeardVisualizer/UnityVS.BeardVisualizer.CSharp/DefaultLogger.cs" owner="Malte Eckhoff" year=2015> 
//   All rights are reserved. Reproduction or transmission in whole or in part, in
//   any form or by any means, electronic, mechanical or otherwise, is prohibited
//   without the prior written consent of the copyright owner.
//  </copyright>
// ----------------------------------------------------------------

using BeardLogger.Interface;

using BeardUnityUtilities.Tools;

namespace Assets.Scripts.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using UnityEngine;

    public class DefaultLogger : MonoBehaviourSingleton<ILogger>, ILogger
    {
        [SerializeField]
        private string cfgLogFileName = "default";

        [SerializeField]
        private LogLevel cfgLogLevel = LogLevel.Warn;

        private ILogger fileLogger;

        public ILogger UsedLogger
        {
            get
            {
                if (this.fileLogger == null)
                {
                    this.fileLogger = LoggerFactory.GetNewThreadedFileLogger(
                        this.cfgLogLevel,
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar
                        + "logs" + Path.DirectorySeparatorChar + this.cfgLogFileName);
                }

                return this.fileLogger;
            }
        }

        public void Debug(string message)
        {
            this.UsedLogger.Debug(message);
        }

        public void Error(string message)
        {
            this.UsedLogger.Error(message);
        }

        public void Fatal(string message)
        {
            this.UsedLogger.Fatal(message);
        }

        public void Info(string message)
        {
            this.UsedLogger.Info(message);
        }

        public void Warn(string message)
        {
            this.UsedLogger.Warn(message);
        }

        public LogLevel MessageLoggingThreshhold
        {
            get
            {
                return this.UsedLogger.MessageLoggingThreshhold;
            }
            set
            {
                this.UsedLogger.MessageLoggingThreshhold = value;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            if (this.cfgLogFileName == "default")
            {
                // When initializing this directly Unity will complain for no good reason...
                this.cfgLogFileName = Application.companyName + "_" + Application.productName + ".log";
            }

            this.fileLogger = this.UsedLogger;

            Application.logMessageReceivedThreaded += this.ApplicationOnLogMessageReceivedThreaded;
        }

        private void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= this.ApplicationOnLogMessageReceivedThreaded;
        }

        private void ApplicationOnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    this.Debug(condition);
                    break;
                case LogType.Warning:
                    this.Warn(condition);
                    break;
                case LogType.Assert:
                case LogType.Exception:
                case LogType.Error:
                    this.Error(condition + "\n\n" + stackTrace);
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("CfgLogFileName: {0}, CfgLogLevel: {1}, MessageLoggingThreshhold: {2}, FileLogger: {3}, UsedLogger: {4}", cfgLogFileName, cfgLogLevel, MessageLoggingThreshhold, fileLogger, UsedLogger);
        }
    }
}