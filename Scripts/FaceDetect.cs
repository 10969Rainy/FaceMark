using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Baidu.Aip.Face;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections;

public class FaceDetect : MonoBehaviour {

    public Text infoText;                            // 显示debug信息

    private Face client;                              // 用来调用百度AI接口
    private Dictionary<string, object> options;       // 返回的数据
    private JObject result;                           // 接收返回的结果

    public Text groupIdText;
    private int index = 0;
    public int topIdentifyNum = 5;

    private int error_code = 0;      //百度API返回的错误代码编号  
    private string error_msg;      //错误描述信息，帮助理解和解决发生的错误。  

    private SQLiteHelper sql;

    void Awake()
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback += 
            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                System.Security.Cryptography.X509Certificates.X509Chain chain,
                System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;           // always accept
                };

        client = new Face("w0o1zVTMpMYva0CAz64VBerV", "7gk7xRQOscdkUPQUcaLZtFBj4lhV2LMS");                 // 此处填写自己申请的key

        options = new Dictionary<string, object>()
        {
            {"detect_top_num", topIdentifyNum},
            {"user_top_num", 1}
        };

        infoText.text = "请输入用户组ID，然后开启摄像头进行签到";
    }

    public void BeginDetect()
    {
        if (groupIdText.text != "")
        {
            InvokeRepeating("StartFrameShoot", 1.0f, 1.0f);
        }
        else
        {
            infoText.text = "请输入用户组ID！然后开启摄像头。";
        }
    }

    //单人识别
    public void UserIdentify()
    {
        string groupId = groupIdText.text;
        string path = Application.dataPath + "/ScreenShoots/Frame/" + index + ".jpg";
        byte[] image = File.ReadAllBytes(path);
        JObject result = client.Identify(groupId, image);

        if (result["result"] == null)
        {
            error_code = int.Parse(result["error_code"].ToString());      //先把json数据转成字符串,再转成int类型
            error_msg = result["error_msg"].ToString();      //把返回的json错误信息转成字符串
            infoText.text = error_msg;
            switch (error_code)
            {
                case 216100:
                    infoText.text = "invalid param 参数异常,请重新填写注册信息";
                    break;
                case 216101:
                    infoText.text = "not enough param 缺少必须的注册信息,请重新填写注册信息";
                    break;
                case 216401:
                    infoText.text = "internal error 内部错误";
                    break;
                case 216402:
                    infoText.text = "face not found 未找到人脸，请检查图片是否含有人脸";
                    break;
                case 216500:
                    infoText.text = "unknown error 未知错误";
                    break;
                case 216615:
                    infoText.text = "fail to process images 服务处理该图片失败，发生后重试即可";
                    break;
                default:
                    infoText.text = error_msg;
                    break;
            }
        }
        else
        {
            JToken users = result["result"];
            string userId = users[0]["uid"].ToString();
            infoText.text = userId + " 签到成功。";
        }
    }

    //多人识别
    public void UserMultiIdentify()
    {
        string groupId = groupIdText.text;
        string path = Application.dataPath + "/ScreenShoots/Frame/" + index + ".jpg";
        byte[] image = File.ReadAllBytes(path);
        JObject result = client.MultiIdentify(groupId, image, options);

        if (result["result"] == null)
        {
            error_code = int.Parse(result["error_code"].ToString());      //先把json数据转成字符串,再转成int类型
            error_msg = result["error_msg"].ToString();      //把返回的json错误信息转成字符串
            infoText.text = error_msg;
            switch (error_code)
            {
                case 216100:
                    infoText.text = "invalid param 参数异常,请重新填写注册信息";
                    break;
                case 216101:
                    infoText.text = "not enough param 缺少必须的注册信息,请重新填写注册信息";
                    break;
                case 216401:
                    infoText.text = "internal error 内部错误";
                    break;
                case 216402:
                    infoText.text = "face not found 未找到人脸，请检查图片是否含有人脸";
                    break;
                case 216500:
                    infoText.text = "unknown error 未知错误";
                    break;
                case 216615:
                    infoText.text = "fail to process images 服务处理该图片失败，发生后重试即可";
                    break;
                default:
                    break;
            }
        }
        else
        {
            JToken users = result["result"];
            switch (result["result_num"].ToString())
            {
                case "0":
                    infoText.text = "";
                    break;
                case "1":
                    infoText.text = users[0]["uid"].ToString() + " 签到成功。\n";
                    break;
                case "2":
                    infoText.text = users[0]["uid"].ToString() + " 签到成功。\n" + 
                                          users[1]["uid"].ToString() + " 签到成功。\n";
                    break;
                case "3":
                    infoText.text = users[0]["uid"].ToString() + " 签到成功。\n"+
                                          users[1]["uid"].ToString() + " 签到成功。\n"+
                                          users[2]["uid"].ToString() + " 签到成功。\n";
                    break;
                case "4":
                    infoText.text = users[0]["uid"].ToString() + " 签到成功。\n"+
                                          users[1]["uid"].ToString() + " 签到成功。\n"+
                                          users[2]["uid"].ToString() + " 签到成功。\n"+
                                          users[3]["uid"].ToString() + " 签到成功。\n";
                    break;
                case "5":
                    infoText.text = users[0]["uid"].ToString() + " 签到成功。\n" +
                                          users[1]["uid"].ToString() + " 签到成功。\n" +
                                          users[2]["uid"].ToString() + " 签到成功。\n" +
                                          users[3]["uid"].ToString() + " 签到成功。\n" +
                                          users[4]["uid"].ToString() + " 签到成功。\n";
                    break;
                default:
                    break;
            }

            for (int i = 0; i < int.Parse(result["result_num"].ToString()); i++)
            {
                sql = new SQLiteHelper("data source=" + Application.streamingAssetsPath + "/" + groupId);
                sql.UpdateValues("table1", new string[] { "Success" }, new string[] { "'true'" }, "Info", "=", "'" + users[i]["user_info"] + "'");
            }
        }
    }

    void StartFrameShoot()
    {
        StartCoroutine("FrameDetect");
    }

    public void StopFrameShoot()
    {
        CancelInvoke();
    }

    IEnumerator FrameDetect()
    {
        yield return new WaitForEndOfFrame();      //等某一帧结束

        string[] files = Directory.GetFiles(Application.dataPath + "/ScreenShoots/Frame/");
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains("meta"))
            {
                //UserIdentify();
                UserMultiIdentify();
                index++;
                if (index > 10 || index > i)
                {
                    index = 0;
                }
            }
        }
    }
}
