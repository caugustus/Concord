Concord
=======

A parallel unit test runner, with poor documentation.

This runs tests in parallel by categories, as opposed to tests or features which is what everything else does (although uncategorised gets run as a thread as well, by default)

### Beta
* `-rerunFailedCategories` should look at the RunStats.html file, and only run categories that failed
  * Only considers tests that ran and failed (with a positive error code), if they did not run, it will not run them
  * Probably does not work with fixtures, (i.e. do not use along with `-uncategorizedInParallel`)
* `-namespace` will allow you to specify a root namespace to look under
  * It uses `.StartsWith(namespace)` so it will find test fixtures in all the types under that
  * If they have categories, and if those categories exist in other namespaces, **all the fixtures with those categories will be run**
* `-includeIgnored` include ignored features
* `-uncategorizedInParallel` runs any uncategorised tests in parallel with each other.  Without this, a category `all` runs for them parallel to other categories.  Each feature gets its own ?runner? when this is enabled.
* `-fancyOutput` output much more advanced output file (RunStats.html) using Razor template (requires dependencies therein)(i.e. it works on my machine)
  
### Craziness:
You can do: `-categories@=IAMAFILEPATH.txt`  to run only specific categories   
(This does not support feature names)

### TODO:
* ~~Bug when using `-uncategorizedInParallel` running some that have category names...~~
* Make the general design of the runners use IOC, instead of hacked together as it is now
  * Use `RunnerSettings` for more stuff
  * Find an alternative to the `ConfigureRun` method?
* Support fixtures being in multiple categories?
* Hit ENTER or some other key, and dump all the current status data (namely, what is running, what is queued)

## Future, more versatile
* Option to output a script that will run similar to this, or describe the run
  * Either an actual batch file, or just a custom script langauge that can be modified externally then loaded in to concord
  - This would allow complete customization of the test runs
