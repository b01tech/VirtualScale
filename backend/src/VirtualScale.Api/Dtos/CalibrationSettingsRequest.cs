namespace VirtualScale.Api.Dtos;

public record CalibrationSettingsRequest(
    int NumberOfCells,
    string Unit,
    decimal CapMax,
    int Division,
    int DecimalPlaces,
    decimal ReferenceWeight
);
