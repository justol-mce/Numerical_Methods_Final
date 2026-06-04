using System;
using System.Collections.Generic;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Domain Model - Booking
    /// Represents a bus ticket booking with passenger and route details
    /// </summary>
    public class Booking
    {
        public string BookingId { get; set; }
        public string UserId { get; set; }
        public string BusId { get; set; }
        public string SeatNumber { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public string PassengerPhone { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TravelDate { get; set; }
        public string Status { get; set; } // CONFIRMED, CANCELLED, PENDING
        public double Price { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }

        public Booking()
        {
            BookingId = Guid.NewGuid().ToString();
            BookingDate = DateTime.Now;
            Status = "PENDING";
        }

        public Booking(string userId, string busId, string seatNumber, string passengerName,
                       string passengerEmail, string passengerPhone, DateTime travelDate,
                       double price, string source, string destination)
            : this()
        {
            UserId = userId;
            BusId = busId;
            SeatNumber = seatNumber;
            PassengerName = passengerName;
            PassengerEmail = passengerEmail;
            PassengerPhone = passengerPhone;
            TravelDate = travelDate;
            Price = price;
            Source = source;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"Booking{{ BookingId={BookingId}, PassengerName={PassengerName}, Source={Source}, " +
                   $"Destination={Destination}, SeatNumber={SeatNumber}, Status={Status}, Price={Price} }}";
        }
    }
}
