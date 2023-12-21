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
    public class ROSLaunch : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI textMeshPro;
        public TextMeshProUGUI textMeshPro2;
        public TMP_Dropdown dd;

        public GameObject buildpanel;

        private NetworkStream stream;

        private TcpClient client;

        public GameObject killButton;
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ButtonClick);
        }

        // Update is called once per frame
        void Update()
        {

        }


        // void  ButtonClick()
        // {

        //     var httpClient = new HttpClient();
        //     string url = textMeshPro.text; // Flask服务器地址
        //         // 创建要发送的JSON数据

        //     int id=dd.value;
        //     Debug.Log(id);

        //     string command="";
        //     if(id==0)
        //     {
        //         command="roslaunch cartographer_ros demo_backpack_3d.launch bag_filename:=${HOME}/Downloads/b3-2016-04-05-14-14-00.bag";
        //     }

        //     var json = JsonSerializer.Serialize(new { command = command });
        //     var content = new StringContent(json, Encoding.UTF8, "application/json");

        //     try
        //     {
        //         // 发送POST请求
        //         var response = await httpClient.PostAsync(url, content);

        //         // 读取并输出响应内容
        //         var responseContent = await response.Content.ReadAsStringAsync();
        //         Console.WriteLine(responseContent);
        //     }
        //     catch (Exception ex)
        //     {
        //         // 错误处理
        //         Console.WriteLine($"请求发送失败: {ex.Message}");
        //     }
        // }
        void ButtonClick()
        {
            string server_str = textMeshPro.text.Trim((char)8203);

            Debug.Log(server_str);
            string port_str = textMeshPro2.text.Trim((char)8203);

            int port = 0;
            if (!int.TryParse(port_str, out port))
            {
                Debug.Log("Port Convert Failed;");
                return;
            }

            Debug.Log(port);

            int id = dd.value;
            Debug.Log(id);
            RosStartCommand command = new RosStartCommand();
            RosConnector.get().setAddress(server_str, port);
            if (id == 0)
            {
                //"roslaunch cartographer_ros demo_backpack_3d.launch bag_filename:=${HOME}/Downloads/b3-2016-04-05-14-14-00.bag";
                command.bagName = "b3-2016-04-05-14-14-00";
            }

            Task a = Task.Run(async () =>
            {
                await RosConnector.get().sendCommandAsync(command);
                
            });
            buildpanel.SetActive(false);

        }
    }
}

