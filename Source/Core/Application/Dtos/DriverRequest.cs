namespace Application.Dtos;

public record DriverRequest(string Name, string BranchGuid, int? AdrQualificationFlag, bool AdrQualificationOfTank);