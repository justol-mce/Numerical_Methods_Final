using System;
using System.Collections.Generic;
using System.Linq;
using BusTicketingSystem.Models;

namespace BusTicketingSystem.Repositories
{
    /// <summary>
    /// Repository - Booking
    /// Handles data access for booking operations (In-Memory)
    /// </summary>
    public class BookingRepository
    {
        private List<Booking> bookings = new List<Booking>();

        public void Save(Booking booking)
        {
            bookings.Add(booking);
        }

        public Booking FindById(string bookingId)
        {
            return bookings.FirstOrDefault(b => b.BookingId == bookingId);
        }

        public List<Booking> FindByUserId(string userId)
        {
            return bookings.Where(b => b.UserId == userId).ToList();
        }

        public List<Booking> FindByBusId(string busId)
        {
            return bookings.Where(b => b.BusId == busId).ToList();
        }

        public List<Booking> FindAll()
        {
            return new List<Booking>(bookings);
        }

        public void Update(Booking booking)
        {
            var existingBooking = FindById(booking.BookingId);
            if (existingBooking != null)
            {
                existingBooking.Status = booking.Status;
                existingBooking.Price = booking.Price;
            }
        }

        public void Delete(string bookingId)
        {
            var booking = FindById(bookingId);
            if (booking != null)
            {
                bookings.Remove(booking);
            }
        }
    }
}
