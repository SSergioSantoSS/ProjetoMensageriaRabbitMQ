using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjMens.Services.Api.Contexts;
using ProjMens.Services.Api.Contexts.Entities;
using ProjMens.Services.Api.Models;
using ProjMens.Services.Api.Produces;

namespace ProjMens.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly SqlServerContext? _sqlServerContext;
        private readonly ProduceMessage? _produceMessage;



        public UsuariosController(SqlServerContext? sqlServerContext, ProduceMessage? produceMessage)
        {
            _sqlServerContext = sqlServerContext;
            _produceMessage = produceMessage;
        }

        [HttpPost]
        public IActionResult Post(UsuarioViewModel model)
        {
            try
            {
                var usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = model.Nome,
                    Email = model.Email,
                    Cpf = model.Cpf,
                    DataHoraCadastro = DateTime.Now
                };

                _sqlServerContext?.Usuarios?.Add(usuario);
                _sqlServerContext?.SaveChanges();

                //publicando uma mensagem na fila..
                _produceMessage?.Publish(new MessageViewModel
                {
                    
                    From = "PostUsuarios", //nome do serviço que originou a mensagem                       
                    To = "EmailService", //nome da mensagem                      
                    Content = JsonConvert.SerializeObject(usuario) //dados da mensagem

                });

                return StatusCode(201, new { usuario.Id, model });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }



}
