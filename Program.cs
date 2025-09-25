using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Resultado 1
        string team1 = "Paris Saint-Germain";
        int year1 = 2013;
        int goalsPSG = await GetTotalGoals(year1, team1);
        Console.WriteLine($"Team {team1} scored {goalsPSG} goals in {year1}");

        // Resultado 2
        string team2 = "Chelsea";
        int year2 = 2014;
        int goalsChelsea = await GetTotalGoals(year2, team2);
        Console.WriteLine($"Team {team2} scored {goalsChelsea} goals in {year2}");
    }

    static async Task<int> GetTotalGoals(int year, string team)
    {
        int totalGoals = 0;
        using (HttpClient client = new HttpClient())
        {
            // Gols quando o time é team1
            totalGoals += await GetGoalsByRole(client, year, team, "team1");

            // Gols quando o time é team2
            totalGoals += await GetGoalsByRole(client, year, team, "team2");
        }
        return totalGoals;
    }

    static async Task<int> GetGoalsByRole(HttpClient client, int year, string team, string role)
    {
        int page = 1;
        int goals = 0;

        while (true)
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{role}={Uri.EscapeDataString(team)}&page={page}";
            var response = await client.GetStringAsync(url);

            using (JsonDocument doc = JsonDocument.Parse(response))
            {
                var root = doc.RootElement;
                int totalPages = root.GetProperty("total_pages").GetInt32();
                var data = root.GetProperty("data");

                foreach (var match in data.EnumerateArray())
                {
                    string goalsStr = match.GetProperty(role + "goals").GetString();
                    goals += int.Parse(goalsStr);
                }

                if (page >= totalPages)
                    break;
                page++;
            }
        }

        return goals;
    }
}
