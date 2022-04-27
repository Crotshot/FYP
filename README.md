# Subjugation
The unity project files for Subjugation.
### GitHub Pages https://crotshot.github.io/FYP/

## What is Subjugation?
Subjugation is a 3rd person 1 vs 1 MOBA (Multiplayer Online Battle Arena). Similarly, to other
MOBAs Subjugation’s map has two bases at opposing sides of the map and players fight to capture
the enemy’s base.

Players control a Conqueror and can use their unique strengths/abilities along with an armada of
minions they command to help dominate the map and win the game.

### Pregame
Before entering a match, players can select one of five Conquerors to play and four of twelve different minion types, this
allows for a large variety of playstyles and strategies to be used.

### The Map
Subjugation’s map is broken down into tiles and most of these map tiles are randomly generated
and capturable by players. There are a variety of different capturable tiles and each offer some form
of bonus resource or strategic value. The map is generated on one side and then mirrored through
its centre so that both players have the area for fairness.

### Control Points
There are five controls points on the map, 1 in the centre and 2 on each players side of the map,
players need to capture the centre control point and at least 1 control point on either side of the
map in order to be able to capture the enemy base. To help players in there mission the game
creates ‘Base Minions’ that go to control points and fight to control them.

### Gameplay
The game uses WASD controls for movement, the player characters attempt to attack towards the players mouse.
Each conqueror has a passive ability and three active abilities, each conqueror has a unique kit and plays differently
to the others.
Players purchase minions from spawners in their base to follow them, minions follow a variety of commands from the player to
enable different forms of strategic gameplay. Minons despite being combat units they are also a resource that can be sacrificed
in structures around the map to gain a resource or strategic advantage over your oppponent

### Controls
>> W/S  == Forward/ Backward
>>A/D == Rotate Clockwise/Anticlockwise
>>Left Click/Hold == Attack
>>Q == Ability 1
>>2 == Ability 2
>>3 == Ability 3

>> Scroll Wheel Up/Down == Change Selected Minion Type
>> Right Click/Hold == Send current selected minion type to attack at mouse
>> F Press/Hold == Send current selected minion to defend at mouse
>> R Press/Hold == Retreat any minions not currently following back to the Conqueror (Retreating minions do not fight)
>> E Press/Hold ( When mouse over Structure )== Summon/ Send into structures
>> E Press/Hold == Recall minions back to Conqueror(Recalling minions will still fight on their way back)

Holding will continuously send minions to perform actions, except recall and retreat which when held will be performed on all minions of selected type



### Known bugs: 
>> In some instances the minion summoning buildings will not function on the client and be stationary in the centre of the map!
>> Buildings may sometimes function when not fully captured
>> Inconsistency with host and client control point capture rates
>> Possible sightings of Herobrine
>> Conqueror mouse followers do not function across network so it may look like attacks are missing but theya re infact hitting
>> Minions respawn through object pool can have the wrong healthbar colour
