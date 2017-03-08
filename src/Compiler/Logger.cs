/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler Logger
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Collections;
using System.IO;
using System.Diagnostics;
using Atomix.CompilerExt;

namespace Atomix
{
    public class Logger
    {
        private string LoggerPath;
        private Stopwatch Timer;
        private bool IsLogging;

        ArrayList Script;
        ArrayList Message;
        ArrayList Details;

        public Logger(string aPath, bool aDoLog)
        {
            IsLogging = aDoLog;

            if (!aDoLog)
                return;

            Script = new ArrayList();
            Message = new ArrayList();
            Details = new ArrayList();
            LoggerPath = Path.Combine(Path.GetDirectoryName(aPath), Path.GetFileName(aPath) + Helper.LoggerFile);
            Timer = new Stopwatch();
            Timer.Start();
        }

        public void Write(string aScript, string aMessage, string aDetail)
        {
            if (!IsLogging)
                return;

            Script.Add(aScript);
            Message.Add(aMessage);
            Details.Add(aDetail);
        }

        public void Write(string aAppend, bool aSub = true)
        {
            if (!IsLogging)
                return;
            if (aSub)
                Details[Details.Count - 1] = string.Format("{0}<li>{1}</li>", Details[Details.Count - 1], aAppend);
            else
                Details[Details.Count - 1] = string.Format("{0}<br>{1}", Details[Details.Count - 1], aAppend);
        }

        public void Dump()
        {
            if (!IsLogging)
                return;

            Timer.Stop();
            log2html page = new log2html(Script, Message, Details, Timer.ElapsedMilliseconds.ToString());
            File.WriteAllText(LoggerPath, page.TransformText());
        }
    }
}
