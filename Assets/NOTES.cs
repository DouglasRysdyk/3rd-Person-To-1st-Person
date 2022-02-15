/*NOTES*/

/* TODO: 
 * - FPS Controller with Unity's New Input System: https://www.youtube.com/watch?v=tXDgSGOEatk -- WATCH, may help me move a character 
 * - Let's Make a First Person Game in Unity! #1 FPS Movement: https://www.youtube.com/watch?v=rJqP5EesxLk -- BACK UP, may help me move a character 
 * - How to Create Player Movement in UNITY with Unity's new Input System: https://www.youtube.com/watch?v=UvfoPTEX4NY -- BACK UP, may help me move a character 
 * - Add the gun and FPS mechanics.  
 *   - https://www.youtube.com/watch?v=_QajrabyTJc 
 *   - https://www.youtube.com/watch?v=THnivyG0Mvo 
 *   - https://www.youtube.com/watch?v=_AwqdnTtaD0&list=PLW3-6V9UKVh2T0wIqYWC1qvuCk2LNSG5c&index=3 
 *   - Using the new input system -- https://www.youtube.com/watch?v=uPlXdMKM5pc&list=PLW3-6V9UKVh2T0wIqYWC1qvuCk2LNSG5c&index=2 
 * - Add the Road map.   
 * - Do a lot of history research.  
 * - Outline the story and specify branches.  
 *   - Simplify.  
 */

/* Errors:
 * 
 */

/* Design Doc: 
 * - It seems like creating a terrain isn't so bad.  And there may be easy ways to do textures and stuff.  
 * - I don't know if it'd be easier to do STALKER/Fallout style open world or Skyrim style.  
 *   - I think I'll do a STALKER/Fallout style.  This means smaller and maybe more levels.  
 * - The player's goal is to integrate themselves into this society; to force themselves to become part of history.  
 * - The game will take place somewhere in medieval Europe.  I haven't really decided where yet.  
 * - I'm not trying to add a bunch of quests into the game.  The story is the draw, the quests are just what you do to get through it.  
 *   - To this end there only need to be as many quests as necessary.  
 *     - I need something flexible, but want something scalable.  
 */

/* Components: 
 * - The gun -- I should do this first.  If I can't figure out how to make an FPS game I am a failure.  
 * - Open world levels -- What levels do I definitely need?  
 *   - Road -- Should be an easy test level.  If I can't dfo this then I dunno how I'll do anything else. 
 *   - Forest  
 *   - Village 
 * - Quests -- There are a ton of systems that handle quests.  I need to research how other games implement quests.  
 *   - There are probably a lot of ways of doing these.  I should look up some best practices.  If not I can probbly come up with some ideas.  

 * Stretch: 
 * - Day night cycle  
 * - What else?  
 *   - Again the goal is to get a better feeling of what it was like to live in this time.  What else do we take for granted?  
 *     - Food kind of.  
 *     - Strength.  
 *     - Light.  
 *     - Temperature.  
 *     - Wealth.  
 * - More guns 
 */

/* History Questions:
 * - How were taxes collected?  
 * - How were villages, and their associated lords positioned physically in the world?  
 *   - How far away would a castle be?  
 *   - Where would other towns be?  
 * - Can bullet pierce armor?  
 * - Who owned armor?  
 * - Who owned weapons?  
 * - When were armorments employed?  

 * Fun Resources:
 * - Spice & Wolf 
 * - The Last Duel 
 * - HBO's Rome? 
 * - Barbarians? -- https://www.youtube.com/watch?v=ojC-zTXSAsY 

 * Youtubers: 
 * - https://www.youtube.com/channel/UCIjGKyrdT4Gja0VLO40RlOw/videos 
 */

/* Notes:
 * - Used this for making the FPS character controller -- https://www.youtube.com/watch?v=_QajrabyTJc 
 * - Switch action maps -- https://www.youtube.com/watch?v=Yjee_e4fICc&t=2040s 
 * - You can use the Player Input Component to make an action map premade by Unity.  That would've been nice to know -- https://youtu.be/Yjee_e4fICc?t=1939 
 * - Rebinding keys.  Still confusing and seems complicated -- https://www.youtube.com/watch?v=Yjee_e4fICc&t=2290s 
 *   
 * FirstPersonController Notes:
 * It makes sense to me to make the controls Vector2 since we're not really controling the character in a three dimensional way. 
 * The character is basically a highly mobile tank.  
 * WASD/Arrows move the player left, right, forward, and back.  
 * Mouse look will determine their orientation.  Obviously the player can't just look up and hit W to move into the sky.  
 * Jump is the only Z axis movement.  
 

* //This is what a guard statement basically looks like.  Learn to use it more.  
* //if (!trueToPass)
* //    return; 

 * Sources:
 * - Using the Generate C# Class Option -- https://www.youtube.com/watch?v=a2vLaKGCYsA 
 * - How to use the new Input System -- https://www.youtube.com/watch?v=m5WsmlEOFiA TAKE NOTES
 * - There is a section going over how to handle pressing and holding -- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html 
 * - Review the generated code later.  
 * - Search this -- https://www.youtube.com/results?search_query=press+and+hold+unity+input+system 
 * - Reviews holding a button down -- https://youtu.be/Yjee_e4fICc?t=813 

 * Notes: 
 * - Value = If the player has multiple controls working at the same time then the input system will select a main one.(?)
 *   - You'll usually use value since the player will normally only be using one controller at a time.  
 *   - Maybe this would falter in a split screen multiplayer situation.  
 * - Button = Very similar to Value, except called less often -- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Actions.html#action-types
 * - Pass Through = If the player has multiple controls working at the same time the input system will simply receive any inputs.(?)
 * - Control Type assigns the data type (Vector2, Quaternion, "Eyes"(?), more... I don't really get all the options).  
 * - Callbacks -- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Interactions.html#default-interaction 
 * - Vector2 Composite -- A group of different bindings (in this case a Vector2 - good for WASD movement).  
 * - A modifier is like a combo.  If I understand correctly you use this with floats.  
 * - The input system lets you organize control schemes.  So if you are making a game for mobile and PC you can make a Mobile control scheme and Desktop control scheme.
 * - When generating code the namespace defaults to the Global Namespace.  
 * - Processors let you do things like Clamp the values.  This is the only example I understand off the bat after looking at it.  
 * - A button is a float.  It starts at 0 and goes to 1.  I guess this is to measure partial presses.  
 * - She said this initial method (the one I'm using; the one that WORKS) is not recommended by Unity.  But the second method did not work for me at all.
 *   - Why is this method not recommended?  
 *   - How do I get her method working?  
 *   - Which one is actually better? 
 *     - Truthfully the answer is the one that works first and foremost.  
 *     
 * - Generating the script means the generated script cannot easily be changed.  This makes rebinding keys difficult (but not impossible).
 *   - Rebinding for who?  The player or the dev?  
 *   - The player?  How does the player rebind controls?  
 *     - Unity has a component called Player Input.  
 *   - Put your PlayerInputs there.  
 *   - Select the default map.  
 *   - UI Input Module and Camera are really used for local multiplayer.  This works with the Player Input Manager component.  
 *   - What does "Behavior" mean?  
 *   - Under Behavior you can select Invoke Unity Events.  
 *     - She does not recommend doing this so okay.  I guess it's because it doesn't differentiate between different contexts?  
 * - Naming Convetions: 
 *   - Player Controls input action asset -- Goes on Player Input component.
 *   - Player Input component -- Goes on player character.
 *   - CharacterController script -- Goes on player character.
 *     - Access the Player Input component from here.  
 *     - Call the actions you want via their names (Strings) in the input action asset.  
 * - Read, understand -- https://stackoverflow.com/questions/155609/whats-the-difference-between-a-method-and-a-function 
 */




