### This directory contains a bunch of WIP notes.

<br/>

## Puzzles

* [Factory Refinery](puzzles/FactoryRefinery.md) - redesign of the Factory puzzle
* [Hackdev Unlock](puzzles/HackdevUnlock.md) - redesign how hackdevs are unlocked

<br/>

## Bug-Fixes

* [Factory Machine gets stuck](fixes/FactoryRefineryBug.md)

<br/>

## Other Notes

Puzzles that could be revised:
* hacking the finance server
* bypassing casino security
* making a door cracker
* hacking the ministry elevators
* escaping prison (perhaps add a story event for the trial)

APIs:
* teleporter is too powerful (remove `SetWorldPosition`)
* tingrunner api seems to powerful
* door api should require door key for unlock/lock
* tingrunner `getThingsOfType` is missing some types (rewrite to use reflection instead of fixed list?)
* elevators should receive a `GetFloorCount` function
* `Help(functionName)` function that returns string "function: 'fnc' | documentation: 'docs' | parameters: ['p1' 'p2'] | api: 'memory'" (if api is provided, screwdriver should say "not allowed" rather than "does not exist")
* `OnConnect` handler on computers that can reject connection requests (?)
* `GetNativeFunctions` and `GetDefinedFunctions` function that return an array of function names
* Documentation / hackdev-help should specify the return type

Story:
* The "find pixie / the lodge" quest could be more elegantly written. Figuring out "what's up with that fishy shoe store" seems much more natural (this on its own would probably be a massive project since half of sebastians dialogues would need to be rewritten)

Misc:
* harddrives are currently useless (?)
* add the possibility for Extractors to gain the internet API (?)