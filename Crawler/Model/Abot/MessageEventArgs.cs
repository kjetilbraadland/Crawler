using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Model.Abot
{
    class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string Message)
        {
            this.Message = Message;
        }
        public string Message { get; }
    }
}
