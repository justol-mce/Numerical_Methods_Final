using System;

namespace BusTicketingSystem.Exceptions
{
    /// <summary>
    /// Custom Exception - Validation
    /// Thrown when model validation fails
    /// </summary>
    public class ValidationException : Exception
    {
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationException(string fieldName, string errorMessage)
            : base($"Validation failed for field '{fieldName}': {errorMessage}")
        {
            FieldName = fieldName;
            ErrorMessage = errorMessage;
        }
    }
}
