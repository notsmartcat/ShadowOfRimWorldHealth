# Shadow of RimWorld Health is a mod that attempts to implement the health system from the game RimWorld into Rain World

Here are the current Damage values for different weapons and creatures, most of these are temporary (I took what I could from RimWorld but it doesn't have many simillarities to Rain World):

## Weapons:

Boomerang: 1 (temp damage but it needs to be low)  
Dart Maggot: 0.5 (it is hard to determine damage for such a insignificant attack)  
JellyFIsh: 1.5 (higher then the rock and boomerang but it might be better to make it the other way)  
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

## Creatures (basically all of these are currently null due to me not being able to come up with a damage at the moment):

BigNeedleWorm: null - Needle  
BigSpider: null - Fangs  
DropBug: null - Mandibles  
FireBug: null - Spine Spikes  
FireBug: around 55 - Explosion  
JetFish: null - Head (it seems to bump creatures with it's head)  
Leech: null - Teeth  
Lizard: 22 - Teeth (taken from RimWorld's Crocodile, unsure which Lizard will take the base damage as I believe the damage should differ depending on the size of the Lizard)  
Lizard: null - Blizzard laser  
MirosBird: null - Beak (not sure if I should say Beak or Teeth as Beak might be for pecking then biting but the MirosBird does seem to have a Beak)  
Slugcat: Null - Teeth  
Slugcat: Null - Roll (the Gourmand roll does damage)  
Slugcat: Null - Slam (this refers when Gourmand jumps on a creature from a height and damages it)  
Slugcat: around 55 - Explosion (whenever arti explodes or rocket jumps)  
SkyWhale: Null - Head (the SkyWhale seems to deal damage upon contact, probabily not often)  
Vulture: Null - Beak (Just Like the MirosBird I am unsure if Beak is the right wording)  
Vulture: around 55 _ Laser explosion (miros vulture)  
StowawayBug: Null - Tendril (these wouldn't do much damage)  
BoxWorm: Null - Steam  
DrillCrab: Null - Drill  
Frog: Null - Tendril (this happens whenever the fron attaches)  
Frog: Null - Body (this happens whenever the frog fails to attach, or jumps through a creature without attaching (do not quote me on this))  
Loach: Null - Body (it seems to do contact damage)  
Rat: Null - Body (it seems to do contact stun)  
RippleSpider: Null - Body (it seems to do contact damage)  

### Creature Special Mention:
Centipede (all of these damages are multipleied by 1.2 if it takes place underwater) (these all have ranges, the range should be replaced with a constant number):

Baby: 0.8-1.2  
Centiwing: 4.8-8.2  
AquaCebti: 4.8-8.2  
Red: 8.8-12.2  
Centipede: 4.8-8.2  

## Misc:

LethalWater: 8.2-18.8 (need to get rid of the range, possibly scale the damage everytime the creature touches it)  
LocustSwarm: 0.8-1.2 (need to get rid of the range, this damage is very frequent)  
TerrainImpactDeath: 9.2-12.8 (need to get rid of the range, this damage is dealt instead of instantly killing the player so it needs to be fairly high)  
TerrainImpactHard: 1.2-4.8 (need to get rid of the range, this damage is dealt whenever the player would is stunned due to falling)  
TongueTouchedZapper: 7.2-12.8 (need to get rid of the range, replaces the instant death from tounging a zapper, will deal damage to the tongue only)  
RainLight: 0.8-3.2 (need to get rid of the range, dealt whenever the rain would slightly stun the creature)  
RainDeath: 14.2-24.8 (need to get rid of the range, dealt whenever the rain would kill the creature so needs to be high)  
SmallSpider: 0.8-1.2 (need to get rid of the range, this damage is very frequent)  
ZapCoil: 7.2-12.8 (need to get rid of the range, dealth whenever the creature touches a ZapCoil, this would normally kill the creature)  
BigJellyfish: 22.2-44.8 (need to get rid of the range)  
ARZapper: 7.2-12.8 (need to get rid of the range, dealth whenever the creature touches a ARZapper, unlike the regular zapper touching this 3 times disintigrates the creature)  
SandStorm: 0.8-3.2 (need to get rid of the range, dealt whenever the rain would slightly stun the creature)  
SandStorm: 14.2-24.8 (need to get rid of the range, dealt whenever the rain would kill the creature so needs to be high)  
