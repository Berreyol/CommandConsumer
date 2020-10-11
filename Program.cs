using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CommanderConsumer
{
    class CommanderConsumer
    {
        static HttpClient client = new HttpClient();

        static void ShowCommand(Command command)
        {
            Console.WriteLine($"HowTo: {command.HowTo}\tLine: " +
                $"{command.Line}\tPlataform: {command.Plataform}");
        }

        static async Task<Uri> CreateCommandAsync(Command command)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/commands", command);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Command> GetCommandAsync(string path)
        {
            Command command = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                command = await response.Content.ReadAsAsync<Command>();
            }
            return command;
        }

        static async Task<Command> UpdateCommandAsync(Command command)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/commands/{command.Id}", command);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated command from the response body.
            command = await response.Content.ReadAsAsync<Command>();
            return command;
        }

        static async Task<HttpStatusCode> DeleteCommandAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/commands/{id}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new command
                Command command = new Command
                {
                    HowTo = "Run migrations in Entity Framework",
                    Line = "dotnet ef migrations add <Name of the migration>",
                    Plataform = "EF Core"
                };

                var url = await CreateCommandAsync(command);
                Console.WriteLine($"Created at {url}");

                // Get the command
                command = await GetCommandAsync(url.PathAndQuery);
                ShowCommand(command);

                // Update the command
                Console.WriteLine("Updating price...");
                command.Line = "New Value";
                

                await UpdateCommandAsync(command);

                // Get the updated command
                command = await GetCommandAsync(url.PathAndQuery);
                ShowCommand(command);

                // Delete the command
                var statusCode = await DeleteCommandAsync(command.Id);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }
    }
}
