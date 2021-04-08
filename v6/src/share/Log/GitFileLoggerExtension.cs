using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Laobian.Share.Log
{
    public static class GitFileLoggerExtension
    {
        public static ILoggingBuilder AddGitFile(this ILoggingBuilder builder,
            Action<GitFileLoggerOptions> configure = null)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, GitFileLoggerProvider>());
            LoggerProviderOptions
                .RegisterProviderOptions<GitFileLoggerOptions, GitFileLoggerProvider>(builder.Services);
            if (configure != null) builder.Services.Configure(configure);

            return builder;
        }
    }
}