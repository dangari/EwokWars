using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EwokWars.Levels
{
    [Serializable()]
    class LevelParserException : System.Exception
    {
        public LevelParserException() : base() { }
        public LevelParserException(string message) : base(message) { }
        public LevelParserException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        protected LevelParserException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
