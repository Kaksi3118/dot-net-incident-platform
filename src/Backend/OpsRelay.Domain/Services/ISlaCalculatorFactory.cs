using OpsRelay.Domain.Enums;

namespace OpsRelay.Domain.Services;

public interface ISlaCalculatorFactory
{
    ISlaCalculator GetCalculator(Priority priority);
}
