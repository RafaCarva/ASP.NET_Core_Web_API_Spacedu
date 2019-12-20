using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;

        public PalavrrasController(IPalavraRepository repository)
        {
            _repository = repository;
        }

        // /api/palavras?data=2019-05-22
        // /api/palavras
        // /api/palavras?pagNumero=1&pagRegistro=2
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);

            if (query.PagNumero > item.Paginacao.TotalPaginas)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

            return Ok(item.ToList());
        }

        // WEB ->  /api/palavras/5
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);
            if (obj == null) return NotFound();
            return Ok(obj);
        }

        //  api/palavras
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            _repository.Cadastrar(palavra);
            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        // /api/palavras/1  (PUT: id, nome, ativo, pontuacao, criacao)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _repository.Obter(id);
            if (obj == null) return NotFound();

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok();
        }

        // /api/palavras/1 
        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);
            if (palavra == null) return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
    }
}
