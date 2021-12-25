using System;

namespace Consumer
{
    public class EmployeeFailure : Employee
    {
        public Exception Exception { get; }

        public EmployeeFailure(Exception exception)
        {
            Exception = exception;
        }
    }
}