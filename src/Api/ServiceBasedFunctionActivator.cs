using System;
using Microsoft.Azure.Functions.Worker;

namespace Api;

public class ServiceBasedFunctionActivator : IFunctionActivator
{
    public object? CreateInstance(Type instanceType, FunctionContext context)
    {
        _ = instanceType ?? throw new ArgumentNullException(nameof(instanceType));
        _ = context ?? throw new ArgumentNullException(nameof(context));
        
        return context.InstanceServices.GetService(instanceType);
    }
}