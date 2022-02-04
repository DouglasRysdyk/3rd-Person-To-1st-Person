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
 * (02-04-2021) @13:35
 * CharacterController.Move called on inactive controller
 * UnityEngine.CharacterController:Move (UnityEngine.Vector3)
 * FirstPersonController:Movement () (at Assets/Scripts/FirstPersonController.cs:68)
 * FirstPersonController:Update () (at Assets/Scripts/FirstPersonController.cs:61)
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




