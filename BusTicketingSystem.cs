using System;
using System.Collections.Generic;
using System.Linq;

namespace BusTicketingSystem
{
    // ==================== MODELS ====================
    
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

    /// <summary>
    /// Domain Model - User
    /// Represents a user account with authentication and profile information
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string UserType { get; set; } // CUSTOMER, ADMIN
        public bool IsActive { get; set; }

        public User()
        {
            UserId = Guid.NewGuid().ToString();
            IsActive = true;
            UserType = "CUSTOMER";
        }

        public User(string username, string email, string password, string fullName,
                    string phoneNumber, string address)
            : this()
        {
            Username = username;
            Email = email;
            Password = password;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public bool ValidateCredentials(string inputPassword)
        {
            return Password.Equals(inputPassword);
        }

        public override string ToString()
        {
            return $"User{{ UserId={UserId}, Username={Username}, Email={Email}, FullName={FullName} }}";
        }
    }

    // ==================== DTOs ====================

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

    // ==================== EXCEPTIONS ====================

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

    /// <summary>
    /// Custom Exception - Validation
    /// Thrown when model validation fails
    /// </summary>
    public class ValidationException : Exception
    {
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationException(string fieldName, string errorMessage)
            : base($"Validation failed for field '{fieldName}': {errorMessage}")
        {
            FieldName = fieldName;
            ErrorMessage = errorMessage;
        }
    }

    // ==================== VALIDATORS ====================

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

    // ==================== REPOSITORIES ====================

    /// <summary>
    /// Repository - Bus
    /// Handles data access for bus operations (In-Memory)
    /// </summary>
    public class BusRepository
    {
        private List<Bus> buses = new List<Bus>();

        public void Save(Bus bus)
        {
            buses.Add(bus);
        }

        public Bus FindById(string busId)
        {
            return buses.FirstOrDefault(b => b.BusId == busId);
        }

        public List<Bus> FindByRoute(string source, string destination)
        {
            return buses.Where(b => b.Source == source && b.Destination == destination).ToList();
        }

        public List<Bus> FindAll()
        {
            return new List<Bus>(buses);
        }

        public void Update(Bus bus)
        {
            var existingBus = FindById(bus.BusId);
            if (existingBus != null)
            {
                existingBus.AvailableSeats = bus.AvailableSeats;
                existingBus.BookedSeats = bus.BookedSeats;
            }
        }
    }

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

        public void Update(Booking booking)
        {
            var existingBooking = FindById(booking.BookingId);
            if (existingBooking != null)
            {
                existingBooking.Status = booking.Status;
            }
        }
    }

    // ==================== SERVICES ====================

    /// <summary>
    /// Service - Booking
    /// Implements booking business logic
    /// </summary>
    public class BookingService
    {
        private BookingRepository bookingRepository;
        private BusRepository busRepository;

        public BookingService()
        {
            bookingRepository = new BookingRepository();
            busRepository = new BusRepository();
        }

        public BookingOutputDto CreateBooking(BookingInputDto inputDto)
        {
            try
            {
                // Validate input
                BookingValidator.Validate(inputDto);

                // Get bus details
                Bus bus = busRepository.FindById(inputDto.BusId);
                if (bus == null)
                    throw new BookingException("Bus not found");

                // Check seat availability
                if (!bus.IsSeatAvailable(inputDto.SeatNumber))
                    throw new BookingException($"Seat {inputDto.SeatNumber} is already booked");

                // Book the seat
                bus.BookSeat(inputDto.SeatNumber);
                busRepository.Update(bus);

                // Create booking
                var booking = new Booking(
                    inputDto.UserId,
                    inputDto.BusId,
                    inputDto.SeatNumber,
                    inputDto.PassengerName,
                    inputDto.PassengerEmail,
                    inputDto.PassengerPhone,
                    inputDto.TravelDate,
                    bus.PricePerSeat,
                    bus.Source,
                    bus.Destination
                );
                booking.Status = "CONFIRMED";
                bookingRepository.Save(booking);

                // Return output DTO
                return MapBookingToOutputDto(booking, bus.BusNumber);
            }
            catch (Exception e)
            {
                throw new BookingException($"Failed to create booking: {e.Message}");
            }
        }

        public BookingOutputDto GetBooking(string bookingId)
        {
            Booking booking = bookingRepository.FindById(bookingId);
            if (booking == null)
                throw new BookingException("Booking not found");

            Bus bus = busRepository.FindById(booking.BusId);
            return MapBookingToOutputDto(booking, bus != null ? bus.BusNumber : "N/A");
        }

        public List<BookingOutputDto> GetUserBookings(string userId)
        {
            List<Booking> bookings = bookingRepository.FindByUserId(userId);
            var result = new List<BookingOutputDto>();

            foreach (var booking in bookings)
            {
                Bus bus = busRepository.FindById(booking.BusId);
                result.Add(MapBookingToOutputDto(booking, bus != null ? bus.BusNumber : "N/A"));
            }

            return result;
        }

        public void CancelBooking(string bookingId)
        {
            Booking booking = bookingRepository.FindById(bookingId);
            if (booking == null)
                throw new BookingException("Booking not found");

            // Cancel seat booking
            Bus bus = busRepository.FindById(booking.BusId);
            if (bus != null)
            {
                bus.CancelSeatBooking(booking.SeatNumber);
                busRepository.Update(bus);
            }

            // Update booking status
            booking.Status = "CANCELLED";
            bookingRepository.Update(booking);
        }

        public BusRepository GetBusRepository()
        {
            return busRepository;
        }

        private BookingOutputDto MapBookingToOutputDto(Booking booking, string busNumber)
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

    /// <summary>
    /// Service - Bus
    /// Implements bus management business logic
    /// </summary>
    public class BusService
    {
        private BusRepository busRepository;

        public BusService(BusRepository busRepository)
        {
            this.busRepository = busRepository;
        }

        public void CreateBus(Bus bus)
        {
            busRepository.Save(bus);
        }

        public Bus GetBusById(string busId)
        {
            return busRepository.FindById(busId);
        }

        public List<Bus> SearchBuses(string source, string destination)
        {
            return busRepository.FindByRoute(source, destination);
        }

        public List<Bus> GetAllBuses()
        {
            return busRepository.FindAll();
        }

        public void UpdateBus(Bus bus)
        {
            busRepository.Update(bus);
        }

        public bool CheckSeatAvailability(string busId, string seatNumber)
        {
            Bus bus = busRepository.FindById(busId);
            return bus != null && bus.IsSeatAvailable(seatNumber);
        }
    }

    // ==================== RECEIPT PRINTER ====================

    /// <summary>
    /// Receipt Printer
    /// Generates formatted bus ticket receipts for printing
    /// </summary>
    public class ReceiptPrinter
    {
        private const int RECEIPT_WIDTH = 58;
        private static string SEPARATOR = new string('=', RECEIPT_WIDTH);
        private static string DASH_LINE = new string('-', RECEIPT_WIDTH);

        public static string GenerateReceipt(BookingOutputDto booking)
        {
            var receipt = new System.Text.StringBuilder();

            // Header
            receipt.AppendLine(SEPARATOR);
            receipt.AppendLine(CenterText("BUS TICKETING SYSTEM"));
            receipt.AppendLine(CenterText("BOOKING RECEIPT"));
            receipt.AppendLine(SEPARATOR);
            receipt.AppendLine();

            // Booking Information
            receipt.AppendLine("BOOKING CONFIRMATION");
            receipt.AppendLine(DASH_LINE);
            receipt.AppendLine($"Booking ID          : {booking.BookingId}");
            receipt.AppendLine($"Booking Date        : {booking.BookingDate:yyyy-MM-dd}");
            receipt.AppendLine($"Booking Time        : {booking.BookingDate:HH:mm:ss}");
            receipt.AppendLine($"Status              : {booking.Status}");
            receipt.AppendLine();

            // Passenger Information
            receipt.AppendLine("PASSENGER DETAILS");
            receipt.AppendLine(DASH_LINE);
            receipt.AppendLine($"Passenger Name      : {booking.PassengerName}");
            receipt.AppendLine();

            // Journey Details
            receipt.AppendLine("JOURNEY DETAILS");
            receipt.AppendLine(DASH_LINE);
            receipt.AppendLine($"Bus Number          : {booking.BusNumber}");
            receipt.AppendLine($"From                : {booking.Source}");
            receipt.AppendLine($"To                  : {booking.Destination}");
            receipt.AppendLine($"Travel Date         : {booking.TravelDate:yyyy-MM-dd}");
            receipt.AppendLine($"Seat Number         : {booking.SeatNumber}");
            receipt.AppendLine();

            // Fare Details
            receipt.AppendLine("FARE DETAILS");
            receipt.AppendLine(DASH_LINE);
            receipt.AppendLine($"Ticket Price        : ${booking.Price:F2}");
            receipt.AppendLine($"Taxes & Charges     : ${booking.Price * 0.1:F2}");
            receipt.AppendLine(DASH_LINE);
            receipt.AppendLine($"TOTAL AMOUNT        : ${booking.Price * 1.1:F2}");
            receipt.AppendLine();

            // Footer
            receipt.AppendLine(SEPARATOR);
            receipt.AppendLine(CenterText("THANK YOU FOR YOUR BOOKING"));
            receipt.AppendLine(CenterText("Have a safe and pleasant journey!"));
            receipt.AppendLine(SEPARATOR);

            return receipt.ToString();
        }

        public static void PrintReceipt(BookingOutputDto booking)
        {
            Console.WriteLine(GenerateReceipt(booking));
        }

        private static string CenterText(string text)
        {
            int totalPadding = RECEIPT_WIDTH - text.Length;
            int leftPadding = totalPadding / 2;
            int rightPadding = totalPadding - leftPadding;
            return new string(' ', leftPadding) + text + new string(' ', rightPadding);
        }
    }

    // ==================== VIEWS ====================

    /// <summary>
    /// View - Home
    /// Displays main menu and handles user navigation
    /// </summary>
    public class HomeView
    {
        private BookingController bookingController;
        private BusController busController;

        public HomeView(BookingController bookingController, BusController busController)
        {
            this.bookingController = bookingController;
            this.busController = busController;
        }

        public void DisplayMenu()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("   MAIN MENU");
                Console.WriteLine(new string('=', 50));
                Console.WriteLine("1. Search Buses");
                Console.WriteLine("2. View All Buses");
                Console.WriteLine("3. Create Booking");
                Console.WriteLine("4. View Booking");
                Console.WriteLine("5. View My Bookings");
                Console.WriteLine("6. Cancel Booking");
                Console.WriteLine("7. Add Bus (Admin)");
                Console.WriteLine("0. Exit");
                Console.WriteLine(new string('=', 50));
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        busController.SearchBuses();
                        break;
                    case "2":
                        busController.ViewAllBuses();
                        break;
                    case "3":
                        bookingController.CreateBooking();
                        break;
                    case "4":
                        bookingController.ViewBooking();
                        break;
                    case "5":
                        bookingController.ViewUserBookings();
                        break;
                    case "6":
                        bookingController.CancelBooking();
                        break;
                    case "7":
                        busController.AddBus();
                        break;
                    case "0":
                        Console.WriteLine("\n" + new string('=', 50));
                        Console.WriteLine("Thank you for using Bus Ticketing System!");
                        Console.WriteLine(new string('=', 50));
                        running = false;
                        break;
                    default:
                        Console.WriteLine("✗ Invalid option. Please try again.");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// View - Booking
    /// Displays booking information and receipts
    /// </summary>
    public class BookingView
    {
        public void DisplayCreateBookingForm()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("   CREATE BOOKING");
            Console.WriteLine(new string('=', 50));
        }

        public void DisplayBookingConfirmation(BookingOutputDto booking)
        {
            Console.WriteLine("\n✓ Booking created successfully!");
            Console.WriteLine("\nGenerating receipt...");
            ReceiptPrinter.PrintReceipt(booking);
        }

        public void DisplayBooking(BookingOutputDto booking)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("   BOOKING DETAILS");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Passenger: {booking.PassengerName}");
            Console.WriteLine($"Bus: {booking.BusNumber}");
            Console.WriteLine($"From: {booking.Source} To: {booking.Destination}");
            Console.WriteLine($"Seat: {booking.SeatNumber}");
            Console.WriteLine($"Price: ${booking.Price}");
            Console.WriteLine($"Status: {booking.Status}");
            Console.WriteLine(new string('=', 50));
        }

        public void DisplayBookingList(List<BookingOutputDto> bookings)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("   MY BOOKINGS");
            Console.WriteLine(new string('=', 50));

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found.");
            }
            else
            {
                foreach (var booking in bookings)
                {
                    Console.WriteLine($"\nBooking ID: {booking.BookingId}");
                    Console.WriteLine($"Route: {booking.Source} → {booking.Destination}");
                    Console.WriteLine($"Seat: {booking.SeatNumber} | Status: {booking.Status} | Price: ${booking.Price}");
                }
            }
            Console.WriteLine("\n" + new string('=', 50));
        }

        public void DisplayError(string message)
        {
            Console.WriteLine($"\n✗ Error: {message}");
        }
    }

    /// <summary>
    /// View - Bus
    /// Displays bus information and availability
    /// </summary>
    public class BusView
    {
        public void DisplayBuses(List<Bus> buses)
        {
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("   AVAILABLE BUSES");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"{"Bus No.",-10} {"From",-20} {"To",-20} {"Seats",-12} {"Price",-12}");
            Console.WriteLine(new string('-', 80));

            foreach (var bus in buses)
            {
                Console.WriteLine($"{bus.BusNumber,-10} {bus.Source,-20} {bus.Destination,-20} {bus.AvailableSeats,-12} ${bus.PricePerSeat,-11:F2}");
            }
            Console.WriteLine(new string('=', 80));
        }
    }

    // ==================== CONTROLLERS ====================

    /// <summary>
    /// Controller - Home
    /// Handles home view events and navigation
    /// </summary>
    public class HomeController
    {
        private HomeView homeView;

        public HomeController(HomeView homeView)
        {
            this.homeView = homeView;
        }

        public void ShowHomeView()
        {
            homeView.DisplayMenu();
        }
    }

    /// <summary>
    /// Controller - Booking
    /// Handles booking view events and calls booking services
    /// </summary>
    public class BookingController
    {
        private BookingService bookingService;
        private BookingView bookingView;

        public BookingController(BookingService bookingService)
        {
            this.bookingService = bookingService;
            this.bookingView = new BookingView();
        }

        public void CreateBooking()
        {
            try
            {
                bookingView.DisplayCreateBookingForm();

                var inputDto = new BookingInputDto();

                Console.Write("Enter User ID: ");
                inputDto.UserId = Console.ReadLine();

                Console.Write("Enter Bus ID: ");
                inputDto.BusId = Console.ReadLine();

                Console.Write("Enter Seat Number: ");
                inputDto.SeatNumber = Console.ReadLine();

                Console.Write("Enter Passenger Name: ");
                inputDto.PassengerName = Console.ReadLine();

                Console.Write("Enter Passenger Email: ");
                inputDto.PassengerEmail = Console.ReadLine();

                Console.Write("Enter Passenger Phone: ");
                inputDto.PassengerPhone = Console.ReadLine();

                inputDto.TravelDate = DateTime.Now.AddDays(3);

                var booking = bookingService.CreateBooking(inputDto);
                bookingView.DisplayBookingConfirmation(booking);
            }
            catch (BookingException e)
            {
                bookingView.DisplayError(e.Message);
            }
        }

        public void ViewBooking()
        {
            Console.Write("Enter Booking ID: ");
            string bookingId = Console.ReadLine();

            try
            {
                var booking = bookingService.GetBooking(bookingId);
                bookingView.DisplayBooking(booking);
            }
            catch (BookingException e)
            {
                bookingView.DisplayError(e.Message);
            }
        }

        public void ViewUserBookings()
        {
            Console.Write("Enter User ID: ");
            string userId = Console.ReadLine();

            var bookings = bookingService.GetUserBookings(userId);
            bookingView.DisplayBookingList(bookings);
        }

        public void CancelBooking()
        {
            Console.Write("Enter Booking ID to cancel: ");
            string bookingId = Console.ReadLine();

            try
            {
                bookingService.CancelBooking(bookingId);
                Console.WriteLine("✓ Booking cancelled successfully");
            }
            catch (BookingException e)
            {
                bookingView.DisplayError(e.Message);
            }
        }
    }

    /// <summary>
    /// Controller - Bus
    /// Handles bus view events and calls bus services
    /// </summary>
    public class BusController
    {
        private BusService busService;
        private BusView busView;

        public BusController(BusService busService)
        {
            this.busService = busService;
            this.busView = new BusView();
        }

        public void SearchBuses()
        {
            Console.Write("Enter Source: ");
            string source = Console.ReadLine();

            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine();

            var buses = busService.SearchBuses(source, destination);
            if (buses.Count == 0)
            {
                Console.WriteLine("No buses found for this route");
            }
            else
            {
                busView.DisplayBuses(buses);
            }
        }

        public void ViewAllBuses()
        {
            var buses = busService.GetAllBuses();
            if (buses.Count == 0)
            {
                Console.WriteLine("No buses available");
            }
            else
            {
                busView.DisplayBuses(buses);
            }
        }

        public void AddBus()
        {
            try
            {
                var bus = new Bus();

                Console.Write("Enter Bus Number: ");
                bus.BusNumber = Console.ReadLine();

                Console.Write("Enter Source: ");
                bus.Source = Console.ReadLine();

                Console.Write("Enter Destination: ");
                bus.Destination = Console.ReadLine();

                Console.Write("Enter Total Seats: ");
                int seats = int.Parse(Console.ReadLine());
                bus.TotalSeats = seats;
                bus.AvailableSeats = seats;

                Console.Write("Enter Price Per Seat: ");
                bus.PricePerSeat = double.Parse(Console.ReadLine());

                Console.Write("Enter Bus Type (AC/NON_AC/SLEEPER): ");
                bus.BusType = Console.ReadLine();

                Console.Write("Enter Operator Name: ");
                bus.OperatorName = Console.ReadLine();

                busService.CreateBus(bus);
                Console.WriteLine("✓ Bus added successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"✗ Error adding bus: {e.Message}");
            }
        }
    }

    // ==================== APPLICATION STARTUP ====================

    /// <summary>
    /// Program - Application Entry Point
    /// Initializes dependencies and starts the application
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Initialize sample data
                InitializeSampleData();

                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("   BUS TICKETING SYSTEM");
                Console.WriteLine(new string('=', 50) + "\n");

                // Configure dependencies
                var bookingService = new BookingService();
                var busService = new BusService(bookingService.GetBusRepository());

                var bookingController = new BookingController(bookingService);
                var busController = new BusController(busService);

                var homeView = new HomeView(bookingController, busController);
                var homeController = new HomeController(homeView);

                // Start application
                homeController.ShowHomeView();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Application failed to start: {e.Message}");
            }
        }

        static void InitializeSampleData()
        {
            // Sample buses (will be used when initialized)
            Console.WriteLine("✓ Initializing sample data...");
        }
    }
}
