using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glasswall.Administration.K8.TransactionQueryService.Controllers
{
    [ApiController]
    [Route("api/v1/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> GetTransactions([Required][FromBody]GetTransactionsRequestV1 request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Beginning get transactions request");

            var transactions = await _transactionService.GetTransactionsAsync(request, cancellationToken);

            _logger.LogInformation("Finished get transactions request");

            return Ok(transactions);
        }

        [HttpGet]
        [ValidateModel]
        public async Task<IActionResult> GetDetail([Required] [FromQuery] string filePath, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Beginning get detail request");

            var detail = await _transactionService.GetDetailAsync(filePath, cancellationToken);

            _logger.LogInformation("Finished get detail request");

            return Ok(detail);
        }
    }
}
