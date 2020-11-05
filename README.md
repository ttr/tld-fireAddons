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

### Embers
New embers mechanic change it to make it more managable.
Idea is that wood (hard, soft and reclamed) and coal, can create embers if fire temperature is not too high. High temperature will burn fuel leaving nothing to become embers.
NOrmally in game, temperature can only increase, so I've added ability to use potted (boiled) watter to reduce heat. You will need to have can/pot in inventory and they will show in 'add fuel' panel. They will not be consumed, but 250ml of water will.

Details:
Each fuel have burn time, that time on adding fuel is divided based on "Fuel to embers ratio" setting. That result is substracted from burn time and moved to embers time, with multiplication of "Fuel to embers exchange". This is done up to "Embers max time" - if that time is reached, all fuel time goes to burn time (as in vanilla game).
When temperature of fire (heat increase) is above "Burnout temperature", ember time is being reduced and added back to burn time (convering back using "Fuel to embers exchange"). Rate of this transfer depends on temperature - higher temperature, quicker transfer.
"Burnout ratio" sets how much of "Embers max time" would be burned in 1 hour if fire temperature is 80C or above. This calculated ratio feeds into previous.
"Water fire cooldown" means how many deg (C) will be reduced per each 250ml water added to fire.

## Notes / Issues.
Some code was based off [Deus13](https://github.com/Deus13/) [Fire_RV mod](https://github.com/Deus13/Fire_RV)

Embers - using can/pot - after 1st use, GUI will show NONE instead of object name - this is due to fact can/pot are consumed and recreated.
With out consuming it (as in, changing it fuel state so it will not be consumed) that chnage state was corrupting gameitem and after few minutes of "feed Fire" window being open, that item was causing lag spike when interacted with.

Embers - todo:
 * open fires (cmaprife, barel) will have this at 1/3-1/2 of maximum ember state (faster burnout, less capacity) - there will be no check if it's indoors (probably)
 * fire starting skill - low skill => chance to extinguish fire, high skill, addon of max time to ember state ?
 * restaring fire - check if embers time is reduced correctly based on time "in ember state"
