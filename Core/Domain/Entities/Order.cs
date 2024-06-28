using Domain.Constants;

namespace Domain.Entities;

public class Order
{
    public Order()
    {
        Guid = System.Guid.NewGuid().ToString();
        DateBegin = DateTime.Now;
    }

    private Order(string guid, DateTime dateBegin)
    {
        Guid = guid;
        DateBegin = dateBegin;
    }
    
    public string Guid { get; } = null!;

    public DateTime DateBegin { get; }

    public DateTime? DateEnd
    {
        get => _dateEnd;
        set
        {
            var isAvailable = value != null;
            _dateEnd = value;
            Truck.IsAvailable = isAvailable;
            Driver1.IsAvailable = isAvailable;
            if (Driver2 != null)
                Driver2.IsAvailable = isAvailable;
        }
    }

    public string Address { get; set; } = null!;
    
    public decimal StartLatitude { get; set; }

    public decimal StartLongtitude { get; set; }

    public decimal EndLatitude { get; set; }

    public decimal EndLongtitude { get; set; }
    
    public decimal LengthInKilometers { get; set; }

    public decimal ClassAdr
    {
        get => _classAdr;
        set
        {
            if (!ClassesAdr.ClassExists(value))
                throw new ArgumentException($"{value} is not an class adr", nameof(value));
            _classAdr = value;
        }
    }

    public decimal Price { get; set; }

    public decimal CargoVolume { get; set; }

    public decimal CargoWeight { get; set; }
    
    public string UserLogin { get; set; } = null!;

    public string TruckNumber { get; set; } = null!;
    
    public string Driver1Id { get; set; } = null!;
    
    public string? Driver2Id { get; set; }

    public virtual Truck Truck { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Driver Driver1 { get; set; } = null!;

    public virtual Driver? Driver2 { get; set; }

    private decimal _classAdr;

    private DateTime? _dateEnd;
}