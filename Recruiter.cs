/*
 * Module code goes here. Use 'module.exports' to export things:
 * module.exports.thing = 'a thing';
 *
 * You can import it from another modules like this:
 * var mod = require('kurultai.Recruiter');
 * mod.thing == 'a thing'; // true
 */
var _ = require('lodash');
const MAX_NUMBER_OF_WORKERS = 10;

module.exports = 
{
    run: function ()
    {
        var spawn = Game.spawns['PIKAKORUM1'];
        var workers = _.filter(Game.creeps, function(creep){return (creep.memory.role == 'worker')});
        var workerLevel = Math.floor(spawn.room.energyCapacityAvailable/250);
        var emergencyWorkerKit = [WORK, CARRY, CARRY, MOVE, MOVE];
        var workerKit = new Array(3*workerLevel);
        var sources = spawn.room.sources;

        var fighters = _.filter(Game.creeps, function(creep){return (creep.memory.role == 'meleeFighter')});
        var fighterLimit = 5;
        var fighterLevel = Math.floor(spawn.room.energyCapacityAvailable/130);
        var emergencyFighterKit = [ATTACK, ATTACK, MOVE, MOVE];
        var fighterKit = new Array(2*workerLevel);

        //Adjust workforce size on new year's day
        if(Memory.date == 0)
        {
            var needMoreWorkers = false;
            if(Memory.workerLimit == undefined)
            {
                Memory.workerLimit = MAX_NUMBER_OF_WORKERS/2;
            }
            for(var sourceIndex in Memory.sources)
            {
                if(Memory.sources[sourceIndex].energyAtReset > 0)
                {
                    needMoreWorkers = true;
                    break;
                }
            }
            
            if(Math.abs(Memory.workerLimit - workers.length) <= 1)
            {
                if(needMoreWorkers == true)
                {
                    if(Memory.workerLimit < (Memory.sources.length * 12))
                    {
                        Memory.workerLimit += 1;
                        console.log('Increasing workforce to ' + Memory.workerLimit);
                    }
                }
                else
                {
                    Memory.workerLimit -= 1;
                    console.log('Decreasing workforce to ' + Memory.workerLimit);
                }
            }
        }
        
        for(var i = 0; i < workerLevel; i++)
        {
            workerKit[i] = WORK;
            workerKit[i+workerLevel] = MOVE;
            workerKit[i+(workerLevel*2)] = CARRY;
        }
        
        for(var i = 0; i < fighterLevel; i++)
        {
            fighterKit[i] = ATTACK;
            fighterKit[i+fighterLevel] = MOVE;
        }
        
        if(workers.length < (Memory.workerLimit/2))
        {
            //emergency mode, build basic workers to bootstrap back up
            spawn.createCreep(emergencyWorkerKit, null, {role: 'worker'});
        }
        else if (workers.length < Memory.workerLimit)
        {
            spawn.createCreep(workerKit, null,{role: 'worker'});
        }
        
        if((fighters.length < fighterLimit))
        {
            if(workers.length == 1)
            {
                spawn.createCreep(emergencyFighterKit, null, {role: "meleeFighter"});
            }
            else
            {
                spawn.createCreep(fighterKit, null, {role: 'meleeFighter'});    
            }
            
        }
    }
};