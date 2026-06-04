using System;

namespace BusTicketingSystem.Exceptions
{
    /// <summary>
    /// Custom Exception - Booking
    /// Thrown when booking-related operations fail
    /// </summary>
    public class BookingException : Exception
    {
        public BookingException(string message) : base(message) { }
        public BookingException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
