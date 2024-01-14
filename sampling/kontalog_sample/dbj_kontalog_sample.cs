#define stress_test

using Log = dbjcore.Kontalog;
using Utl = dbjcore.DBJcore;

Log.info("Running on: " + Utl.my_domain(false));
Log.info("Program: " + Utl.my_domain() + ", purpose: KONTALOG logging sample!");

#if stress_test
Log.info( " This KONTALOG stress test will attempt to run forever.");

long counter = 0L;
while (true)
{
    counter = 1 + (counter % long.MaxValue);
    // Kontalog in prod only fatal will be shown
    Log.fatal("Counter: {0,12} !", counter);
}
#else
for (System.Int32 k = 0; k < 0xF; ++k)
    Log.debug("Hello, {0,12} !", k);
Log.fatal("Loop (size = {0}) done.", 0xF);

Log.info("Done. Press ENTER");
Console.ReadLine();
#endif

// WARNING! Console.Writeline will be out of sync, if used 