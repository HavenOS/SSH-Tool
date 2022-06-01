using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSH_Tool
{
    internal class Functions
    {
        public static int ReadInt24(byte[] buffer)
        {
            return (buffer[0] << 16) + (buffer[1] << 8) + (buffer[2]);
        }


    }
}
