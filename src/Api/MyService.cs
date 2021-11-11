using Autofac.Features.AttributeFilters;

namespace Api;

public class MyService : IMyService
{
    private readonly IGreeting _hello;
    private readonly IGreeting _goodbye;

    public MyService(
        [KeyFilter("hello")] IGreeting hello,
        [KeyFilter("goodbye")] IGreeting goodbye
    )
    {
        _hello = hello;
        _goodbye = goodbye;
    }

    public string Speak() => $"{_hello.Speak()} and {_goodbye.Speak()}";
}