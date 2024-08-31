using System.Globalization;
using Application.Dtos.Driver;

namespace Web.Dtos;

public record Driver
{
    public Driver(Response driver)
    {
        Guid = driver.Guid;
        HireDate = driver.HireDate.ToString(CultureInfo.InvariantCulture);
        HoursWorkedPerWeek = driver.HoursWorkedPerWeek.ToString(CultureInfo.InvariantCulture);
        TotalHoursWorked = driver.TotalHoursWorked.ToString(CultureInfo.InvariantCulture);
        AdrQualificationFlag = driver.AdrQualificationFlag ?? "null";
        AdrQualificationOfTank = driver.AdrQualificationOfTank ? "true" : "false";
        Name = driver.Name;
        IsAvailable = driver.IsAvailable ? "true" : "false";
        
        if (driver.DismissalDate == null)
        {
            DismissalDate = "Работает";
            ButtonValue = "Dismiss";
            ButtonText = "Уволить";
        }
        else
        {
            DismissalDate = driver.DismissalDate.Value.ToString(CultureInfo.InvariantCulture);
            ButtonValue = "Reinstate";
            ButtonText = "Восстановить";
        }
    }
    
    public string Guid { get; }
    
    public string HireDate { get; }
    
    public string DismissalDate { get; }
    
    public string HoursWorkedPerWeek { get; }
    
    public string TotalHoursWorked { get; }
    
    public string AdrQualificationFlag { get; set; }
    
    public string AdrQualificationOfTank { get; set; }
    
    public string Name { get; }
    
    public string IsAvailable { get; set; }
    
    public string ButtonValue { get; }
    
    public string ButtonText { get; }
    
    public string? Color { get; set; }
}