using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayoutTest
{
    class CommData
    {
        public int bodyLength;
        public byte[] bodyData;
    }
    public enum ManagerCommand
    {
        MC_STRING,
    }
    public enum ServerCommand
    {
        SC_STRING
    }


    [Serializable]
    struct MC_STRING_Struct
    {
        public string content;
    }
    [Serializable]
    struct SC_STRING_Struct
    {
        public string reason;//出错原因
    }



    class CommProtocol
    {
    }


}
