using EF_Example.Models;

namespace EF_Example.DTOs;

public class TripsDTO
{
    public string Name { get; set; } = null!;
    public DateTime DateFrom { get; set; }
    public int MaxPeople { get; set; }
    public virtual IEnumerable<ClientDTO> Clients { get; set; } = new List<ClientDTO>();
}