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

    public static Driver New(string name, int? adrQualificationsFlags, Branch branch)
    {
        var driver = new Driver
        {
            Guid = System.Guid.NewGuid().ToString(), HireDate = DateTime.Now, Name = name, HoursWorkedPerWeek = 0,
            TotalHoursWorked = 0
        };
        Reinstate();
        driver.SetAdrQualificationsFlags(adrQualificationsFlags);
        driver.SetBranch(branch);

        return driver;
    }

    public void Reinstate()
    {
        IsAvailable = true;
        DismissalDate = null;
    }
    
    public void Dismiss()
    {
        IsAvailable = false;
        DismissalDate = DateTime.Now;
    }

    public void AddHoursWorked(double hoursWorked)
    {
        HoursWorkedPerWeek += hoursWorked;
        TotalHoursWorked += hoursWorked;
    }

    public void ResetHoursWorkedPerWeek() => HoursWorkedPerWeek = 0;
    
    public void SetAdrQualificationsFlags(int? adrQualificationsFlags)
    {
        if (adrQualificationsFlags != null)
            if (!AdrDriverQualificationsFlags.IsFlagCombination(adrQualificationsFlags.Value))
                throw new ArgumentOutOfRangeException(nameof(adrQualificationsFlags), adrQualificationsFlags,
                    "The AdrQualificationsFlags describe 4 ADR driver qualifications. This means that the" +
                    " value of their combination must be in the range [0; 2^4 (16)).");
            
        AdrQualificationsFlags = adrQualificationsFlags;
    }

    public void SetBranch(Branch branch)
    {
        Branch = branch;
        BranchGuid = branch.Guid;
    }

    public override string ToString() => $"Name = {Name}";

    public string Guid { get; private set; } = null!;
    
    public DateTime HireDate { get; private set; }
    
    public DateTime? DismissalDate { get; private set; }
    
    public double HoursWorkedPerWeek { get; private set; }
    
    public double TotalHoursWorked { get; private set; }
    
    public int? AdrQualificationsFlags { get; private set; }
    
    public string BranchGuid { get; private set; } = null!;

    public virtual Branch Branch { get; private set; } = null!;
    
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();

    public string Name { get; set; } = null!;
    
    public bool IsAvailable { get; set; }
}
