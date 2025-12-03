using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Infrastructure.ClaimEngine
{
    public class TransactionAsync : BackgroundService
    {

        private readonly ILogger<TransactionAsync> _logger;
        //readonly IEntryPointService _trns;
        //public TransactionAsync(ILogger<TransactionAsync> logger, IEntryPointService incomingTransactions)
        //{
        //    _logger = logger;
        //    _trns = incomingTransactions;
        //}
        public TransactionAsync(ILogger<TransactionAsync> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // await _trns.ExecuteAsync();
                // _transactionServices.processTransactions();
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(20000, stoppingToken);
            }
        }
    }
}
