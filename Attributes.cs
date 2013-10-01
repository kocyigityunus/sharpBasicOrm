using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMVC
{
    public sealed class RequiredAttribute : Attribute
    { }

    public sealed class MaxLengthAttribute : Attribute
    {
        public MaxLengthAttribute(int maxLength)
        {
            maxLength = _MaxLength;
        }

        private int _MaxLength;
        public int MaxLength
        {
            get { return _MaxLength; }
        }

    }




}
