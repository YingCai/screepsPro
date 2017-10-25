/*
 * Module code goes here. Use 'module.exports' to export things:
 * module.exports.thing = 'a thing';
 *
 * You can import it from another modules like this:
 * var mod = require('kurultai.CityPlanner');
 * mod.thing == 'a thing'; // true
 */

const CHECKERBOARD_GENERATOR_STATE_OUT = 0;
const CHECKERBOARD_GENERATOR_STATE_LEFT = 1;
const CHECKERBOARD_GENERATOR_STATE_DOWN = 2;
const CHECKERBOARD_GENERATOR_STATE_RIGHT = 3;
const CHECKERBOARD_GENERATOR_STATE_UP = 4;
module.exports = 
{
    run: function()
    {
        for(var roomName in Game.rooms)
        {
            var room = Game.rooms[roomName];
            var maxExtensions = 0;
            var builtExtensionsCount = room.find(FIND_MY_STRUCTURES,
                                                 {filter: {structureType: STRUCTURE_EXTENSION}}).length;
            var wipExtensionsCount = room.find(FIND_MY_CONSTRUCTION_SITES,
                                               {filter: {structureType: STRUCTURE_EXTENSION}}).length;
            
            
            //Extension planning
            var extensionSitesChecked = 0;
            var spawn = room.find(FIND_MY_STRUCTURES,
                                  {filter: {structureType: STRUCTURE_SPAWN}});
            var extensionSiteToCheck = new RoomPosition(spawn[0].pos.x + 1, spawn[0].pos.y - 1, roomName);
            var extensionSiteGeneratorState = CHECKERBOARD_GENERATOR_STATE_LEFT;
            var extensionSiteGeneratorDiagLen = 1;
            var extensionSiteGeneratorSideProgress = 0;
            
            switch(room.controller.level)
            {
                case 0:
                    break;
                    
                case 1:
                    break;
                    
                case 2:
                    maxExtensions = 5;
                    break;
                    
                case 3:
                    maxExtensions = 10;
                    break;
                    
                case 4:
                    maxExtensions = 20;
                    break;
                    
                case 5:
                    maxExtensions = 30;
                    break;
                    
                case 6:
                    maxExtensions = 40;
                    break;
                    
                case 7:
                    maxExtensions = 50;
                    break;
                    
                case 8:
                    maxExtensions = 60;
                    break;
            }
            
            var watchDog = 0;
            //Plop down extensions in a checkerboard spiraling outwards
            while(((builtExtensionsCount + wipExtensionsCount) < maxExtensions) && (watchDog < 1000))
            {
                watchDog++;
                if(room.createConstructionSite(extensionSiteToCheck, STRUCTURE_EXTENSION) == OK)
                {
                    wipExtensionsCount++;
                }
                extensionSiteGeneratorSideProgress++;
                switch(extensionSiteGeneratorState)
                {
                    case CHECKERBOARD_GENERATOR_STATE_LEFT:
                        extensionSiteToCheck.x -= 2;
                        if(extensionSiteGeneratorSideProgress >= extensionSiteGeneratorDiagLen)
                        {
                            extensionSiteGeneratorSideProgress = 0;
                            extensionSiteGeneratorState = CHECKERBOARD_GENERATOR_STATE_DOWN;
                        }
                        break;
                        
                    case CHECKERBOARD_GENERATOR_STATE_DOWN:
                        extensionSiteToCheck.y += 2;
                        if(extensionSiteGeneratorSideProgress >= extensionSiteGeneratorDiagLen)
                        {
                            extensionSiteGeneratorSideProgress = 0;
                            extensionSiteGeneratorState = CHECKERBOARD_GENERATOR_STATE_RIGHT;
                        }
                        break;
                    
                    case CHECKERBOARD_GENERATOR_STATE_RIGHT:
                        extensionSiteToCheck.x += 2;
                        if(extensionSiteGeneratorSideProgress >= extensionSiteGeneratorDiagLen)
                        {
                            extensionSiteGeneratorSideProgress = 0;
                            extensionSiteGeneratorState = CHECKERBOARD_GENERATOR_STATE_UP;
                        }
                        break;
                        
                    case CHECKERBOARD_GENERATOR_STATE_UP:
                        extensionSiteToCheck.y -= 2;
                        if(extensionSiteGeneratorSideProgress >= extensionSiteGeneratorDiagLen)
                        {
                            extensionSiteToCheck.x += 1;
                            extensionSiteToCheck.y -= 1;
                            extensionSiteGeneratorSideProgress = 0;
                            extensionSiteGeneratorDiagLen += 1;
                            extensionSiteGeneratorState = CHECKERBOARD_GENERATOR_STATE_LEFT;
                        }
                        break;
                        
                    default:
                        //wtf
                        break;
                }
            }
        }
    }
};