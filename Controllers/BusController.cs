using System;
using System.Collections.Generic;
using BusTicketingSystem.Models;
using BusTicketingSystem.Repositories;

namespace BusTicketingSystem.Controllers
{
    /// <summary>
    /// Controller - Bus
    /// Handles business logic for bus operations (MVC Pattern)
    /// </summary>
    public class BusController
    {
        private BusRepository busRepository;

        public BusController()
        {
            busRepository = new BusRepository();
        }

        public void RegisterBus(Bus bus)
        {
            busRepository.Save(bus);
        }

        public Bus GetBusDetails(string busId)
        {
            return busRepository.FindById(busId);
        }

        public List<Bus> SearchBusesByRoute(string source, string destination)
        {
            return busRepository.FindByRoute(source, destination);
        }

        public List<Bus> GetAllBuses()
        {
            return busRepository.FindAll();
        }

        public void UpdateBusAvailability(string busId, int availableSeats)
        {
            Bus bus = busRepository.FindById(busId);
            if (bus != null)
            {
                bus.AvailableSeats = availableSeats;
                busRepository.Update(bus);
            }
        }
    }
}
