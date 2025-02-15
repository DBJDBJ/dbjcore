#define stress_test

//
// Note on usage:
//
// this makes visible ALL thingts
// using dbjcore;
// this renames to Log
// using Log = dbjcore.Kontalog;
//
// can use like this: using Utl = dbjcore.DBJcore;
//
// Can also use like this:
// using static DBJcore;
// after which you can just use the method names 
// from the class DBJcore in here
// without a class name and dot in the front, for example:
// Writeln( Whoami() );
// obviously that is nice but can also provoke name clash
// with third party code
// but let's try in this trivial program
using static dbjcore.DBJcore;
using static dbjcore.DBJCfg;
using static dbjcore.Kontalog;
// now we have an illusion of functional programing code :)

info("Running on: " + my_domain(false));
info("Program: " + my_domain() + ", purpose: KONTALOG logging sample!");

var LOOP_LENGTH = get<int>("LOOP_LENGTH", 0);
info("LOOP_LENGTH is defined in " + instance.config_file_name  + " as: " + LOOP_LENGTH);


long counter = 0L;
while (counter != LOOP_LENGTH)
{
    counter = 1 + (counter % LOOP_LENGTH);
    // Kontalog in prod
    // Only fatal will be shown
    fatal("Counter: {0,12} !", counter);
}

info("Done");
// WARNING! Console.Writeline will be out of sync, if used 

// Note
// there is a Q in front of a Kontalog 
// and there is a separate thread get-ing from that Q and writing
// to the console, this oder of logging is preserved
// but logging is not blocking 