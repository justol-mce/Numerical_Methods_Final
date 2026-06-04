using System;
using System.Collections.Generic;
using System.Linq;
using BusTicketingSystem.Models;

namespace BusTicketingSystem.Repositories
{
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
}
