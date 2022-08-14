# Intentions

Unlocking Hackdevs is currently too easy / not very interesting.  
Stoves currently serve no purpose beyond frying your code.  

Solution: Hackdevs are now SecurityLevel 1 and can be placed on stoves.

This requires that `Character.Hack` and `ComputerInteractionState.Hack` are adjusted such that the `Allow` function is optional and still allows level 0 hacking.  
Otherwise the player could hard-lock his save file by bricking all of the hackdevs.