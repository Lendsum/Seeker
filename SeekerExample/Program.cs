using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeekerExample.Dtos;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SeekerExample
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            var command = string.Empty;


            while (command != "stop")
            {
                if (command == "reset")
                {
                    var result=client.DeleteAsync("http://localhost:54818/api/Scenario/1").Result.Content.ReadAsStringAsync().Result;
                    command = "";
                }
                else
                {

                    var direction = Transform(command);
                    var response = client.PostAsync("http://localhost:54818/api/Scenario?id=1&direction=" + direction, null).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;

                    response = client.GetAsync("http://localhost:54818/api/Scenario?id=1").Result;
                    responseString = response.Content.ReadAsStringAsync().Result;
                    var scenario = JsonConvert.DeserializeObject<Scenario>(responseString);

                    for (int y = scenario.Items.Min(x => x.Y); y <= scenario.Items.Max(x => x.Y); y++)
                    {
                        StringBuilder builder = new StringBuilder();
                        for (int x = scenario.Items.Min(item => item.X); x <= scenario.Items.Max(item => item.X); x++)
                        {
                            builder.Append(Transform(scenario.Items.Where(item => item.X == x && item.Y == y).FirstOrDefault()));
                        }

                        Console.WriteLine(builder.ToString());
                    }
                }


                command = Console.ReadLine();
            }
        }

        private static string Transform(ScenarioItem item)
        {
            if (item == null) return " ";
            if (item.ItemType == ScenarioItemType.Block) return "#";
            if (item.ItemType == ScenarioItemType.Target) return "X";
            if (item.ItemType == ScenarioItemType.Player) return "!";
            return " ";
        }

        private static string Transform(string direction)
        {
            if (string.IsNullOrWhiteSpace(direction)) return "0";
            if (direction == "up") return "10";
            if (direction == "down") return "20";
            if (direction == "left") return "30";
            if (direction == "right") return "40";

            return "0";
        }
    }
}

