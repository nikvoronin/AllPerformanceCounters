# All Performance Counters

Enumerates and prints all system performance counters

## System Utility

You can monitor your Windows system over `Performance Monitor`:

> `Win+R` &rarr; `perfmon` &rarr; `OK`

![tools_perfmon_addCounters](https://user-images.githubusercontent.com/11328666/152036463-2d7cb2ae-4ef5-40ad-a990-5ffec048bf44.png)

## Code First

MSDN: [System.Diagnostics.PerformanceCounter](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecounter?view=dotnet-plat-ext-6.0)

It is important to select right thread culture:

```c#
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
```

- Category
  - Counter 1
    - Instance01 of counter 1
    - Instance02 of counter 1
    - ...
  - Counter 2
  - ...
- Category ...

## How To Measure

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

### Memory Usage

Some categories contains `instances` but some not. Memory has not:

```c#
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var memoryCategory = PerformanceCounterCategory.GetCategories()
    .FirstOrDefault( cat => cat.CategoryName == "Memory" );
var memoryUsage = memoryCategory?.GetCounters()
    .First( cnt => cnt.CounterName == "Available MBytes" );

while ( !Console.KeyAvailable ) {
    Console.WriteLine( $"Memory usage: { memoryUsage?.NextValue() ?? 0f } MBytes" );
    Thread.Sleep( 300 );
}
```

### Temperature

Temperature in Kelvin degrees.

> Chips used for things like fan control have an internal structure that allows them to be set up so that zone 1 might be ram+cache,
            zone 2 cpu,
            zone 3 video card. Pretty much the way you can divide a home into zones for heating and cooling.

- [Thermal management in Windows](https://docs.microsoft.com/en-us/windows-hardware/design/device-experiences/design-guide). This PC thermal management design guide provides information about how to determine the PC temperature values that are "too hot" and "too cold." // 08/24/2021
- [ACPI Specification 6.4 - Thermal Management - Thermal Objects](https://uefi.org/specs/ACPI/6.4/11_Thermal_Management/thermal-objects.html?highlight=tzd)

```c#
const double AbsZeroK = 273.15;
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var thermalCategory = PerformanceCounterCategory.GetCategories()
    .FirstOrDefault( cat => cat.CategoryName == "Thermal Zone Information" );
var countersInCategory = thermalCategory?.GetCounters( @"\_TZ.TZ00" );
var temperature = countersInCategory?
    .First( cnt => cnt.CounterName == "High Precision Temperature" ); // "Temperature"

while ( !Console.KeyAvailable ) {
    Console.WriteLine( $"Temperature: { (( temperature?.NextValue() ?? AbsZeroK * 10.0 ) / 10.0 - AbsZeroK):0.#} °C" );
    Thread.Sleep( 300 );
}
```

Command line with PowerShell:

```powershell
Get-WmiObject -Class Win32_PerfFormattedData_Counters_ThermalZoneInformation |Select-Object Name,Temperature
```

Instances for thermal zones (TZ):

- \_TZ.THRM
- \_TZ.TZ00,
            \_TZ.TZ01,
            \_TZ.TZ02
- \_TZ.EXTZ
- \_TZ.GFXZ - GPU or graphics card
- \_TZ.LOCZ
- \_TZ.BATZ - Battery
- \_TZ.CPUZ - CPU
- \_TZ.CHGZ
- \_TZ.TSZ0
- \_TZ.GETP
- \_TZ.CHGZ._CRT
- \_TZ.HPTZ
- \_TZ.DTSZ - digital thermal sensor
- \_TZ.SKNZ
- \_TZ.FDTZ - fan speed not temperature

Example listing (HP ProBook G7 laptop):

```plain
Thermal Zone Information
    >>> INSTANCES
    \_TZ.EXTZ
        >>> COUNTERS
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
    \_TZ.GFXZ
        >>> COUNTERS
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
    \_TZ.LOCZ
        >>> COUNTERS
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
    \_TZ.BATZ
        >>> COUNTERS
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
    \_TZ.CPUZ
        >>> COUNTERS
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
    \_TZ.CHGZ
        >>> COUNTERS
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
```

Acer Nitro 5 laptop:

```plain
CATEGORY: Thermal Zone Information
    INSTANCE: \_TZ.TZ01
        >>> COUNTERS:
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
    INSTANCE: \_TZ.TZ00
        >>> COUNTERS:
        Temperature
        % Passive Limit
        Throttle Reasons
        High Precision Temperature
```
