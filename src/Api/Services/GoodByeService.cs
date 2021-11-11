using Api.Abstractions;

namespace Api.Services;

public class GoodByeService : IGreeting
{
    public string Speak() => "Goodbye!";
}