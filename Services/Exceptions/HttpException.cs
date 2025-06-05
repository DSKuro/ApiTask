using System;

namespace ApiTask.Services.Exceptions
{
    public class HttpException : Exception
    {
        public HttpException(string message) : base(message)
        { 
        }
    }
}
