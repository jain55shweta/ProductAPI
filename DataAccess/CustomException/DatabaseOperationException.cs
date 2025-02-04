﻿namespace DataAccess.CustomException
{
    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException() { }

        public DatabaseOperationException(string message)
            : base(message) { }

        public DatabaseOperationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
