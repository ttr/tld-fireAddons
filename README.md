# FireAddons

This is mod for TLD 1.83+ 
It allows to use Lantern as fire starter and at level 3 firestarting tinder can be used as fuel which will add buffs there.

## Settings

### Lantern
Thre are two concepts in mind to use Lantern as firestarter:
accelerated - You ignite lantern and use it as fire source
mechanical - You use mechanical igniter to generate spark that you use to start fire - this concept is similar to using flints

Both are possible in real life - cover of lantern is user removable (for refuel, wick replace, cleanig), so it should not even count towards degredation. 
However, since there was no easy way to consume fuel while doing this, idea is that as balance You should set degredation to some positive value.

Using Lantern will take most time (more than flint) as you will deassembly it (in theory).

### Tinder.
TInder as fuel - when reach of level 3 of firestarting, tinder is not needed for starting fires, therfore tinder will be used as fuel and it will attribiute to starting fire rate.
Because it goes in place of fuel, Tinder base start offset is for compensate this.

## Notes / Issues.
All those values are added on pickup/discovery of item - if used in current saves, all items will not have new values. Same goes when reaching lvl 3 - only new tinder will be used as fuel.
On Scene change (so also game load) all inventory tinder is being reset, so they will not act as fuel. To mitigate this, drop all stack on a floor and pick it up (works most of time).
This is bit annoying but not planing to fix it right now.

