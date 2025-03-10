using Microsoft.Extensions.Logging;

namespace o2rabbit.Utilities.Extensions;

public static class LoggerExtensions
{
    public static void LogAggregateException(this ILogger logger, AggregateException exception)
    {
        foreach (Exception exInnerException in exception.Flatten().InnerExceptions)
        {
            Exception exNestedInnerException = exInnerException;
            do
            {
                if (!string.IsNullOrEmpty(exNestedInnerException.Message))
                {
                    logger.LogError(exNestedInnerException, exNestedInnerException.Message);
                }

                exNestedInnerException = exNestedInnerException.InnerException;
            } while (exNestedInnerException != null);
        }
    }

    public static void CustomExceptionLogging(this ILogger logger, Exception e)
    {
        logger.LogError(e, e.Message);
        if (e is AggregateException aggregateException)
            logger.LogAggregateException(aggregateException);
    }
}