/*
 * Module code goes here. Use 'module.exports' to export things:
 * module.exports.thing = 'a thing';
 *
 * You can import it from another modules like this:
 * var mod = require('kurultai.Registrar');
 * mod.thing == 'a thing'; // true
 */

//Memory and caching module
module.exports = {
    run: function()
    {
        var sources = Game.spawns['PIKAKORUM1'].room.find(FIND_SOURCES);
        
        //forget memory from dead creeps
        for(var name in Memory.creeps)
        {
            if(!Game.creeps[name])
            {
            delete Memory.creeps[name];
            console.log('Clearing non-existing creep memory:', name);
            }
        }
        
        
        //Workforce size maintenance
        //Make sure sources array exists
        if(Memory.sources == undefined)
        {
            Memory.sources = new Array();
        }
        while(Memory.sources.length < sources.length)
        {
            Memory.sources.push({energyAtReset: 0});
        }
        
        //remember amount of energy in each source when it refreshes
        for(var sourceIndex in sources)
        {
            if(sources[sourceIndex].ticksToRegeneration == 10)
            {
                Memory.sources[sourceIndex].energyAtReset = sources[sourceIndex].energy;
                console.log('source resets with ' + sources[sourceIndex].energy + ' energy');
            }
        }
        
        //Calendar keeping
        if(Memory.date == undefined)
        {
            Memory.date = 0;
        }
        else
        {
            Memory.date = (Memory.date + 1) % 365;
        }
    }
};