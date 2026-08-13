# Shadow of RimWorld Health is a mod that attempts to implement the health system from the game RimWorld into Rain World

Here are the current Damage values for different weapons and creatures, most of these are temporary (I took what I could from RimWorld but it doesn't have many simillarities to Rain World). The disposition will go like this: (Name: [Damage Type], [Damage], [Armour Penetration])

## Weapons:

Boomerang: [Blunt, 1, 0]  
Dart Maggot: [Stab, 0.5, 0]  
JellyFIsh: [Electrical Burn, 1.5, 0]   
Pomegranate: [Blunt, 25, 50] (damage is so high because it can only damage a creature if it falls on the creature and this object is quite heavy, might make it lower later)  
Rock: [Blunt, 1, 0]  
Bomb: [Bomb, 55, 10] (it will be 55 only if it explodes close enough to the creature, this damage can be split up to 4 limbs at once. Applies to all Bomb and damages)  
Spear: [Stab, 8.3, 10] (the original RimWorld weapon the Pila had 25 damage but I divided it by 3 due to the fact that the Pila has a long cooldown that the spear does not have. this damage get's multiplied by the "spearDamageBonus" which goes up to 3 for the non-exhausted Gourmand)  
Fire spear: Same damage as Spear but an additional 5 burn damage (5 is a temp number, the damage does not currently get dealt to the same limb)  
Explosive spear: Same damage as Spear also causes an explosion that deals around 55 damage  
Electric spear: Same damage as Spear but an additional 5 electric burn damage (5 is a temp number, the damage does not currently get dealt to the same limb)  
JokeRifle bullet: [Bullet, [Rock: 5, Light: 1, Ash: 0.5, Void: 18, Fruit: 2], 27]  
Lilypuck: [Blunt, 0.8, 0] (less then a rock)  
SingularityBomb: [Super Bomb, 550, 10]  
Fire egg - [Bomb, 55, 10]  

## Creatures:

BigNeedleWorm: [Stab, 25, 20] - Needle  
BigSpider: [Bite, 10, 10] - Fangs  
DropBug: [Bite, 10, 10] - Mandibles  
FireBug: [Stab, 5, 10] - Spine Spikes  
FireBug: [Bomb, 55, 10] - Explosion  
JetFish: [Blunt, 6, 0] - Head  
Leech: [Bite, 0.5, 0] - Teeth  
Lizard: [Bite, 22(max), 33] - Teeth (taken from RimWorld's Crocodile) (here is the calculation for it: Custom.LerpMap(lizard.lizardParams.maxMusclePower, 0, 16, 4, 22) This means that the maximum damage a Lizard can do is 22, and the damage ranges from 4 to 22 depending on the Lizards maxMusclePower)  
Lizard: [Frostbite, 5, 10] - Blizzard laser  
MirosBird: [Nite, 30, 50] - Teeth  
Slugcat: [Bite, 7, 10] - Teeth  (taken from RimWorld's Cat, Teeth)  
Slugcat: [Blunt, 3, 10] - Roll (the Gourmand roll does damage, taken from RimWorld's Cat, Head)  
Slugcat: [Blunt, 10, 10] - Slam (this refers when Gourmand jumps on a creature from a height and damages it)  
Slugcat: [Bomb,55, 10] - Explosion (whenever arti explodes or rocket jumps)  
SkyWhale: [lunt, 10, 10] - Head (the SkyWhale seems to deal damage upon contact, probabily not often)  
Vulture: [Bite, 10, 10] - Teeth  
Vulture: [Bomb, 55, 10] - Laser explosion (miros vulture)  
StowawayBug: [Stab, 1, 10] - Tendril
BoxWorm: [Burn, 6, 10] - Steam  
DrillCrab: [Cut, 7, 10] - Drill  
Frog: [Stab, 1, 10] - Tendril (this happens whenever the fron attaches)  
Frog: [Blunt, 0.5, 0] - Head (this happens whenever the frog fails to attach, or jumps through a creature without attaching (do not quote me on this))  
Loach: [Blunt, 5, 10] - Head (it seems to do contact damage)  
Rat: [Blunt, 2, 3] - Head (it seems to do contact stun, raken from RimWorld's Rat, Head)  
RippleSpider: [Blunt, 1, 0] - Head (it seems to do contact damage)  

### Creature Special Mention:
Centipede (all of these damages are multipleied by 1.2 if it takes place underwater):

Baby: [Electrical Burn, 1.2, 0]  
Centiwing: [Electrical Burn, 5, 0]  
AquaCebti: [Electrical Burn, 5, 0]  
Red: [Electrical Burn, 10, 0]  
Centipede: [Electrical Burn, 5, 0]  

## Misc:

LethalWater: [Acid Burn (or Burn if Lava), 8, 999] + lavaContactCount (basically a +1 everytime the creature touches LethalWater)  
LocustSwarm: [Bite, 1, 0] (this damage is very frequent)  
TerrainImpactHard: [Blunt, 2-8, 0] (scales by 2 the faster the impact is, this damage is dealt whenever the player would is stunned due to falling)  
TerrainImpactDeath: [Blunt, 8-14, 0] (scales by 2 the faster the impact is)  
TongueTouchedZapper: [Electrical Burn, 8, 999] (replaces the instant death from tounging a zapper, will deal damage to the tongue only)  
RainLight: [Blunt, 0.5, 999] (dealt whenever the rain would slightly stun the creature)  
RainDeath: [Blunt, 8, 999] (dealt whenever the rain would kill the creature)  
SmallSpider: [Bite, 1, 10] (this damage is very frequent)  
ZapCoil: [Electrical Burn, 20, 999] (dealth whenever the creature touches a ZapCoil, this would normally kill the creature)  
BigJellyfish: [Electrical Burn, 20, 999]  
ARZapper: [Electrical Burn, 8-12, 999] (first number is used the first 2 time the zapper is touched, from then on the second number is used)  
SandStorm: [Blunt, 0.5, 999] (dealt whenever the sandstorm would slightly stun the creature)  
SandStorm: [Blunt, 8, 999] (dealt whenever the sandstorm would kill the creature)
