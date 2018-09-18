using System;
using Just.Anarchy.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Just.Anarchy
{
    public static class MvcCoreBuilderExtensions
    {
        public static IMvcBuilder AddAnarchy(this IMvcBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddApplicationPart(typeof(MvcCoreBuilderExtensions).Assembly);
            builder.Services.AddSingleton<IAnarchyManager, AnarchyManager>();
            builder.Services.AddTransient<ICauseAnarchy, DelayAnarchy>();
            builder.Services.AddTransient<ICauseAnarchy, CpuAnarchy>();
            builder.Services.AddTransient<ICauseAnarchy, MemoryAnarchy>();
            builder.Services.AddTransient<ICauseAnarchy, RandomErrorResponseAnarchy>();
            return builder;
        }

        public static IApplicationBuilder UseAnarchy(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AnarchyMiddleware>();
            return builder;
        }
    }
}