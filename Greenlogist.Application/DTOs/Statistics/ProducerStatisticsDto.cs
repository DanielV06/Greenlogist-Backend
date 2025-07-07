namespace Greenlogist.Application.DTOs.Statistics
{
    /// <summary>
    /// DTO for detailed producer statistics (read model).
    /// Corresponds to the data shown in Estadisticas.vue.
    /// </summary>
    public record ProducerStatisticsDto(
        int TotalSalesCount,
        decimal TotalSalesAmount,
        decimal TotalProductsSoldKg,
        int TotalTransportRequests,
        int CompletedTransportRequests,
        decimal TotalEnvironmentalImpact // Placeholder for environmental impact
    );
}
