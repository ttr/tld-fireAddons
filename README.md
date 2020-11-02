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

Using Lantern will take most time (more than flint) as you will neet to take it cover off (in theory).

### Tinder.
Tinder as fuel - when reach of level 3 of firestarting, tinder is not needed for starting fires, therfore tinder will be used as fuel and it will attribiute to starting fire rate.
Because it goes in place of fuel, Tinder base start offset is for compensate this.
Regardless of level, Tinder can be added to burning fire.

### Flint and steel
Flint is in game but hidden (and no icon) and normally is not crafteble nor lootable.
However, Whetstone (sharpening stone) can be used as flint in real life.
The steel part - this needs to be high carbon steel, and not aloys. In theory smelting prybar (which is hard steel) with coal will carbonize it.
That (over simplistic) theory can actually give us flint and steel as firestarter.

It's degredation should be keept low-ish - around 0.5-2 - as prybar and whetstone are rare and finite

Also, if game had shovel as lootable, that could be more realistic flint.

Thanks to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc/), and his [TinCanImprovemnts](https://github.com/zeobviouslyfakeacc/TinCanImprovements) mod for ability to create blueprints.

## Notes / Issues.
Some code was based off [Deus13](https://github.com/Deus13/) [Fire_RV mod](https://github.com/Deus13/Fire_RV)

Embers idea:
 * coal, hardwood and perhaps softwood will contribiute to ember state
 * their values for time burned will be halved but will add (more than reduced value, maybe even double) to ember state 
 * this will not apply when temp is 80+C
 * this possibly should not apply fully when temp is high (50+C)
 * ember state needs to be caped at 12h tops, 8-10h optimum, open fires (cmaprife, barel) will have this at 1/3-1/2 of this value
 * if temp is high, reduce time from ember state to zero, and add time to burn fuel (reverse this)
 Pitfalls:
 * remining eber state on re-lit, should be re-added as fuel (partially) - we will ingnore this and accept it's lost
 * only last X of added fuel should be aded on calculation - this will require tracking of fuel individually, too much effort for win and keeping cap and need of low-mid temp will balance it.
 Implemenation proposal:
 Selected fuels, when added their burn time will be reduced by ratio of X, and that reduced value will be added to ember time by X*Y (Y is conversion ratio) up to max of Zh (and W*Zh for campfire).
 If burning temperature is above A, embertime is reduced by ratio B (b is calculated based on temp in range of Z<->80C, at 80C, full burn out (in 1h?)), that reduction is converted as (1/Y) and added to burning time.