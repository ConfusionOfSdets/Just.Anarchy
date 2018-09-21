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
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new DelayAnarchy()));
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new CpuAnarchy()));
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new MemoryAnarchy()));
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new RandomErrorResponseAnarchy()));
            return builder;
        }

        public static IApplicationBuilder UseAnarchy(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AnarchyMiddleware>();
            return builder;
        }
    }
}