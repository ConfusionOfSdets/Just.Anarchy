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
            builder.Services.AddSingleton<IHandleTime, Timer>();
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new DelayAnarchy(), c.GetService<IHandleTime>()));
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new CpuAnarchy(), c.GetService<IHandleTime>()));
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new MemoryAnarchy(), c.GetService<IHandleTime>()));
            builder.Services.AddTransient<IAnarchyActionFactory>(c => new AnarchyActionFactory(new RandomErrorResponseAnarchy(), c.GetService<IHandleTime>()));
            return builder;
        }

        public static IApplicationBuilder UseAnarchy(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AnarchyMiddleware>();
            return builder;
        }
    }
}