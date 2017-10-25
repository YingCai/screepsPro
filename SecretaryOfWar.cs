/*
 * Module code goes here. Use 'module.exports' to export things:
 * module.exports.thing = 'a thing';
 *
 * You can import it from another modules like this:
 * var mod = require('kurultai.SecretaryOfWar');
 * mod.thing == 'a thing'; // true
 */

module.exports = {
    run: function()
    {
        var spawn = Game.spawns['PIKAKORUM1'];
        var fighters = _.filter(Game.creeps, function(creep){return (creep.memory.role == 'meleeFighter')});
        var enemies = spawn.room.find(FIND_HOSTILE_CREEPS);
    
        for(var fighterIndex in fighters)
        {
            if(enemies.length > 0)
            {
                if(fighters[fighterIndex].attack(enemies[0]) == ERR_NOT_IN_RANGE)
                {
                    fighters[fighterIndex].moveTo(enemies[0]);
                }
            }
            else
            {
                if((Math.abs(fighters[fighterIndex].pos.x - spawn.room.controller.pos.x) +
                    Math.abs(fighters[fighterIndex].pos.y - spawn.room.controller.pos.y)) > 5)
                {
                    fighters[fighterIndex].moveTo(spawn.room.controller);
                }
            }
        }

    }
};