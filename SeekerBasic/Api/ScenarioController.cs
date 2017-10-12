using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeekerBasic.Model;
using Lendsum.Infrastructure.Core.Persistence.SqlServerPersistence;
using SeekerBasic.Domain;
using SeekerBasic.Domain.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SeekerBasic.Api
{
    [Route("api/[controller]")]
    public class ScenarioController: Controller
    {
        private readonly IScenarioService service;

        public ScenarioController(IScenarioService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Gets the scenario.
        /// </summary>
        /// <param name="id">The identifier of the scenario</param>
        /// <returns></returns>
        [HttpGet]
        public ScenarioAggregate GetScenario(string id)
        {
            return this.service.GetSceneario(int.Parse(id));
        }

        /// <summary>
        /// Move the player in the scenario with id passed.
        /// </summary>
        /// <param name="id">The identifier of the scenario</param>
        /// <param name="direction">The direction.</param>
        [HttpPost]
        public void Post(string id, DirectionEnum direction)
        {
            this.service.Move(int.Parse(id), direction);
        }

        /// <summary>
        /// Reset the scenario
        /// </summary>
        /// <param name="id">The identifier of the scenario</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            this.service.Reset(id);
        }
    }
}
