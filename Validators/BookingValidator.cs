using System;
using BusTicketingSystem.DTOs;
using BusTicketingSystem.Exceptions;

namespace BusTicketingSystem.Validators
{
    /// <summary>
    /// Model Validator - Booking
    /// Validates input DTOs and business rules
    /// </summary>
    public class BookingValidator
    {
        public static void Validate(BookingInputDto bookingInputDto)
        {
            if (bookingInputDto == null)
                throw new ValidationException("booking", "Booking input cannot be null");

            if (string.IsNullOrEmpty(bookingInputDto.UserId))
                throw new ValidationException("userId", "User ID is required");

            if (string.IsNullOrEmpty(bookingInputDto.BusId))
                throw new ValidationException("busId", "Bus ID is required");

            if (string.IsNullOrEmpty(bookingInputDto.SeatNumber))
                throw new ValidationException("seatNumber", "Seat number is required");

            if (string.IsNullOrEmpty(bookingInputDto.PassengerName))
                throw new ValidationException("passengerName", "Passenger name is required");

            if (!IsValidEmail(bookingInputDto.PassengerEmail))
                throw new ValidationException("passengerEmail", "Valid email is required");

            if (string.IsNullOrEmpty(bookingInputDto.PassengerPhone))
                throw new ValidationException("passengerPhone", "Phone number is required");

            if (bookingInputDto.TravelDate == DateTime.MinValue)
                throw new ValidationException("travelDate", "Travel date is required");
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains(".");
        }
    }
}
