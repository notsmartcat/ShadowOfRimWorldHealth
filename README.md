# Shadow of RimWorld Health is a mod that attempts to implement the health system from the game RimWorld into Rain World

## Damage Values

Here are the current Damage values for different weapons and creatures, most of these are temporary (I took what I could from RimWorld but it doesn't have many overlaps with Rain World). The disposition will go like this: (Name: [Damage Type], [Damage], [Armour Penetration])

## Weapons:

Boomerang: [Blunt, 6, 0]  
Dart Maggot: [Stab, 0.5, 0]  
JellyFIsh: [Electrical Burn, 1.5, 0]   
Pomegranate: [Blunt, 25, 50]  
Rock: [Blunt, 12, 5]  
Bomb: [Bomb, 55, 10] (damage 55 is a maximum and will applied only if it explodes close enough to the creature, this damage can be split up to 4 limbs at once. Applies to all Explosion damages)  
Spear: [Stab, 25, 10] (damage taked from RimWorld's Pila weapon, not certain if the 25 damage will stay as it is fairly high. This damage get's multiplied by the "spearDamageBonus" which goes up to 3 for the non-exhausted Gourmand)  
Fire spear: Same as Spear but will also set the creature on fire.  
Explosive spear: Same damage as Spear also causes an explosion that deals maximum 30.25 damage  
Electric spear: Same damage as Spear but an additional 5 electric burn damage (5 is a temp number, the damage does not currently get dealt to the same limb)  
JokeRifle bullet: [Bullet, [Rock: 5, Light: 1, Ash: 0.5, Void: 18, Fruit: 2], 27]  
Lilypuck: [Blunt, 4, 0]  
SingularityBomb/EnergyCell: [Super Bomb, 550, 10]  
Firecracker: [Bomb, 2.75, 10]  
Fire egg - [Bomb, 1375, 10] (damage is so high because I am multiplying the damage by the same amount I multiply the Bomb damage, for some reason the base Fire egg damage is just extremely high, might need to tweak the multiplier but for now I will leave it)  

## Creatures:

BigNeedleWorm: [Stab, 25, 20] - Needle  
BigSpider: [Bite, 10, 10] - Fangs  
DropBug: [Bite, 10, 10] - Mandibles  
FireBug: [Stab, 5, 10] - Spine Spikes  
FireBug: [Bomb, 6.875, 10] - Explosion  
JetFish: [Blunt, 6, 0] - Head  
Leech: [Bite, 0.5, 0] - Teeth  
Lizard: [Bite, 22(max), 33] - Teeth (taken from RimWorld's Crocodile) (here is the calculation for it: Custom.LerpMap(lizard.lizardParams.maxMusclePower, 0, 16, 4, 22) This means that the maximum damage a Lizard can do is 22, and the damage ranges from 4 to 22 depending on the Lizards maxMusclePower)  
Lizard: [Frostbite, 5, 10] - Blizzard laser  
MirosBird: [Nite, 30, 50] - Teeth  
Slugcat: [Bite, 7, 10] - Teeth  (taken from RimWorld's Cat - Teeth)  
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
Baby Centipede: [Electrical Burn, 1.2, 0] (multiplied by 1.2 if underwater)  
Centiwing/AquaCenti/Centipede: [Electrical Burn, 5, 0] (multiplied by 1.2 if underwater)    
Red Centipede: [Electrical Burn, 10, 0] (multiplied by 1.2 if underwater)    

## Misc:

LethalWater: [Acid Burn (or Burn if Lava), 8, 999] damage will get a + lavaContactCount (basically a +1 everytime the creature touches LethalWater)  
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
