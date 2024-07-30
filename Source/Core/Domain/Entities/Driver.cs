using Domain.Constants;

namespace Domain.Entities;

public class Driver
{
    // Раньше тут была "обходная" инициализация вычисляемых свойств с помощью выделения инициализации полей, на которые
    // эти свойства ссылаются, в отдельный приватный конструктор. Выполнять этот прием нужно для EF в двух случаях.
    // Первый: вычисляемое свойство уставливает значения не только себе, но и другим свойствам (прямо или опосредованно
    // через их поля), а нам при получении записи из БД с помощью EF требуется установка значения только одному
    // свойству. Второй: вычисляемое свойство не просто уставливает себе значение, а сперва вычисляет его, а нам при
    // получении записи из БД с помощью EF требуется установка первоначально вычисленного значения. Пример такого
    // случая: хэширующее свойство пароля - если устанавливать его как обычно, то при первом получении записи из БД
    // получим хэш хэша пароля, при втором хэш хэша хэша пароля и т. д. Повторюсь: использовать этот прием нужно ТОЛЬКО
    // в этих двух случая, а не для каждого вычисляемого свойства. Примеры бессмысленности применения этого приема:
    // 1. Вычисляемое свойство просто проверяет поступающее значение на корректность. Хотя, конечно же, гарантируется,
    // что все записи в БД будут валидны, так что этот прием все-таки можно (но необязательно) использовать для
    // оптимизации, избавившись от бессмысленной проверки. Хотя если проверка предельно легковесна (например, проверка
    // int на принадлежность интервалу), то этот прием, вероятно, оверхед;
    // 2. Свойство имеет только геттер. В этом случае инициализация свойства возможна только из конструктора, так что
    // этот прием, на первый взгляд, необходим. Но зачем тебе вообще GetOnly-свойство? Добавь приватный сеттер и не
    // парься - для клиентов класса абсолютно ничего не изменится.
    // Простой способ (который я и применил везде) делать все красиво, избегая этот прием - выделять установку
    // вычисляемых полей в отдельные методы-сеттеры.
    private Driver() { }

    public static Driver New(string name, string branchGuid, int? adrQualificationFlag, bool adrQualificationOfTank)
    {
        var driver = new Driver
        {
            Guid = System.Guid.NewGuid().ToString(), HireDate = DateTime.Now, DismissalDate = null,
            HoursWorkedPerWeek = 0, TotalHoursWorked = 0, AdrQualificationFlag = null,
            AdrQualificationOfTank = false, BranchGuid = branchGuid, Name = name, IsAvailable = true
        };
        
        if (adrQualificationFlag != null)
            driver.QualifyAdr(adrQualificationFlag.Value);
        if (adrQualificationOfTank)
            driver.QualifyAdrTank();

        return driver;
    }

    public void Reinstate()
    {
        if (DismissalDate == null)
            throw new InvalidOperationException($"Driver {Guid}. Only a dismissed driver can use the reinstatement operation.");
        
        IsAvailable = true;
        DismissalDate = null;
    }
    
    public void Dismiss()
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        
        IsAvailable = false;
        DismissalDate = DateTime.Now;
    }

    public void AddHoursWorked(double hoursWorked)
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        
        HoursWorkedPerWeek += hoursWorked;
        TotalHoursWorked += hoursWorked;
    }

    public void ResetHoursWorkedPerWeek()
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        
        HoursWorkedPerWeek = 0;
    }

    public void DequalifyAdr()
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        
        AdrQualificationOfTank = false;
        AdrQualificationFlag = null;
    }
    
    public void QualifyAdr(int adrQualificationFlag)
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        if (!AdrDriverQualificationsFlags.IsFlag(adrQualificationFlag))
            throw new ArgumentOutOfRangeException(nameof(adrQualificationFlag), adrQualificationFlag,
                $"Driver {Guid}. AdrQualificationFlag describes the 3 ADR driver qualifications. Valid values: Base (917440), BaseAndClass7 (1048512), BaseAndClass1 (917503), Full (1048575).");
        
        AdrQualificationFlag = adrQualificationFlag;
    }
    
    public void QualifyAdrTank()
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        if (AdrQualificationFlag == null)
            throw new InvalidOperationException(
                $"Driver {Guid}. The driver cannot simultaneously have an ADR qualification for the transportation of tanks and not have any other ADR qualification");

        AdrQualificationOfTank = true;
    }

    public void DequalifyAdrTank()
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        
        AdrQualificationOfTank = false;
    }

    public void SetBranch(string branchGuid)
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        
        BranchGuid = branchGuid;
    }
    
    public void SetName(string name)
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");
        
        Name = name;
    }

    public void Work()
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");

        IsAvailable = true;
    }
    
    public void Rest()
    {
        if (DismissalDate != null)
            throw new InvalidOperationException($"Driver {Guid}. A dismissed driver can only use the reinstatement operation.");

        IsAvailable = false;
    }

    public override string ToString() => $"Name = {Name}";

    public string Guid { get; private set; } = null!;
    
    public DateTime HireDate { get; private set; }
    
    public DateTime? DismissalDate { get; private set; }
    
    public double HoursWorkedPerWeek { get; private set; }
    
    public double TotalHoursWorked { get; private set; }
    
    public int? AdrQualificationFlag { get; private set; }
    
    public bool AdrQualificationOfTank { get; private set; }
    
    public string BranchGuid { get; private set; } = null!;
    
    public string Name { get; private set; } = null!;
    
    public bool IsAvailable { get; private set; }
    
    public virtual Branch Branch { get; private set; } = null!;
    
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
}