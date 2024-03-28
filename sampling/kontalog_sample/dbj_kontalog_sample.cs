#define stress_test

using dbjcore;
using Log = dbjcore.Kontalog;
using Utl = dbjcore.DBJcore;

Log.info("Running on: " + Utl.my_domain(false));
Log.info("Program: " + Utl.my_domain() + ", purpose: KONTALOG logging sample!");

var LOOP_LENGTH = DBJCfg.get<int>("LOOP_LENGTH", 0);
Log.info("LOOP_LENGTH is defined in " + DBJCfg.instance.config_file_name  + " as: " + LOOP_LENGTH);


long counter = 0L;
while (counter != LOOP_LENGTH)
{
    counter = 1 + (counter % LOOP_LENGTH);
    // Kontalog in prod only fatal will be shown
    Log.fatal("Counter: {0,12} !", counter);
}

Log.info("Done");
// WARNING! Console.Writeline will be out of sync, if used 