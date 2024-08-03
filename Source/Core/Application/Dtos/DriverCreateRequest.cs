namespace Application.Dtos;

public record DriverCreateRequest(string Name, string BranchGuid, int? AdrQualificationFlag, bool AdrQualificationOfTank);