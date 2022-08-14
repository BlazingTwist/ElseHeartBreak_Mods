## Issue

After Converting goods with the Factory machine, it cannot accept additional goods until the game is restarted / save loaded.

<br/>

## Fix

This is because deleted Objects are not removed from the tile they are occupying.  
* either add `ting.tile.RemoveOccupant(ting);` to `TingRunner.RemoveTing`
* or change `Machine.Update` such that deleted Tings are ignored
* or (perhaps preferably) do both

<br/>

## TODO

Create a patcher that does this.