# What is this? 
Code contains a bunch of extension methods for built-in types and some extra useful stuff.


## Should I use it? 

It is a personal utility library, so while fairly generic, it is still tailored for specific tasks.

You probably would be better off if you built your own helper lib.


## What is inside?

### Portable file system abstraction

Cake (http://cakebuild.net) build tool has a nice file system abstraction; sadly, it does not work with PCL.

I really wanted to use it in portable class libraries without hassle, so almost all of `Cake.IO` was ripped and key classes were reworked with a small number of breaking API changes.


### Collections

* Standard generic list with events before and after all modifications with ability to cancel and separate handling of batch additions/removals.
* Two simple pools:
  * typical "take instance out of the pool, put it back after you done";
  * less standard "owning" pool, which tracks provided instances and returns them back to pool when they become unused, as defined by user callback.
* Dictionary with weak-referenced values. Null values are supported.


### Extensions

* Check whether a type contains another type in its inheritance chain, be it interface, abstract classe or generic with parameters.
* Reverse and chain comparers.
* Invoke latests delegate targets.
* Manipulate collection values inside a dictionary more easily.
* Get value from a dictionary in a convenient way.
* Create default instance of the given type.
* Clamp numbers, do float equality checks with epsilon, normalize and remap values,
 find proper modulo (not to be confused with `%` operator), lots of stuff.
* Check sequences for equivalence.
* Randomly chose one of the elements based on their probabilities.
* Åc


### Utility classes

* Simple fluent argument checking.
* Base disposable class, helps with the standard .NET disposal pattern. Does not implement a finalizer by default.
* Ranges, with intersection and union operations.
* Reference counter.
* Xorshift rng taken from Redzen code library (https://github.com/colgreen/Redzen/).
* Few interpolation functions.
* Null checker without boxing for generic variables.


### License

It's MIT. We are all standing on the shoulders of giants, so there are few pieces of code from other libraries (and a giant chunk of code from Cake), 
all under MIT or Apache license.