using System;

namespace BusTicketingSystem.DTOs
{
    /// <summary>
    /// Input DTO - Booking
    /// Used to receive booking data from UI
    /// </summary>
    public class BookingInputDto
    {
        public string UserId { get; set; }
        public string BusId { get; set; }
        public string SeatNumber { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public string PassengerPhone { get; set; }
        public DateTime TravelDate { get; set; }
    }
}
