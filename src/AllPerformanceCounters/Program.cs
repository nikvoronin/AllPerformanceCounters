// NuGet package `System.Diagnostics.PerformanceCounter` have to be installed
// .NET 6

#nullable disable
using System.Globalization;
using System.Diagnostics;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
using var writer = new StreamWriter( Console.OpenStandardOutput() );

var categories = PerformanceCounterCategory.GetCategories( Environment.MachineName );
foreach ( var cat in categories ) {
    writer.WriteLine( $"CATEGORY: {cat.CategoryName}" );

    string[] instances = default;
    try {
        instances = cat.GetInstanceNames();
    }
    catch { }

    writer.WriteInstances( cat, instances );

    var noInstances = instances.Length == 0;
    if ( noInstances ) {
        try {
            var counters = cat.GetCounters();
            writer.WriteCounters( counters );
        }
        catch { }
    }
}

writer.WriteLine( "END." );

public static class Extensions
{
    public static void WriteInstances( this StreamWriter writer,
        PerformanceCounterCategory category,
        IEnumerable<string> instances )
    {
        foreach ( var instanceName in instances ) {
            try {
                var counters = category.GetCounters( instanceName );
                writer.Write( $"\tINSTANCE: {instanceName}\n\t" );
                writer.WriteCounters( counters );
            }
            catch { }
        }
    }

    public static void WriteCounters( this StreamWriter writer,
        IEnumerable<PerformanceCounter> counters )
    {
        writer.WriteLine( "\tCOUNTERS:" );
        foreach ( var counter in counters )
            writer.WriteLine( $"\t\t{counter.CounterName}" );
    }
}