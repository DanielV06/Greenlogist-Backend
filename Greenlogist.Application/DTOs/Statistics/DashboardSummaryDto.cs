namespace Greenlogist.Application.DTOs.Statistics
{
    /// <summary>
    /// DTO for the producer dashboard summary (read model).
    /// Corresponds to the data shown in ProductoresDashboard.vue.
    /// </summary>
    public record DashboardSummaryDto(
        int RegisteredProductsCount,
        int RequestedTransportsCount,
        int CompletedOrdersCount, // Used as a proxy for "statistics" in the dashboard
        decimal TotalSalesAmount, // New: Total sales amount for dashboard
        decimal TotalProductsSoldKg, // New: Total products sold in kg for dashboard
        decimal EnvironmentalImpactMetric // Placeholder for environmental impact
    );
}
