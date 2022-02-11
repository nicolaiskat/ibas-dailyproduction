using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DailyProduction.Model;
using Azure.Data.Tables;
using Azure;
using Azure.Data.Tables.Models;

namespace DailyProduction.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyProductionController : ControllerBase
    {
        private readonly ILogger<DailyProductionController> _logger;

        private TableClient _tableClient;

        public DailyProductionController(ILogger<DailyProductionController> logger)
        {
            _logger = logger;

            var serviceuri = "https://nsni.table.core.windows.net/IBASProduktion2020";
            var tablename = "IBASProduktion2020";
            var accountname = "nsni";
            var storagekey = "sY3n+ovJto+yCVOFw7BeU/cfL1wF1YTOmuYCewsRkLiA8/A2B+gRoDbnJOMBXn1ovXwxy5x+cs3Fl0kfaVZCkA==";

            this._tableClient = new TableClient(
                    new Uri(serviceuri),
                    tablename,
                    new TableSharedKeyCredential(accountname, storagekey)
                );
        }


        [HttpGet]
        public IEnumerable<DailyProductionDTO> Get()
        {
            var production = new List<DailyProductionDTO>();
            Pageable<TableEntity> entities = this._tableClient.Query<TableEntity>();

            foreach (var entity in entities)
            {
                var dto = new DailyProductionDTO
                {
                    Date = DateTime.Parse(entity.RowKey),
                    Model = (BikeModel)Enum.ToObject(typeof(BikeModel), Int32.Parse(entity.PartitionKey)),
                    ItemsProduced = (int)entity.GetInt32("itemsProduced")
                };
                production.Add(dto);
            }
            return production;
        }
    }
}