/*
 * Module code goes here. Use 'module.exports' to export things:
 * module.exports.thing = 'a thing';
 *
 * You can import it from another modules like this:
 * var mod = require('kurultai.WsorkerMaster');
 * mod.thing == 'a thing'; // true
 */

const WORKER_STATE_DEFAULT = 0;
const WORKER_STATE_HARVEST_ENERGY = 1;
const WORKER_STATE_XFER_ENERGY_TO_SPAWN = 2;
const WORKER_STATE_BUILD = 3;
const WORKER_STATE_UPGRADE_CONTROLLER = 4;

module.exports = {
    run: function()
    {
        var workers = _.filter(Game.creeps, function(creep){return (creep.memory.role == 'worker')});
        
        for(var workerName in workers)
        {
            var worker = workers[workerName];
            
            switch(worker.memory.state)
            {
                default:
                case WORKER_STATE_DEFAULT:
                    //what the dingus
                    this.transition(worker, WORKER_STATE_HARVEST_ENERGY);
                    worker.say('uhh');
                    break;
                    
                case WORKER_STATE_HARVEST_ENERGY:
                    var sources = worker.room.find(FIND_SOURCES);
                    
                    //fill inventory at an energy source
                    if(worker.carry.energy < worker.carryCapacity)
                    {
                        //Gather energy if carry capacity is still available
                        switch(worker.harvest(sources[worker.memory.targetSource]))
                        {
                            case ERR_NOT_IN_RANGE:
                                worker.moveTo(sources[worker.memory.targetSource]);
                                break;
                                
                            case ERR_NOT_ENOUGH_RESOURCES:
                                //move to another source
                                this.transition(worker, WORKER_STATE_HARVEST_ENERGY);
                                break;
                                
                            default:
                                break;
                        }
                    }
                    //Full on energy, transfer to spawn
                    else
                    {
                        this.transition(worker, WORKER_STATE_XFER_ENERGY_TO_SPAWN);
                    }
                    worker.say('harvesting')
                    break;
                    
                case WORKER_STATE_XFER_ENERGY_TO_SPAWN:
                    var targets = worker.room.find(FIND_STRUCTURES, {
                            filter: (structure) => {
                                return (structure.structureType == STRUCTURE_EXTENSION || structure.structureType == STRUCTURE_SPAWN) &&
                                    structure.energy < structure.energyCapacity;
                            }
                    });
                    
                    if(worker.carry.energy == 0)
                    {
                        //worker is empty, get more energy
                        this.transition(worker, WORKER_STATE_HARVEST_ENERGY);
                    }
                    else if(targets.length > 0) {
                        //spawns to be filled, fill them
                        var result = worker.transfer(targets[0], RESOURCE_ENERGY)
                        if(result == ERR_NOT_IN_RANGE) {
                            worker.moveTo(targets[0]);
                        }
                    }
                    else if(worker.carry.energy > 0)
                    {
                        //all spawns are full, upgrade controller instead
                        this.transition(worker, WORKER_STATE_BUILD);
                    }
                    else
                    {
                        //negative energy??
                    }
                    
                    worker.say("returning");
                    break;
                    
                case WORKER_STATE_BUILD:
                    if(worker.carry.energy > 0)
                    {
                        var constructionSites = worker.room.find(FIND_CONSTRUCTION_SITES);
                        if(constructionSites.length > 0)
                        {
                            if(worker.build(constructionSites[0]) == ERR_NOT_IN_RANGE)
                            {
                                worker.moveTo(constructionSites[0]);
                            }
                        }
                        else
                        {
                            //No more construction sites, upgrade controller
                            this.transition(worker, WORKER_STATE_UPGRADE_CONTROLLER);
                        }
                    }
                    else
                    {
                        //out of energy, go get some more
                        this.transition(worker, WORKER_STATE_HARVEST_ENERGY);
                    }
                    break;
                    
                case WORKER_STATE_UPGRADE_CONTROLLER:
                    if(worker.carry.energy > 0)
                    {
                        //there is energy to upgrade
                        if(worker.upgradeController(worker.room.controller) == ERR_NOT_IN_RANGE) {
                            worker.moveTo(worker.room.controller);
                        }
                    }
                    else
                    {
                        //no more energy, go get more
                        this.transition(worker, WORKER_STATE_HARVEST_ENERGY);
                    }
                    worker.say('upgrading controller');
                    break;
            }
        }
    },
    
    transition: function(worker, toState)
    {
        var sources = worker.room.find(FIND_SOURCES);
        switch(toState)
        {
            default:
            case WORKER_STATE_DEFAULT:
                break;
                
            case WORKER_STATE_HARVEST_ENERGY:
                worker.memory.targetSource = 0;
                for(var i = 0; i < sources.length; i++)
                {
                    if(sources[i].energy > sources[worker.memory.targetSource].energy)
                    {
                        //Target the richest source
                        worker.memory.targetSource = i;
                    }
                }
                break;
                
            case WORKER_STATE_XFER_ENERGY_TO_SPAWN:
            case WORKER_STATE_UPGRADE_CONTROLLER:
                break;
        }
        
        worker.memory.state = toState;
    }
};