# All Performance Counters

Enumerates and prints all system performance counters

## System Utility

You can monitor your Windows system over `perfmon`s:

> `Win+R` &rarr; `perfmon` &rarr; `OK`

## Code First

MSDN: [System.Diagnostics.PerformanceCounter](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecounter?view=dotnet-plat-ext-6.0)

It is important to select right thread culture:

```c#
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
```

### CPU Load

```c#
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var processorCategory = PerformanceCounterCategory.GetCategories()
    .FirstOrDefault( cat => cat.CategoryName == "Processor" );
var countersInCategory = processorCategory?.GetCounters( "_Total" );
var cpuLoadCounter = countersInCategory?
    .First( cnt => cnt.CounterName == "% Processor Time" );

while ( !Console.KeyAvailable ) {
    Console.WriteLine( $"CPU Load: { cpuLoadCounter?.NextValue() ?? 0f }" );
    Thread.Sleep( 300 );
}
```
