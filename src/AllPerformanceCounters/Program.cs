// NuGet package `System.Diagnostics.PerformanceCounter` have to be installed
// Less functional way there https://gist.github.com/nikvoronin/643b9dc775c648a0397fdf853f70e7b0
// .NET 6

#nullable disable
using System.Diagnostics;
using System.Globalization;
using System.Text;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
using var writer = new StreamWriter( Console.OpenStandardOutput() );

var categories = PerformanceCounterCategory.GetCategories( Environment.MachineName );
foreach ( var category in categories ) {
    writer.WriteLine( $"CATEGORY: {category.CategoryName}" );

    string[] instances = ResultOrDefault( ( cat ) => cat.GetInstanceNames(), category );

    // fallthrough if there are no instances
    foreach ( var instanceContent in EnumerateInstances( instances, category ) )
        writer.Write( instanceContent );

    var noInstances = instances.Length == 0;
    if ( noInstances )
        writer.Write( ResultOrDefault( ( cat ) => cat.GetCounters().ToText(), category ) );
}

writer.WriteLine( "END." );

static IEnumerable<string> EnumerateInstances( IEnumerable<string> instances, PerformanceCounterCategory category )
{
    foreach ( var instance in instances )
        yield return
            $"\tINSTANCE: {instance}\n\t"
            + ResultOrDefault( ( cat ) => cat.GetCounters( instance ), category ).ToText();
}

static TResult ResultOrDefault<TArg, TResult>( Func<TArg, TResult> f, TArg x )
{
    try { return f( x ); }
    catch { }
    return default;
}

public static class Extensions
{
    public static string ToText( this IEnumerable<PerformanceCounter> counters )
        => counters?.Aggregate( new StringBuilder( "\tCOUNTERS:\n" ),
            ( textBuilder, counter ) => textBuilder.AppendLine( $"\t\t{counter.CounterName}" ) )
        .ToString()
        ?? string.Empty;
}
