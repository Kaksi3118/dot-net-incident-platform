using OpsRelay.Domain.Enums;
using OpsRelay.Domain.Services;

namespace OpsRelay.Infrastructure.Services;

public class SlaCalculatorFactory(IEnumerable<ISlaCalculator> calculators)
    : ISlaCalculatorFactory
{
    public ISlaCalculator GetCalculator(Priority priority) =>
        calculators.FirstOrDefault(calculator => calculator.CanHandle(priority))
        ?? throw new InvalidOperationException(
            $"No SLA calculation strategy registered for priority '{priority}'.");
}
