using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Baidu.Aip.Face;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json.Linq;

public class UserManager : MonoBehaviour {

    private Face client;
    public string API_KEY, SECRET_KEY;

    public Text userIdText;
    public Text userInfoText;
    public Text groupIdText;

    public Text userDebugText;

    //private int error_code = 0;      //百度API返回的错误代码编号  
    private string error_msg;      //错误描述信息，帮助理解和解决发生的错误。  

    private int flag = 0;

    private SQLiteHelper sql;

    private void Awake()
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                System.Security.Cryptography.X509Certificates.X509Chain chain,
                System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;           // always accept
            };

        client = new Face(API_KEY, SECRET_KEY)
        {
            Timeout = 8000
        };
    }

    public void UserAdd()
    {
        string userId = userIdText.text;
        string userInfo = userInfoText.text;
        string groupId = groupIdText.text;
        if (userInfo == "" || groupId == "")
        {
            userDebugText.text = "请完整填写信息！";
            return;
        }
        string path = Application.dataPath + "/ScreenShoots/" + groupId + "/" + userInfo + ".jpg";
        if (File.Exists(path))
        {
            byte[] image = File.ReadAllBytes(path);
            JObject result = client.UserAdd(userId, userInfo, groupId, image);
            flag = 1;
            ErrorInfo(result);
        }
        else
        {
            userDebugText.text = "照片库里没有这张照片，请确认或重新截图！";
        }
        
        if (!File.Exists(Application.streamingAssetsPath + "/" + groupId))
        {
            sql = new SQLiteHelper("data source=" + Application.streamingAssetsPath + "/" + groupId);
            sql.CreateTable("table1", new string[] { "ID", "Info", "GroupID", "Success" }, new string[] { "TEXT", "TEXT", "TEXT", "BOOLEAN" });
            sql.InsertValues("table1", new string[] { "'" + userId + "'", "'" + userInfo + "'", "'" + groupId + "'", "'" + false + "'" });
            sql.CloseConnection();
        }
        else
        {
            sql = new SQLiteHelper("data source=" + Application.streamingAssetsPath + "/" + groupId);
            sql.InsertValues("table1", new string[] { "'" + userId + "'", "'" + userInfo + "'", "'" + groupId + "'", "'" + false + "'" });
            sql.CloseConnection();
        }
    }

    public void UserGet()
    {
        string userId = userIdText.text;
        JObject result = client.UserGet(userId);
        flag = 2;
        ErrorInfo(result);
    }

    public void GroupDelete()
    {
        string userId = userIdText.text;
        string userInfo = userInfoText.text;
        string groupId = groupIdText.text;
        JObject result = client.GroupDeleteuser(groupId, userId);
        flag = 3;
        ErrorInfo(result);
    }

    public void ErrorInfo(JObject result)
    {
        if (result["error_msg"] != null)
        {
            error_msg = result["error_msg"].ToString();      //把返回的json错误信息转成字符串
            userDebugText.text = error_msg;
        }
        else
        {
            switch (flag)
            {
                case 1:
                    userDebugText.text = "面部注册成功！";
                    break;
                case 2:
                    JToken users = result["result"];
                    userDebugText.text = "ID：" + users[0]["uid"].ToString() + "\n" +
                                                    "UserInfo：" + users[0]["user_info"].ToString() + "\n" +
                                                    "Group：" + users[0]["group_id"].ToString();
                    break;
                case 3:
                    userDebugText.text = "面部删除成功！";
                    break;
                default:
                    break;
            }
        }
    }
}
