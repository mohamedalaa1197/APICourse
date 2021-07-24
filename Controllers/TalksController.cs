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
    [ApiController]
    [Route("/api/camps/{moniker}/talks")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
            _mapper = mapper;
            _repository = repository;

        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {

            try
            {
                var camp=await _repository.GetCampAsync(moniker);
                if(camp ==null) return NotFound($"No camp with moniker{moniker}");

                var talks = await _repository.GetTalksByMonikerAsync(moniker);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in the server");
            }

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                return _mapper.Map<TalkModel>(talk);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in the server");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> post(string moniker, TalkModel model)
        {

            try
            {
                System.Console.WriteLine(moniker);
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp is not Found!");

                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;
                
                _repository.Add(talk);

                if (await _repository.SaveChangesAsync())
                {
                    System.Console.WriteLine("Error is Here");
                    var Location=_linkGenerator.GetPathByAction(HttpContext,"Get",
                    values:new{moniker,id=talk.TalkId});

                    return Created(Location,_mapper.Map<TalkModel>(talk));
                }
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in the server");
            }
            
            return BadRequest("Failed To Create");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>>put(string moniker,int id,TalkModel model){

            try
            {
               var talk=await _repository.GetTalkByMonikerAsync(moniker,id,true);
               if(talk==null)return NotFound("talk Not Found");

               _mapper.Map(model,talk); 

               if(model.Speaker !=null){
                   var speaker=await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                   
                   if(speaker!=null) {
                       talk.Speaker=speaker;
                   }
               }

               if(await _repository.SaveChangesAsync()){
                   return _mapper.Map<TalkModel>(talk);
               }else{
                   return BadRequest("Failed To update");
               }

            }
            catch (System.Exception)
            {
                
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> delete(string moniker,int id){
            try
            {
                var talk=await _repository.GetTalkByMonikerAsync(moniker,id);
                if(talk==null) return NotFound("Talk is not Found");

                _repository.Delete(talk);
                if(await _repository.SaveChangesAsync()){
                    
                    return Ok("Talk has been deleted");

                }else{
                    return BadRequest("Failed To delete the talk");
                }


            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in the server");
            }
               
        }
    }
}