using System.Collections;

namespace Domain.Entities;

public class Truck
{
    private Truck() { }

    private Truck(DateTime? dismissalDate)
    {
        _dismissalDate = dismissalDate;
    }

    public static Truck New(string name, bool certificatAdr, Branch branch) => new Driver(null)
    {
        Guid = System.Guid.NewGuid().ToString(), HireDate = DateTime.Now, Name = name, IsAvailable = true,
        CertificatAdr = certificatAdr, HoursWorkedPerWeek = 0, TotalHoursWorked = 0, BranchAddress = branch.Address,
        Branch = branch
    };

    public void SetBranch(Branch branch)
    {
        Branch = branch;
        BranchAddress = branch.Address;
    }

    public decimal CalculateOrderPrice(Order order)
    {
        var weightPricePerKilometer = WeightPrice * order.CargoWeight;
        var volumePricePerKilometer = VolumePrice * order.CargoVolume;
        var totalPricePerKilometer = (weightPricePerKilometer + volumePricePerKilometer) * PricePerKm;
        var orderPrice = totalPricePerKilometer * (decimal)order.DistanceInKm;

        return orderPrice;
    }

    public override string ToString() => Number;

    public string Guid { get; private set; } = null!;

    public string Number { get; set; } = null!;

    // В соответствии с действующим на момент 01.07.2024 ГОСТ Р 57479, существует 20 подклассов опасности грузов. Для
    // перевозки груза с тем или иным подклассом опасности, фура и ее полуприцеп должны быть сертифицированы на
    // соответствие всем требованиям перевозки грузов с данным подклассом опасности. Флаговое свойство ниже как раз и
    // моделирует наличие/отсутствие у совокупности Фура-Полуприцеп сертификата по всем 20 подклассам. Например, если
    // 17-ый бит равен 1, то Фура-Полуприцеп имеет сертификат по 17 подклассу (в терминах ГОСТ Р 57479 это будет
    // подкласс 6.2 - "Инфекционные вещества")
    public int HazardClassesFlag
    {
        get => _hazardClassesFlag;
        set
        {
            if (value is < 0 or > 0b1111_1111_1111_1111_1111)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The flag describes 20 attributes. This means that its value must be in the range [0; 2^20 (1 048 576)].");
            
            if (value != 0)
            {
                // TODO: Это всего-лишь концептуальный код для преобразования Int32 в bool[], моделирующий массив битов
                var bitArray = new BitArray(new[] { _flagsOfAvailableHazardClasses });
                var bits = new bool[bitArray.Length];
                bitArray.CopyTo(bits, 0);
            }
            _hazardClassesFlag = value;
        }
    }

    public bool IsAvailable { get; set; }

    public decimal VolumeMax { get; set; }
    
    public decimal VolumePrice { get; set; }

    public decimal WeightMax { get; set; }
    
    public decimal WeightPrice { get; set; }
    
    public decimal PricePerKm { get; set; }
    
    public string BranchAddress { get; private set; } = null!;

    public virtual Branch Branch { get; private set; } = null!;

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();

    private int _hazardClassesFlag;
}
