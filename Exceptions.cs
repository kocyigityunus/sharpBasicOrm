using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMVC
{
    public class MaxLengthAttributeNotFoundException : Exception
    {
        public MaxLengthAttributeNotFoundException() : base() { }
        public MaxLengthAttributeNotFoundException(string message) : base(message) { }
    }

    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException() : base() { }
        public ObjectNotFoundException(string message) : base(message) { }
    }

    public class ConstrucutorNullException : Exception
    {
        public ConstrucutorNullException() : base() { }
        public ConstrucutorNullException(string message) : base(message) { }
    }

}
