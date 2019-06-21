using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraFrame : MonoBehaviour {

    [Tooltip("返回的照片数据,根据返回的照片,进行照片的识别和比对认证等等!"), Space(5)]
    [Header("摄像头拍摄的照片"),]
    public WebCamTexture camTexture;      //摄像头拍下的图片数据

    [Header("摄像头设备名")]
    private WebCamDevice[] devices;

    [Tooltip("USB摄像头设备"), Space(5)]
    [Header("摄像头设备名")]
    public string deviceName;      //摄像头设备名称

    public bool isClick;      //是否点击了按钮

    private Texture2D tex2D;      //截取的图片
    public RawImage rawImage;      //显示摄像头画面
    public Image image;      //截图区域
    
    public Text groupIdText;
    
    private int index = 0;

    //初始化摄像头显示的图像的大小
    private void Awake()
    {
        camTexture = new WebCamTexture(deviceName, 800, 600, 60);
    }

    private void Start()
    {
        isClick = false;
    }

    //通过GUI绘制摄像头要显示的窗口
    private void Update()
    {
        //首先根据摄像头展示的画面来判断摄像头是否存在
        if (isClick == true && camTexture != null)
        {
            rawImage.texture = camTexture;
        }
        if (isClick == false && camTexture != null)      //不显示画面
        {
            rawImage.texture = null;
        }
    }

    //打开摄像机的方法 挂到button按钮上
    public void OpenWebCamDevice()
    {
        if (groupIdText.text != "")
        {
            isClick = true;
            if (isClick == true)
            {
                //用户授权打开摄像头  
                if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                {
                    devices = WebCamTexture.devices;      //显示画面的设备就是要打开的摄像头
                    deviceName = devices[0].name;      //获取到设备名称
                    camTexture.Play();      //开启摄像头
                }
                InvokeRepeating("StartScreenShoot", 1.0f, 3.0f);
            }
        }
    }

    //关闭摄像头 挂到button按钮上
    public void CloseWebCamDevice()
    {
        if (isClick == true && camTexture != null)
        {
            isClick = false;
            camTexture.Stop();      //关闭摄像头
        }
    }

    //截图按钮事件
    public void ClickShootButton()
    {
        OpenWebCamDevice();      //先打开摄像头
        Invoke("StartScreenShoot", 1.0f);      //几秒后截图
    }

    void StartScreenShoot()
    {
        camTexture.Pause();      //截图暂停一下
        StartCoroutine("ScreenShoot");      //启动协程截图
    }

    IEnumerator ScreenShoot()
    {
        yield return new WaitForEndOfFrame();      //等某一帧结束

        int width = (int)rawImage.rectTransform.rect.width;
        int height = (int)rawImage.rectTransform.rect.height;
        int x = Mathf.Abs((int)image.gameObject.transform.position.x);
        int y = Mathf.Abs((int)image.gameObject.transform.position.y);

        tex2D = new Texture2D(width, height, TextureFormat.RGB24, true);//截图的大小
        tex2D.ReadPixels(new Rect(x, y, width, height), 0, 0, false);      //截取的区域
        tex2D.Apply();
        byte[] by = tex2D.EncodeToJPG();      //将截取到的图片转换成字节数据
        camTexture.Play();      //摄像头继续开启
        if (!File.Exists(Application.dataPath + "/ScreenShoots/Frame/"))
        {
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/ScreenShoots/");
            dir.CreateSubdirectory("Frame");
        }
        File.WriteAllBytes(Application.dataPath + "/ScreenShoots/Frame/" + index + ".jpg", by);
        index++;
        if (index > 2)
        {
            index = 0;
        }
    }

    public void ToAdmin()
    {
        SceneManager.LoadScene("01_Admin");
    }
}
