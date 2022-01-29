// NuGet package `System.Diagnostics.PerformanceCounter` have to be installed
// .NET 6

#nullable disable
using System.Globalization;
using System.Diagnostics;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
using var writer = new StreamWriter( Console.OpenStandardOutput() );

var cats = PerformanceCounterCategory.GetCategories( Environment.MachineName );
foreach ( var cat in cats ) {
    writer.WriteLine( $"CATEGORY: {cat.CategoryName}" );

    var instances = Array.Empty<string>();
    try {
        instances = cat.GetInstanceNames();
        var hasNotInstance = instances.Length == 0;
        if ( hasNotInstance )
            PrintCounters( cat );
        else {
            foreach ( var instanceName in instances )
                PrintCounters( cat, instanceName );
        }
    }
    catch { }

    void PrintCounters( PerformanceCounterCategory cat, string instanceName = "" )
    {
        if ( !string.IsNullOrEmpty( instanceName ) )
            writer.Write( $"\tINSTANCE: {instanceName}\n\t" );

        writer.WriteLine( "\tCOUNTERS:" );

        var counters = string.IsNullOrEmpty( instanceName )
            ? cat.GetCounters()
            : cat.GetCounters( instanceName );

        foreach ( var counter in counters )
            writer.WriteLine( $"\t\t{counter.CounterName}" );
    }
}

writer.WriteLine( "END." );