using System;
using System.Collections.Generic;
using BusTicketingSystem.DTOs;
using BusTicketingSystem.Models;
using BusTicketingSystem.Repositories;
using BusTicketingSystem.Validators;
using BusTicketingSystem.Exceptions;

namespace BusTicketingSystem.Controllers
{
    /// <summary>
    /// Controller - Booking
    /// Handles business logic for booking operations (MVC Pattern)
    /// </summary>
    public class BookingController
    {
        private BookingRepository bookingRepository;
        private BusRepository busRepository;
        private UserRepository userRepository;

        public BookingController()
        {
            bookingRepository = new BookingRepository();
            busRepository = new BusRepository();
            userRepository = new UserRepository();
        }

        public BookingOutputDto CreateBooking(BookingInputDto bookingInputDto)
        {
            try
            {
                // Validate Input
                BookingValidator.Validate(bookingInputDto);

                // Get Bus and User
                Bus bus = busRepository.FindById(bookingInputDto.BusId);
                User user = userRepository.FindById(bookingInputDto.UserId);

                if (bus == null)
                    throw new BookingException("Bus not found");
                if (user == null)
                    throw new BookingException("User not found");

                // Check seat availability
                if (!bus.IsSeatAvailable(bookingInputDto.SeatNumber))
                    throw new BookingException("Seat is not available");

                // Create and Save Booking
                var booking = new Booking(
                    bookingInputDto.UserId,
                    bookingInputDto.BusId,
                    bookingInputDto.SeatNumber,
                    bookingInputDto.PassengerName,
                    bookingInputDto.PassengerEmail,
                    bookingInputDto.PassengerPhone,
                    bookingInputDto.TravelDate,
                    bus.PricePerSeat,
                    bus.Source,
                    bus.Destination
                );

                booking.Status = "CONFIRMED";
                bus.BookSeat(bookingInputDto.SeatNumber);

                bookingRepository.Save(booking);
                busRepository.Update(bus);

                return MapToOutputDto(booking, bus.BusNumber);
            }
            catch (Exception ex)
            {
                throw new BookingException($"Failed to create booking: {ex.Message}");
            }
        }

        public BookingOutputDto GetBooking(string bookingId)
        {
            var booking = bookingRepository.FindById(bookingId);
            if (booking == null)
                throw new BookingException("Booking not found");

            Bus bus = busRepository.FindById(booking.BusId);
            return MapToOutputDto(booking, bus.BusNumber);
        }

        public List<BookingOutputDto> GetUserBookings(string userId)
        {
            var bookings = bookingRepository.FindByUserId(userId);
            List<BookingOutputDto> result = new List<BookingOutputDto>();

            foreach (var booking in bookings)
            {
                Bus bus = busRepository.FindById(booking.BusId);
                result.Add(MapToOutputDto(booking, bus.BusNumber));
            }

            return result;
        }

        public void CancelBooking(string bookingId)
        {
            try
            {
                Booking booking = bookingRepository.FindById(bookingId);
                if (booking == null)
                    throw new BookingException("Booking not found");

                Bus bus = busRepository.FindById(booking.BusId);
                if (bus != null)
                {
                    bus.CancelSeatBooking(booking.SeatNumber);
                    busRepository.Update(bus);
                }

                booking.Status = "CANCELLED";
                bookingRepository.Update(booking);
            }
            catch (Exception ex)
            {
                throw new BookingException($"Failed to cancel booking: {ex.Message}");
            }
        }

        private BookingOutputDto MapToOutputDto(Booking booking, string busNumber)
        {
            return new BookingOutputDto
            {
                BookingId = booking.BookingId,
                PassengerName = booking.PassengerName,
                Source = booking.Source,
                Destination = booking.Destination,
                SeatNumber = booking.SeatNumber,
                Status = booking.Status,
                Price = booking.Price,
                BookingDate = booking.BookingDate,
                TravelDate = booking.TravelDate,
                BusNumber = busNumber
            };
        }
    }
}
