using ConsoleAdmin;
using System.Collections.Generic;
using System.Text.Json;

Functions f = new Functions();
bool exit = f.exit;

while (!exit)
{
    Console.WriteLine("Please enter a command: ");
    string com = Console.ReadLine();

    f.GetCommand(com);
    f.HandleInput();

    exit = f.exit;
}