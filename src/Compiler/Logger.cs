using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Atomix.CompilerExt;

namespace Atomix
{
    public class Logger
    {        
        string path;
        Stopwatch timer;
        bool DoLog;

        ArrayList Script;
        ArrayList Message;
        ArrayList Details;

        public Logger(string mPath, bool aDo)
        {
            this.DoLog = aDo;
            if (DoLog)
            {
                this.Script = new ArrayList();
                this.Message = new ArrayList();
                this.Details = new ArrayList();
                this.path = Path.Combine(mPath, Helper.LoggerFile);            
                this.timer = new Stopwatch();
                timer.Start();                
            }
        }

        public void Write(string mscript, string message, string aDetail)
        {
            if (DoLog)
            {
                Script.Add(mscript);
                Message.Add(message);
                Details.Add(aDetail);
            }
        }

        public void Write(string append, bool Sub = true)
        {
            if (Sub)
                Details[Details.Count - 1] = string.Format("{0}<li>{1}</li>", Details[Details.Count - 1], append);
            else
                Details[Details.Count - 1] = string.Format("{0}<br>{1}", Details[Details.Count - 1], append);
        }

        public void Dump()
        {
            if (DoLog)
            {
                timer.Stop();
                log2html page = new log2html(Script, Message, Details, timer.ElapsedMilliseconds.ToString());
                String pageContent = page.TransformText();
                System.IO.File.WriteAllText(path, pageContent);
            }
        }
    }
}
