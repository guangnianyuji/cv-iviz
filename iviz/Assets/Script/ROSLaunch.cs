using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

public class ROSLaunch : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI textMeshPro;
    public TextMeshProUGUI textMeshPro2;
    public TMP_Dropdown  dd;

    public GameObject buildpanel;

    private NetworkStream stream;

    private TcpClient client;
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
        string server_str = textMeshPro.text;
         
        Debug.Log(server_str);
        string port_str=textMeshPro2.text;
  
        int port=0;
         try
        {
            // Convert the strings to floats
            
           string sp=port_str;
           int l=sp.Length;
            for(int i=0;i<l;i++)
            {
                // Debug.Log("letter");
                // Debug.Log("第i个");
                // Debug.Log(i);
                // Debug.Log("是");
                // Debug.Log(sp[i]);
                if(sp[i]>'9' ||sp[i]<'0' ) continue;
                port=port*10+sp[i]-'0';
            }
        }
        catch 
        { 
            Debug.Log("Port Convert Failed;");
        }
 
        Debug.Log(port);

        int id=dd.value;
        Debug.Log(id);

        string command="";
        if(id==0)
        {
            command="roslaunch cartographer_ros demo_backpack_3d.launch bag_filename:=${HOME}/Downloads/b3-2016-04-05-14-14-00.bag";
        }

        Connect(server_str,port);
    }

        
    public void Connect(string server, int port)
    {
        client = new TcpClient();
        try
        {
            client.Connect(server, port);
            Debug.Log("Connected to server.");
            stream = client.GetStream();
        }
        catch
        {
            Debug.LogError("Failed to connect to server" );
        }
        //192.168.43.224

    }

        public void SendCommand(string command)
    {
        if (client != null && client.Connected)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
        else
        {
            Debug.LogError("Not connected to server.");
        }
    }
}