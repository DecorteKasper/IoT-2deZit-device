using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using IoT_2deZit_device.Models;
using System.Text;


string connectionString = "HostName=kasperiothub.azure-devices.net;DeviceId=kasperdecorte;SharedAccessKey=C3FVCWOCzgQOvmgDGphMw9MP571ulom+CUPQFcpsEtM=";
var deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
DesiredProperties desiredProperties = new DesiredProperties();

await deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null);
await deviceClient.OpenAsync();
await GetTwinConfigInfo();



await Loop();

async Task Loop()
{
    while (true)
    {
        Console.WriteLine("Maak uw keuze: ");
        Console.WriteLine("1. Persoon komt binnen op het event");
        Console.WriteLine("2. Persoon verlaat het event");
        Console.WriteLine("9. Afsluiten");
        string keuze = Console.ReadLine();

        await ProcessPerson(keuze);
    }
}

async Task ProcessPerson(string keuze)
{
    switch (keuze)
    {
        case "1":
            await EventIn();
            break;
        case "2":
            await EventOut();
            break;
        case "9":
            Environment.Exit(0);
            break;
        default:
            Console.WriteLine("Ongeldige keuze");
            break;
    }
}

//Persoon komt event binnen
async Task EventIn()
{
    PersonEnteredMessage messageObject = new PersonEnteredMessage()
    {
        Naam = "Kasper Decorte",
        Leeftijd = 21,
        Datum = DateTime.Now,
        DeviceId = "kasperdecorte",
        GuidId = 1
    };

    Console.WriteLine("Persoon komt binnen op het event");

    var json = JsonConvert.SerializeObject(messageObject);
    var message = new Message(Encoding.UTF8.GetBytes(json));

    await deviceClient.SendEventAsync(message);
}

//Persoon verlaat event
async Task EventOut()
{
    PersonLeftMessage messageObject = new PersonLeftMessage()
    {
        GuidId = 1,
        DeviceId = "kasperdecorte"
    };

    Console.WriteLine("Persoon verlaat het event");

    var json = JsonConvert.SerializeObject(messageObject);
    var message = new Message(Encoding.UTF8.GetBytes(json));

    await deviceClient.SendEventAsync(message);
}



#region Boot

async Task GetTwinConfigInfo()
{
    var twin = await deviceClient.GetTwinAsync();
    var twinCollection = twin.Properties.Desired;
    var twinJson = twinCollection.ToJson();
    desiredProperties = JsonConvert.DeserializeObject<DesiredProperties>(twinJson);
}

#endregion

#region Update desired properties

async Task OnDesiredPropertyChanged(TwinCollection props, object userContext)
{
    var twinJson = props.ToJson();
    desiredProperties = JsonConvert.DeserializeObject<DesiredProperties>(twinJson);
    Console.WriteLine($"Received twin update with value: {twinJson}");
}

#endregion