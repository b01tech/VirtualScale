namespace VirtualScale.Api.Dtos;

public record CalibrationSettingsRequest(
    int NumberOfCells,
    decimal CapMax,
    int Division,
    int DecimalPlaces,
    decimal ReferenceWeight
);

