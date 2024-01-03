using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Iviz.App
{
    public class RosCommand
    {
        public string command;
    }

    public class RosStartCommand : RosCommand
    {
        public string bagName;

        public RosStartCommand()
        {
            command = "start";
        }
    }

    public class RosKillCommand : RosCommand 
    {
        public RosKillCommand()
        {
            command = "kill";
        }
    }

    public class RosConnector
    {
        private static RosConnector instance;
        private string serverAddress;
        private HttpClient httpClient;
        public static RosConnector get()
        {
            if (instance == null)
                instance = new RosConnector();
            return instance;
        }

        public void setAddress(string hostName, int port)
        {
            serverAddress = "http://" + hostName + ":" + port;
            httpClient = new()
            {
                BaseAddress = new Uri(serverAddress),
            };
        }

        public async Task sendCommandAsync(RosCommand command)
        {
            string commandJson = JsonConvert.SerializeObject(command, Formatting.Indented);
            StringContent content = new StringContent(commandJson, Encoding.UTF8, "application/json");



            using HttpResponseMessage response = await httpClient.PostAsync("command",content);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Debug.Log($"{jsonResponse}\n");
        }


    }
}
