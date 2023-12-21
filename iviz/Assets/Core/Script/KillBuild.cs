using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Iviz.Core
{
    public class KillBuild : MonoBehaviour
    {
        public GameObject killButton;
        public GameObject startButton;
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ButtonClick);
            killButton.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void ButtonClick()
        {
            RosKillCommand command = new RosKillCommand();
            Task a = Task.Run(async () =>
            {
                await RosConnector.get().sendCommandAsync(command); 
            });
            killButton.SetActive(false);
            startButton.SetActive(true);
        }
    }
}
