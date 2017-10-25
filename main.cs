module.exports.loop = function ()
{
    var recruiter = require('kurultai.Recruiter');
    var workerMaster = require('kurultai.WorkerMaster');
    var registrar = require('kurultai.Registrar');
    var cityPlanner = require('kurultai.CityPlanner');
    var secretaryOfWar = require('kurultai.SecretaryOfWar');
    
    recruiter.run();
    workerMaster.run();
    registrar.run();
    cityPlanner.run();
    secretaryOfWar.run();
    
}