using System;
using System.Collections.Generic;
using BusTicketingSystem.Controllers;
using BusTicketingSystem.DTOs;
using BusTicketingSystem.Models;
using BusTicketingSystem.Views;

namespace BusTicketingApp
{
    class Program
    {
        private static BusController busController = new BusController();
        private static BookingController bookingController = new BookingController();
        private static UserController userController = new UserController();
        private static User currentUser = null;

        static void Main(string[] args)
        {
            InitializeSampleData();
            ShowMainMenu();
        }

        static void InitializeSampleData()
        {
            // Register sample users
            User user1 = new User("john_doe", "john@example.com", "password123", 
                                  "John Doe", "9876543210", "123 Main St");
            User user2 = new User("jane_smith", "jane@example.com", "password456", 
                                  "Jane Smith", "9123456789", "456 Oak Ave");
            userController.RegisterUser(user1);
            userController.RegisterUser(user2);

            // Register sample buses
            Bus bus1 = new Bus("BUS-001", "Delhi", "Agra", 40, 500.0, "AC", "TravelCo");
            Bus bus2 = new Bus("BUS-002", "Delhi", "Agra", 50, 450.0, "NON_AC", "QuickBus");
            Bus bus3 = new Bus("BUS-003", "Mumbai", "Pune", 45, 600.0, "SLEEPER", "LuxusBus");
            busController.RegisterBus(bus1);
            busController.RegisterBus(bus2);
            busController.RegisterBus(bus3);

            Console.WriteLine("Sample data initialized successfully!\n");
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n========== BUS TICKETING SYSTEM ==========");
                Console.WriteLine("1. User Login");
                Console.WriteLine("2. Register New User");
                Console.WriteLine("3. Search Buses");
                Console.WriteLine("4. My Bookings (after login)");
                Console.WriteLine("5. Exit");
                Console.WriteLine("==========================================");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        UserLogin();
                        break;
                    case "2":
                        RegisterUser();
                        break;
                    case "3":
                        SearchBuses();
                        break;
                    case "4":
                        if (currentUser != null)
                            MyBookings();
                        else
                            Console.WriteLine("Please login first!");
                        break;
                    case "5":
                        Console.WriteLine("Thank you for using Bus Ticketing System!");
                        return;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }

        static void UserLogin()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            try
            {
                currentUser = userController.Login(username, password);
                Console.WriteLine($"\nWelcome, {currentUser.FullName}!");
                ShowUserMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
            }
        }

        static void RegisterUser()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            Console.Write("Enter Full Name: ");
            string fullName = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            string phoneNumber = Console.ReadLine();
            Console.Write("Enter Address: ");
            string address = Console.ReadLine();

            try
            {
                User newUser = new User(username, email, password, fullName, phoneNumber, address);
                userController.RegisterUser(newUser);
                Console.WriteLine("Registration successful! Please login.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
            }
        }

        static void SearchBuses()
        {
            Console.Write("Enter Source: ");
            string source = Console.ReadLine();
            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine();

            List<Bus> buses = busController.SearchBusesByRoute(source, destination);

            if (buses.Count == 0)
            {
                Console.WriteLine("No buses found for this route!");
                return;
            }

            Console.WriteLine($"\n========== Buses from {source} to {destination} ==========");
            for (int i = 0; i < buses.Count; i++)
            {
                Console.WriteLine($"\n{i + 1}. Bus Number: {buses[i].BusNumber}");
                ReceiptPrinter.PrintBusAvailability(buses[i]);
            }

            if (currentUser != null)
            {
                Console.Write("Book a bus? (Enter bus number or 0 to cancel): ");
                string busNum = Console.ReadLine();
                if (busNum != "0")
                {
                    BookBus(buses, busNum);
                }
            }
        }

        static void BookBus(List<Bus> buses, string busNumber)
        {
            Bus selectedBus = buses.Find(b => b.BusNumber == busNumber);
            if (selectedBus == null)
            {
                Console.WriteLine("Invalid bus number!");
                return;
            }

            Console.Write("Enter Seat Number: ");
            string seatNumber = Console.ReadLine();
            Console.Write("Enter Your Name: ");
            string passengerName = Console.ReadLine();
            Console.Write("Enter Your Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Travel Date (dd-MMM-yyyy): ");
            string dateStr = Console.ReadLine();

            try
            {
                DateTime travelDate = DateTime.ParseExact(dateStr, "dd-MMM-yyyy", null);

                BookingInputDto bookingInput = new BookingInputDto
                {
                    UserId = currentUser.UserId,
                    BusId = selectedBus.BusId,
                    SeatNumber = seatNumber,
                    PassengerName = passengerName,
                    PassengerEmail = email,
                    PassengerPhone = currentUser.PhoneNumber,
                    TravelDate = travelDate
                };

                BookingOutputDto booking = bookingController.CreateBooking(bookingInput);
                ReceiptPrinter.PrintBookingReceipt(booking.BookingId, booking.PassengerName,
                                                    booking.BusNumber, booking.Source,
                                                    booking.Destination, booking.SeatNumber,
                                                    booking.Price, booking.TravelDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Booking failed: {ex.Message}");
            }
        }

        static void ShowUserMenu()
        {
            while (currentUser != null)
            {
                Console.WriteLine("\n========== USER MENU ==========");
                Console.WriteLine("1. Search Buses");
                Console.WriteLine("2. View My Bookings");
                Console.WriteLine("3. Cancel a Booking");
                Console.WriteLine("4. Logout");
                Console.WriteLine("===============================");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SearchBuses();
                        break;
                    case "2":
                        MyBookings();
                        break;
                    case "3":
                        CancelBooking();
                        break;
                    case "4":
                        currentUser = null;
                        Console.WriteLine("Logged out successfully!");
                        break;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }

        static void MyBookings()
        {
            List<BookingOutputDto> bookings = bookingController.GetUserBookings(currentUser.UserId);

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found!");
                return;
            }

            Console.WriteLine("\n========== MY BOOKINGS ==========");
            foreach (BookingOutputDto booking in bookings)
            {
                Console.WriteLine(booking);
            }
        }

        static void CancelBooking()
        {
            Console.Write("Enter Booking ID to cancel: ");
            string bookingId = Console.ReadLine();

            try
            {
                BookingOutputDto booking = bookingController.GetBooking(bookingId);
                bookingController.CancelBooking(bookingId);
                ReceiptPrinter.PrintCancellationReceipt(bookingId, booking.Price);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cancellation failed: {ex.Message}");
            }
        }
    }
}
