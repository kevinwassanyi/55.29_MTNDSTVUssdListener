using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TCPServer tcpServer = new TCPServer();
            while (true)
            {
                try
                {
                    tcpServer.ListenAndProcess();
                }catch (Exception e)
                {

                }
            }
        }
    }
}
