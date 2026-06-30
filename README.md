# Shadow of RimWorld Health is a mod that attempts to implement the health system from the game RimWorld into Rain World

Here are the current Damage values for different weapons and creatures, most of these are temporary (I took what I could from RimWorld but it doesn't have many simillarities to Rain World):

## Weapons:

Boomerang: 1 (temp damage but it needs to be low)  
Dart Maggot: 0.5 (it is hard to determine damage for such a insignificant attack)  
JellyFIsh: 0.5 (higher then the rock and boomerang but it might be better to make it the other way)  
Pomegranate: 25 (damage is so high because it can only damage a creature if it falls on the creature and this object is quite heavy, might make it lower later)  
Rock: 1 (temp)  
Bomb: 55 (it will be 55 only if it explodes close enough to the creature, this damage can be split up to 4 limbs at once)  
Spear: 8.3 (the original RimWorld weapon the Pila had 25 damage but I divided it by 3 due to the fact that the Pila has a long cooldown that the spear does not have. this damage get's multiplied by the "spearDamageBonus" which goes up to 3 for the non-exhausted Gourmand)  
Fire spear: Same damage as Spear but an additional 5 burn damage (5 is a temp number, the damage does not currently get dealt to the same limb)  
Explosive spear: Same damage as Spear also causes an explosion that deals around 55 damage  
Electric spear: Same damage as Spear but an additional 5 electric burn damage (5 is a temp number, the damage does not currently get dealt to the same limb)  
JokeRifle bullet: Rock: 5, Light: 1, Ash: 0.5, Void: 18, Fruit: 2  
Lilypuck: 0.8 (less then a rock)  
SingularityBomb: 550  
Fire egg - around 55  

## Creatures:

BigNeedleWorm: 25 - Needle  
BigSpider: 10 - Fangs  
DropBug: 10 - Mandibles  
FireBug: 5 - Spine Spikes  
FireBug: around 55 - Explosion  
JetFish: 6 - Head (it seems to bump creatures with it's head)  
Leech: 0.5 - Teeth  
Lizard: (max) 22 - Teeth (taken from RimWorld's Crocodile) (here is the calculation for it: Custom.LerpMap(lizard.lizardParams.maxMusclePower, 0, 16, 4, 22);)  
Lizard: 5 - Blizzard laser  
MirosBird: 30 - Teeth
Slugcat: 8 - Teeth  
Slugcat: 1 - Roll (the Gourmand roll does damage)  
Slugcat: 10 - Slam (this refers when Gourmand jumps on a creature from a height and damages it)  
Slugcat: around 55 - Explosion (whenever arti explodes or rocket jumps)  
SkyWhale: 10 - Head (the SkyWhale seems to deal damage upon contact, probabily not often)  
Vulture: 10 - Teeth  
Vulture: around 55 _ Laser explosion (miros vulture)  
StowawayBug: 1 - Tendril
BoxWorm: 6 - Steam  
DrillCrab: 7 - Drill  
Frog: 1 - Tendril (this happens whenever the fron attaches)  
Frog: 0.5 - Body (this happens whenever the frog fails to attach, or jumps through a creature without attaching (do not quote me on this))  
Loach: 5 - Body (it seems to do contact damage)  
Rat: 0.5 - Body (it seems to do contact stun)  
RippleSpider: 0.5 - Body (it seems to do contact damage)  

### Creature Special Mention:
Centipede (all of these damages are multipleied by 1.2 if it takes place underwater):

Baby: 1.2  
Centiwing: 5  
AquaCebti: 5  
Red: 10  
Centipede: 5  

## Misc:

LethalWater: 8 + lavaContactCount (basically a +1 everytime the creature touches LethalWater)  
LocustSwarm: 1 (this damage is very frequent)  
TerrainImpactDeath: 8-14 (scales by 2 the faster the impact is)  
TerrainImpactHard: 2-8 (scales by 2 the faster the impact is, this damage is dealt whenever the player would is stunned due to falling)  
TongueTouchedZapper: 8 (replaces the instant death from tounging a zapper, will deal damage to the tongue only)  
RainLight: 0.5 (dealt whenever the rain would slightly stun the creature)  
RainDeath: 8 (dealt whenever the rain would kill the creature)  
SmallSpider: 1 (this damage is very frequent)  
ZapCoil: 20 (dealth whenever the creature touches a ZapCoil, this would normally kill the creature)  
BigJellyfish: 20  
ARZapper: 8-12 (first number is used the first 2 time the zapper is touched, from then on the second number is used)  
SandStorm: 0.5 (dealt whenever the sandstorm would slightly stun the creature)  
SandStorm: 8 (dealt whenever the sandstorm would kill the creature)
