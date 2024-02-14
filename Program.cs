using ConsoleAdmin;
using System.Collections.Generic;
using System.Text.Json;

Functions f = new Functions();
bool exit = f.exit;

Console.WriteLine("Type 'help' for a list of commands\n");

while (!exit)
{
    string com = Console.ReadLine();

    f.GetCommand(com);
    f.HandleInput();

    Console.Write("\n");

    exit = f.exit;
}