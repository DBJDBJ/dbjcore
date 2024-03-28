<h1>DBJ CORE KONTALOG</h1>

While in container there is no logging library necessary. Gasp!

Code (in any prog lang) written to run inside a container has no other obligation towards logging but to use console out stream. Aka STDOUT and STDERR.

There is very little useful one can do while "improving the logging" experience from a container hosted code.

Basically I  use my `kontalog` to add iso time stamps, tags for the level, and make it asynchronous.

## Test 

Here is the full program using and hammering the `kontalog`

```c#
#define stress_test

using Log = dbj.Kontalog.Kontalog;

Log.info("Hello, KONTALOG logging Q!");

#if stress_test
Log.info("KONTALOG stress test will attempt to run forever.");

long counter = 0L;
while (true)
{
    counter = 1 + (counter % long.MaxValue);
    // in prod only fatal will be shown
    Log.fatal("Counter: {0,12} !", counter);
}
#else
for (System.Int32 k = 0; k < 0xF; ++k)
    Log.debug("Hello, {0,12} !", k);
Log.fatal("Loop (size = {0}) done.", 0xF);

Log.info("Done. Press ENTER");
Console.ReadLine();
#endif

// WARNING: Console.Writeline will be out of sync, if used 
```

## Opinionated Concepts

1. log levels are over engineering, kontalog for non DEBUG code outputs only `fatal` messages.

## TODO

1. make it work in presence of multiple threads

## Integrate with dbj core log?

   1. this is questionable.
   2. dbj core log uses Serilog extensively
   3. Serilog is ok but in container we do not need Serilog. 
      1. Only if we envisage same code will one day run somewhere outside of the container
    4. alternatively I can use kontalog for situations where dbj log serilog outputs to console only.

But. Serilog is good but far from trivial and light lib. Why using it just to do the same job as super light `kontalog` does?

Why don't I just use `kontalog` as a front to **ALL** logging?

And sometimes in some future, code using `kontalog` can be easily adopted to output to `Serilog` or `NLOG` or whatever else.

Code is very simple by no means, and I think you are more than capable to make it use `Serilog` or whatever else you manager might require.

Hint. Everything goes out through this single line

```c#
System.Console.WriteLine(data);
```