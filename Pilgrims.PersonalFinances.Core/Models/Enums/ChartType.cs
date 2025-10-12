using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models.Enums
{
    /// <summary>
    /// Chart types available for report visualization
    /// </summary>
    public enum ChartType
    {
        [Display(Name = "Bar Chart")]
        BarChart,
        
        [Display(Name = "Line Chart")]
        LineChart,
        
        [Display(Name = "Pie Chart")]
        PieChart,
        
        [Display(Name = "Area Chart")]
        AreaChart,
        
        [Display(Name = "Donut Chart")]
        DonutChart,
        
        [Display(Name = "Stacked Bar Chart")]
        StackedBarChart,
        
        [Display(Name = "Trend Line")]
        TrendLine,
        
        [Display(Name = "Scatter Plot")]
        ScatterPlot,
        
        [Display(Name = "Waterfall Chart")]
        WaterfallChart,
        
        [Display(Name = "Gauge Chart")]
        GaugeChart,
        
        [Display(Name = "Table")]
        Table
    }
}
