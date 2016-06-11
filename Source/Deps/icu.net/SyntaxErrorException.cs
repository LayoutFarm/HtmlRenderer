// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
//using System.ComponentModel;
//using System.Data;
using System.IO;

namespace Icu
{
    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(string description)
            : base(description)
        {
        }
    }

}