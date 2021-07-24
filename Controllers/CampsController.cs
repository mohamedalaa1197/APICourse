using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Pluralsight.Data;
using Pluralsight.Data.Entities;
using Pluralsight.Models;

namespace Pluralsight.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
            _mapper = mapper;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var results = await repository.GetAllCampsAsync(includeTalks);
                return _mapper.Map<CampModel[]>(results);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {


            try
            {
                var result = await repository.GetCampAsync(moniker);

                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

        }

        [HttpGet("{moniker}")]
        [MapToApiVersion("1.1")]
        public async Task<ActionResult<CampModel>> Get11(string moniker)
        {


            try
            {
                var result = await repository.GetCampAsync(moniker,true);

                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

        }


        [HttpGet("/search")]
        public async Task<ActionResult<CampModel[]>> searchCamp(DateTime date, bool includeTalks = false)
        {

            try
            {
                var results = await repository.GetAllCampsByEventDate(date);

                if (!results.Any()) return NotFound();

                return _mapper.Map<CampModel[]>(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> post(CampModel model)
        {
            try
            {
                var Location=_linkGenerator.GetPathByAction("Get","Camps",new{moniker=model.Moniker});
                if(string.IsNullOrWhiteSpace(Location)){
                    return BadRequest("Can not Use This moniker");
                }

                var campExisting=await repository.GetCampAsync(model.Moniker);
                
                if(campExisting !=null){
                    return BadRequest("moniker in Use");
                }


                var camp = _mapper.Map<Camp>(model);
                repository.Add(camp);

                if (await repository.SaveChangesAsync())
                {
                    return Created(Location, _mapper.Map<CampModel>(camp));
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest("Failed to create!");
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker,CampModel model){

            try
            {
                var oldCamp=await repository.GetCampAsync(moniker);
                if(oldCamp ==null) return  NotFound($"Can not Find moniker with this {moniker}"); 

                _mapper.Map(model,oldCamp);      

                if(await repository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }     
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest("Faild To update");
        }
   
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker){
            try
            {
                var oldCamp=await repository.GetCampAsync(moniker);
                if(oldCamp ==null) return NotFound($"Can not Find moniker with name {moniker}");

                 repository.Delete(oldCamp);

                 if(await repository.SaveChangesAsync()){
                     return Ok($"The item with {moniker} has been deleted");
                 }

            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest("Failed To delete");
        }
    }
}