using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Api;

public class HttpRequestMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // determine the type, the default is Microsoft.Azure.Functions.Worker.Context.Features.GrpcFunctionBindingsFeature
        (Type featureType, object featureInstance) = context.Features.SingleOrDefault(x => x.Key.Name == "IFunctionBindingsFeature");

        // find the input binding of the function which has been invoked and then find the associated parameter of the function for the data we want
        var inputData = featureType.GetProperties().SingleOrDefault(p => p.Name == "InputData")?.GetValue(featureInstance) as IReadOnlyDictionary<string, object>;
        var requestData = inputData?.Values.SingleOrDefault(obj => obj is HttpRequestData) as HttpRequestData;

        if (requestData is not null)
        {
            // set the request data on the accessor from DI
            var accessor = context.InstanceServices.GetRequiredService<IHttpRequestAccessor>();
            accessor.HttpRequest = requestData;
        }

        await next(context);
    }
}