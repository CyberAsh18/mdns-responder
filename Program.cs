// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using Makaretu.Dns;

string service = "incus.local";

Console.Write("Enter ipv4 for incus.local: ");
string? ipv4 = Console.ReadLine();

bool IsValidIPv4(string? ipString)
{
    if (!IPAddress.TryParse(ipString, out _))
    {
        Console.WriteLine("Invalid ipv4 provided, try again...");
        Console.Write("Enter ipv4 for incus.local: ");
        ipv4 = Console.ReadLine();
        return IsValidIPv4(ipv4);
    }
    return true;
}

IsValidIPv4(ipv4);

var ipv4Address = IPAddress.Parse(ipv4!);
var mdns = new MulticastService();

Console.WriteLine($"Listening to incus.local ({ipv4Address}) queries, Press any key to stop...");

mdns.QueryReceived += (s, e) =>
{
    var msg = e.Message;
    if (msg.Questions.Any(q => q.Name == service))
    {
        Console.WriteLine($"Query for {msg.Questions[0].Name}");
        var res = msg.CreateResponse();
        res.Answers.Add(new ARecord
        {
            Name = service,
            Address = ipv4Address,
        });
        mdns.SendAnswer(res);
    }
};
mdns.Start();

Console.ReadKey();

mdns.Stop();