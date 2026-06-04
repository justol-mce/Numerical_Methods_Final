using System;
using System.Collections.Generic;

namespace BusTicketingSystem.Models
{
    /// <summary>
    /// Domain Model - Bus
    /// Represents a bus with route, capacity, and seat information
    /// </summary>
    public class Bus
    {
        public string BusId { get; set; }
        public string BusNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public double PricePerSeat { get; set; }
        public string BusType { get; set; } // AC, NON_AC, SLEEPER
        public string OperatorName { get; set; }
        public HashSet<string> BookedSeats { get; set; }

        public Bus()
        {
            BusId = Guid.NewGuid().ToString();
            BookedSeats = new HashSet<string>();
        }

        public Bus(string busNumber, string source, string destination, int totalSeats,
                   double pricePerSeat, string busType, string operatorName)
            : this()
        {
            BusNumber = busNumber;
            Source = source;
            Destination = destination;
            TotalSeats = totalSeats;
            AvailableSeats = totalSeats;
            PricePerSeat = pricePerSeat;
            BusType = busType;
            OperatorName = operatorName;
        }

        public bool BookSeat(string seatNumber)
        {
            if (BookedSeats.Contains(seatNumber))
                return false;
            BookedSeats.Add(seatNumber);
            AvailableSeats--;
            return true;
        }

        public bool CancelSeatBooking(string seatNumber)
        {
            if (!BookedSeats.Contains(seatNumber))
                return false;
            BookedSeats.Remove(seatNumber);
            AvailableSeats++;
            return true;
        }

        public bool IsSeatAvailable(string seatNumber)
        {
            return !BookedSeats.Contains(seatNumber);
        }

        public override string ToString()
        {
            return $"Bus{{ BusNumber={BusNumber}, Source={Source}, Destination={Destination}, " +
                   $"AvailableSeats={AvailableSeats}, PricePerSeat={PricePerSeat}, BusType={BusType} }}";
        }
    }
}
