using System;
using System.IO;
using BusTicketingSystem.Models;

namespace BusTicketingSystem.Views
{
    /// <summary>
    /// View - Receipt Printer
    /// Handles receipt generation and printing for bookings
    /// </summary>
    public class ReceiptPrinter
    {
        public static void PrintBookingReceipt(string bookingId, string passengerName, 
                                               string busNumber, string source, 
                                               string destination, string seatNumber, 
                                               double price, DateTime travelDate)
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("                    BOOKING CONFIRMATION RECEIPT");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Booking ID:          {bookingId}");
            Console.WriteLine($"Passenger Name:      {passengerName}");
            Console.WriteLine($"Bus Number:          {busNumber}");
            Console.WriteLine($"Route:               {source} --> {destination}");
            Console.WriteLine($"Seat Number:         {seatNumber}");
            Console.WriteLine($"Price:               Rs. {price:F2}");
            Console.WriteLine($"Travel Date:         {travelDate:dd-MMM-yyyy}");
            Console.WriteLine($"Booking Date:        {DateTime.Now:dd-MMM-yyyy HH:mm:ss}");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("         Thank you for your booking! Safe travels!");
            Console.WriteLine(new string('=', 60) + "\n");
        }

        public static void PrintBusAvailability(Bus bus)
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.WriteLine($"Bus Number:          {bus.BusNumber}");
            Console.WriteLine($"Route:               {bus.Source} --> {bus.Destination}");
            Console.WriteLine($"Bus Type:            {bus.BusType}");
            Console.WriteLine($"Total Seats:         {bus.TotalSeats}");
            Console.WriteLine($"Available Seats:     {bus.AvailableSeats}");
            Console.WriteLine($"Price Per Seat:      Rs. {bus.PricePerSeat:F2}");
            Console.WriteLine($"Operator:            {bus.OperatorName}");
            Console.WriteLine(new string('-', 60) + "\n");
        }

        public static void PrintCancellationReceipt(string bookingId, double refundAmount)
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("                    CANCELLATION RECEIPT");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Booking ID:          {bookingId}");
            Console.WriteLine($"Refund Amount:       Rs. {refundAmount:F2}");
            Console.WriteLine($"Cancellation Date:   {DateTime.Now:dd-MMM-yyyy HH:mm:ss}");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("         Your refund will be processed within 5-7 days");
            Console.WriteLine(new string('=', 60) + "\n");
        }
    }
}
