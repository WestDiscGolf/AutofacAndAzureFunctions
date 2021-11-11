using Api.Abstractions;

namespace Api.Services;

public class HelloService : IGreeting
{
    public string Speak() => "Hello!";
}