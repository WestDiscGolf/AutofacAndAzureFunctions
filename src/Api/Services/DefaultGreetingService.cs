using Api.Abstractions;

namespace Api.Services;

public class DefaultGreetingService : IGreeting
{
    public string Speak() => "Not coming or going :-/";
}