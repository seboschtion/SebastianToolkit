using System;

namespace Sebastian.Toolkit.MVVM.Navigation
{
    public class NavigatorException : Exception
    {
        public NavigatorException(string message = "Unknown exception")
            : base(message)
        {
            
        }
    }
}