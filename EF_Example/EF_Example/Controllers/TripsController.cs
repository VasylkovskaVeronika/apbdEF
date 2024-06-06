using EF_Example.Data;
using EF_Example.DTOs;
using EF_Example.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EF_Example.Controllers;

public class TripsController: ControllerBase
{
     private readonly ScaffoldContext _context;

        public TripsController(ScaffoldContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (_context.Trips == null)
            {
                return NotFound();
            }

            var trips = await _context.Trips.Select(e => new TripsDTO()
                {
                    Name = e.Name,
                    DateFrom = e.DateFrom,
                    MaxPeople = e.MaxPeople,
                    Clients = e.ClientTrips.Select(e => new ClientDTO()
                    {
                        FirstName = e.IdClientNavigation.FirstName,
                        LastName = e.IdClientNavigation.LastName
                    })
                })
                .OrderBy(e => e.DateFrom)
                .Skip((page-1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalTrips = await _context.Trips.CountAsync();
            var totalPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

            return Ok(new
            {
                pageNum = page,
                pageSize,
                allPages = totalPages,
                trips
            });
        }
        
        [HttpPost("{idTrip}/clients")]
        public async Task<ActionResult> AddClientToTrip(ClientTripDTO tripaddClientDto, int idTrip)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == tripaddClientDto.Pesel);
            if (client == null)
            {
                return BadRequest("Client with the given PESEL number does not exist.");
            }

            var trip_client = await _context.ClientTrips.FirstOrDefaultAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == idTrip);
            if (trip_client != null)
            {
                return BadRequest("This client has already registered for this trip.");
            }
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
            if (trip == null)
            {
                return BadRequest("Trip with the given ID does not exist.");
            }

            if (trip.DateFrom < DateTime.Now)
            {
                return BadRequest("The trip has already occurred.");
            }
           

            var newClientTrip = new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = trip.IdTrip,
                RegisteredAt = DateTime.Now,
                PaymentDate = tripaddClientDto.PaymentDate
            };

            _context.ClientTrips.Add(newClientTrip);
            await _context.SaveChangesAsync();

            return Ok();
        }
}