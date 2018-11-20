using System;
using Just.Anarchy.Actions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Core.Utils;
using Just.Anarchy.Exceptions.Handlers;
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
            builder.Services.AddSingleton<IAnarchyManagerNew, AnarchyManagerNew>();
            builder.Services.AddSingleton<IHandleTime, Timer>();
            builder.Services.AddSingleton<IHandleAnarchyExceptions, ExceptionHandlerManager>();
            builder.Services.AddSingleton<IExceptionHandler, AnarchyActionNotFoundExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, ScheduleRunningExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, ScheduleExistsExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, UnschedulableActionExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, UnrequestableActionExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, MultipleResponseAlteringActionsEnabledExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, SetActionTargetPatternRequestBodyRequiredExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, InvalidTargetPatternExceptionHandler>();
            builder.Services.AddSingleton<IExceptionHandler, EmptyTargetPatternExceptionHandler>();
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