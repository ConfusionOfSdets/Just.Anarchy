using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Actions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Timer = Just.Anarchy.Core.Utils.Timer;

namespace Just.Anarchy
{
    public static class MvcCoreBuilderExtensions
    {
        public static IMvcBuilder AddAnarchy(this IMvcBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddAnarchyCore();
            builder.AddAnarchyDefaultActions();

            return builder;
        }

        public static IMvcBuilder AddAnarchyCore(this IMvcBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddApplicationPart(typeof(MvcCoreBuilderExtensions).Assembly);
            builder.Services.AddSingleton<IAnarchyManagerNew, AnarchyManagerNew>();
            builder.Services.AddSingleton<IHandleTime, Timer>();
            builder.Services.AddSingleton<IHandleAnarchyExceptions, ExceptionHandlerManager>();
            builder.Services.Scan(scan => 
                scan.FromAssemblyOf<ExceptionHandlerManager>()
                    .AddClasses(classes => classes.AssignableTo<IExceptionHandler>())
                    .As<IExceptionHandler>()
                    .WithSingletonLifetime());
                
            builder.Services.AddSingleton<ISchedulerFactory, SchedulerFactory>();

            return builder;
        }

        public static IMvcBuilder AddAnarchyDefaultActions(this IMvcBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddAnarchyAction<DelayAnarchy>();
            builder.Services.AddAnarchyAction<CpuAnarchy>();
            builder.Services.AddAnarchyAction<MemoryAnarchy>();
            builder.Services.AddAnarchyAction<RandomErrorResponseAnarchy>();

            return builder;
        }

        public static IApplicationBuilder UseAnarchy(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.UseMiddleware<AnarchyMiddleware>();
            return builder;
        }

        public static void AddAnarchyAction<TAnarchyAction>(this IServiceCollection services) where TAnarchyAction : class, ICauseAnarchy
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<TAnarchyAction>();
            services.AddSingleton<IActionOrchestrator, ActionOrchestrator<TAnarchyAction>>();
        }
    }
}