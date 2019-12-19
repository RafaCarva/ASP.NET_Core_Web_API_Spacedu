using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using System;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrrasController : ControllerBase
    {
        private readonly MimicContext _banco;

        public PalavrrasController(MimicContext banco)
        {
            _banco = banco;
        }

        // /api/palavras?data=2019-05-22
        // /api/palavras
        // /api/palavras?pagNumero=1&pagRegistro=2
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas(DateTime? data, int? pagNumero, int? pagRegistro)
        {
            var item = _banco.Palavras.AsQueryable();
            if (data.HasValue)
            {
                item = item.Where(a=>a.Criado > data.Value || a.Atualizado > data.Value);
            }

            if (pagNumero.HasValue)
            {
                item = item.Skip((pagNumero.Value -1)*pagRegistro.Value).Take(pagRegistro.Value);
                var paginacao = new Paginacao();

            }

            return Ok(item);
        }

        // WEB ->  /api/palavras/5
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _banco.Palavras.Find(id);
            if (obj == null) return NotFound();
            return Ok();
        }

        //  api/palavras
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        // /api/palavras/1  (PUT: id, nome, ativo, pontuacao, criacao)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(async=>async.Id == id);
            if (obj == null) return NotFound();

            palavra.Id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }

        // /api/palavras/1 
        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            // Deletar
            // _banco.Palavras.Remove(_banco.Palavras.Find(id));

            // Exclusão lógica
            var palavra = _banco.Palavras.Find(id);
            if (palavra == null) return NotFound();

            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }
    }
}
