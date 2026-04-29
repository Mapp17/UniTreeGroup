using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class AutomatedPayoutBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutomatedPayoutBackgroundService> _logger;

    public AutomatedPayoutBackgroundService(IServiceProvider serviceProvider, ILogger<AutomatedPayoutBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Automated Payout Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking for scheduled payouts...");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var transactionsService = scope.ServiceProvider.GetRequiredService<TransactionsServices>();

                    // Get all scheduled payouts due now
                    var duePayouts = unitOfWork.Payouts.Find(p => p.Status == PayoutScheduleStatus.Scheduled && p.ScheduledDate <= DateTime.UtcNow).ToList();

                    foreach (var payout in duePayouts)
                    {
                        _logger.LogInformation($"Processing payout for User {payout.BeneficiaryUserId}, Amount: {payout.Amount}");

                        try
                        {
                            // Execute the payout transaction
                            var transactionDto = new TransactionRequestDto
                            {
                                UserId = payout.BeneficiaryUserId,
                                Amount = payout.Amount,
                                Reference = $"AUTO-PAYOUT-{payout.Id}-{DateTime.UtcNow.Ticks}",
                                Description = $"Automated scheduled payout #{payout.Id}",
                                UniTreeGroupId = 1 
                            };

                            transactionsService.ProcessPayout(transactionDto);

                            //Update Payout Schedule status
                            payout.Status = PayoutScheduleStatus.Completed;
                            payout.ProcessedAt = DateTime.UtcNow;
                            unitOfWork.Payouts.Update(payout);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to process payout {payout.Id}");
                            // Or handle retry logic
                            payout.Status = PayoutScheduleStatus.Skipped; 
                            unitOfWork.Payouts.Update(payout);
                        }
                    }

                    await unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing automated payouts.");
            }

            // Run checks
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("Automated Payout Background Service is stopping.");
    }
}