using System;

namespace BusTicketingSystem.DTOs
{
    /// <summary>
    /// Output DTO - Booking
    /// Used to send booking data to UI for display
    /// </summary>
    public class BookingOutputDto
    {
        public string BookingId { get; set; }
        public string PassengerName { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string SeatNumber { get; set; }
        public string Status { get; set; }
        public double Price { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TravelDate { get; set; }
        public string BusNumber { get; set; }

        public override string ToString()
        {
            return $"BookingOutputDto{{ BookingId={BookingId}, PassengerName={PassengerName}, " +
                   $"Source={Source}, Destination={Destination}, SeatNumber={SeatNumber}, " +
                   $"Status={Status}, Price={Price} }}";
        }
    }
}
