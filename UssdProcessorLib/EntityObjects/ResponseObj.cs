using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UssdProcessorLib.EntityObjects
{
    public class ResponseObj
    {
        private string response, toNode, fromNode;
        public string[] arraymenus = new string[20];
        private bool end, log;

        public string Response
        {
            get
            {
                return response;
            }
            set
            {
                response = value;
            }
        }
        public string ToNode
        {
            get
            {
                return toNode;
            }
            set
            {
                toNode = value;
            }
        }
        public string FromNode
        {
            get
            {
                return fromNode;
            }
            set
            {
                fromNode = value;
            }
        }
        public bool End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
            }
        }
        public bool Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
            }
        }
    }

}
