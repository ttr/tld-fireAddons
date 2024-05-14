# FireAddons

This is mod for TLD 2.27+ 
It allows to use Lantern as fire starter and at level 3 firestarting tinder can be used as fuel which will add buffs there.
Also adds smoldering fuel (extended embers), flint and steel firestarter and ability to use charcoal as fuel.

## Settings and concepts behind ideas.

### Lantern

Thre are two concepts in mind to use Lantern as firestarter:
accelerated - You ignite lantern and use it as fire source
mechanical - You use mechanical igniter to generate spark that you use to start fire - this concept is similar to using flints

Both are possible in real life - cover of lantern is user removable (for refuel, wick replace, cleanig), so it should not even count towards degredation. 
However, since there was no easy way to consume fuel while doing this, idea is that as balance You should set degredation to some positive value.

Using Lantern will take most time (more than flint) as you will neet to take it cover off (in theory).

### Lenses

This is used to modify fire starting parameters for Magnifing glass and binoculars (if installed).

### Tinder.

Tinder can be used as fuel, to burn for small amount of time and temperature.
Idea is that using tinder as fuel does not give long time to burn (in real life, newspaper can burn in few seconds) but quickly emit good portion of heat.
If we look at stick, that gives 7.5 minutes of time and 1 deg temperature increase. One stick is two tinder plugs. If we do follow this idea, that tinder is more efficent in releasing energy, It will be unrealistic that two tinder plugs would give 10 mins of burn time and 10deg increase (you can still do so).

Additionally, each tinder have separated setting is to have different values for firestarting for tinder (aka TinderMatters).

### Flint and steel

Flint is in game but hidden (and no icon) and normally is not crafteble nor lootable.
However, Whetstone (sharpening stone) can be used as flint in real life.
The steel part - this needs to be high carbon steel, and not aloys. In theory smelting prybar (which is hard steel) with coal will carbonize it.
That (over simplistic) theory can actually give us flint and steel as firestarter.

It's degredation should be keept low-ish - around 0.5-2 - as prybar and whetstone are rare and finite

Also, if game had shovel as lootable, that could be more realistic "steel" coponent.

Thanks to [zeobviouslyfakeacc](https://github.com/zeobviouslyfakeacc/), and his [TinCanImprovemnts](https://github.com/zeobviouslyfakeacc/TinCanImprovements) mod for ability to create blueprints.

### Charcoal

Ability touse charoal as fuel. If any other mod provide this functionlaity it might cause conflicts, so pleae disable it in settings.
Also, core game mechanics do generate one charcoal per hour of burn fire - this could cause infinitive fuel source. So this mod, every time when charcoal is added to fire, will reduce amount of generated charcoal.

### Embers / Smoldering fuel

Note - This was called embers but more accurate name should be "smoldering of fuel". Whole concept is that low temperature burn, would not consume all fuel and in the end, it would be left smoldering. However I've called it wrongly as "embers" (mostly because I've builed on existing vanilla embers), so most of this documentation and in mod it's called this way.
In short - if you enable new embers mechanics, think of it as smoldering and all in-game references to embers are actually smoldering fuel.

---

Idea is that wood (hard, soft and reclamed) and coal, can create embers if fire temperature is not too high. High temperature will burn all fuel leaving nothing to become embers.
Normally in game, temperature can only increase, so I've added ability to use snow to reduce heat.

Details:
Each fuel have burn time, that time on adding fuel is divided based on "Fuel to embers ratio" setting. That result is substracted from burn time and moved to embers time, with multiplication of "Fuel to embers exchange". This is done up to "Embers max time" - if that time is reached, all fuel time goes to burn time (as in vanilla game).
When temperature of fire (heat increase) is above "Burnout temperature", ember time is being reduced and added back to burn time (convering back using "Fuel to embers exchange"). Rate of this transfer depends on temperature - higher temperature, quicker transfer.
"Burnout ratio" sets how much of "Embers max time" would be burned in 1 hour if fire temperature is 80C or above. This calculated ratio feeds into previous.
"Water fire cooldown" means how many deg (C) will be reduced per each coldown usage (throwinf snow on fire).

Embers timer is showed in hover card, next to burn time.
Thanks to [Digitalzombie](https://github.com/DigitalzombieTLD) for help with hover card.

## Notes / Issues.
Some code was based off [Deus13](https://github.com/Deus13/) [Fire_RV mod](https://github.com/Deus13/Fire_RV)