using Blockcore.DataVault.Models;
using Blockcore.DataVault.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("dv")]
    public class VaultController : ControllerBase
    {
        private readonly IStorageRepository repo;

        private readonly ILogger<VaultController> log;

        public VaultController(IStorageRepository repo, ILogger<VaultController> log)
        {
            this.repo = repo;
            this.log = log;
        }

        [HttpGet("{collection}")]
        public async Task<IActionResult> Get([FromRoute] string collection, [FromQuery] string filter)
        {
            this.log.LogInformation($"Getting the collection {collection}");

            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentNullException("collection");
            }

            return new ObjectResult(await repo.Get(collection, filter));
        }

        [HttpGet("{collection}/{id}")]
        public async Task<IActionResult> GetById([FromRoute] string collection, [FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentNullException("collection");
            }

            var entity = await repo.GetById(collection, id);

            if (entity == null)
            {
                return new NotFoundResult();
            }

            return Ok(entity);
        }

        [HttpPost("{collection}")]
        public async Task<IActionResult> Post([FromRoute] string collection)
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentNullException("collection");
            }

            string json = string.Empty;

            using (StreamReader stream = new StreamReader(Request.Body, Encoding.UTF8))
            {
                json = await stream.ReadToEndAsync();
            }

            var document = BsonSerializer.Deserialize<BsonDocument>(json);

            // We need to ensure that we remove "_id" if it supplied in the body.
            document.Remove("_id");

            await repo.Create(collection, document);

            return Ok(new ResultModel { Result = document["_id"] });
        }

        [HttpPut("{collection}/{id}")]
        public async Task<IActionResult> Put([FromRoute] string collection, [FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentNullException("collection");
            }

            string json = string.Empty;

            using (StreamReader stream = new StreamReader(Request.Body, Encoding.UTF8))
            {
                json = await stream.ReadToEndAsync();
            }

            var entityFromDb = await repo.GetById(collection, id);

            if (entityFromDb == null)
            {
                log.LogInformation($"{id} was not found in {collection}.");

                return new NotFoundResult();
            }

            var document = BsonSerializer.Deserialize<BsonDocument>(json);

            // We need to ensure that we remove "_id" if it supplied in the body.
            document.Remove("_id");

            var result = await repo.Update(collection, id, document);

            return Ok(new ResultModel { Result = result });
        }

        [HttpDelete("{collection}/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string collection, [FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentNullException("collection");
            }

            var entity = await repo.GetById(collection, id);

            if (entity == null)
            {
                return new NotFoundResult();
            }

            var result = await repo.Delete(collection, id);

            return Ok(new ResultModel { Result = result });
        }
    }
}
