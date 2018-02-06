using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    private List<string> list = new List<string>();
    public Text ReadText;
    public SubtitleTest Subtitle;

    public Text Score_Lable;
    public Text Rebound_Lable;
    public Text Assisting_Lable;

    //public Text Height_Lable;
    //public Text Weight_Lable;
    //public Text Age_Lable;

    public Text Score;
    public Text Rebound;
    public Text Assisting;
    public Text Name;
    public Image Photo;
    public Image Team;

    private string ScoreText;
    private string ReboundText;
    private string AssistingText;
    private string NameText;
    private string PhotoUrl;
    private string TeamUrl;

    //public Text Height;
    //public Text Weight;
    //public Text Age;

    public GameObject UI;
    Transform tra;

    private Vector3 UIEndPosition;
    private Vector3 UIStartPosition;

    private Vector3 UIStartScale;
    private Vector3 UIEndScale;

    private Quaternion UIStartRota;

    // Use this for initialization
    void Start()
    {

    }

    public void Init()
    {
        UI = GameObject.Find("Canvas_Info/Image_Info");

        UIStartPosition = GameObject.Find("Sofa/Scenario/TV/Canvas_TV/Image_TV").transform.position;
        UIEndPosition = UI.transform.position;

        UIStartScale = Vector3.zero;
        UIEndScale = UI.transform.localScale;

        UIStartRota = UI.transform.rotation;

        tra = UI.transform;
        foreach (Transform trans in tra)
        {
            trans.gameObject.SetActive(false);
        }

        UI.transform.localScale = UIStartScale;
        UI.transform.position = UIStartPosition;

        TextAsset textAsset = Resources.Load("1") as TextAsset;
        if (null != textAsset && "" != textAsset.text)
        {
            string[] lines = textAsset.text.Split("\n"[0]);
            for (int i = 0; i < lines.Length; i++)
            {
                if ("" == lines[i]) { break; }
                list.Add(lines[i]);
            }
        }
        StartCoroutine("ReadTxt");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ReadTxt()
    {
        yield return new WaitForSeconds(6);
        for (int i = 0; i < list.Count; i++)
        {
            Subtitle.SetData(list[i]);
            yield return new WaitForSeconds(4);
        }
        ReadText.text = null;
    }

    /// <summary>
    /// 设置内容，此处为解析字符串，后续会改为解析Json
    /// </summary>
    /// <param name="data"></param>
    public void SetData(string data)
    {
        if (null == data || !data.Contains("Data"))
        {
            return;
        }

        if (data.Contains("得分"))
        {
            ScoreText = data.Substring(data.IndexOf("分") + 1, 4);
        }
        if (data.Contains("篮板"))
        {
            ReboundText = data.Substring(data.IndexOf("板") + 1, 3);
        }
        if (data.Contains("助攻"))
        {
            AssistingText = data.Substring(data.IndexOf("攻") + 1, 3);
        }
        if (data.Contains("Name"))
        {
            NameText = data.Substring(data.IndexOf("Name:") + 5, data.IndexOf("Team") - 5 - data.IndexOf("Name:") - 1);
        }
        if (data.Contains("Team"))
        {
            TeamUrl = data.Substring(data.IndexOf("Team:") + 5, data.IndexOf("Pic") - 5 - data.IndexOf("Team:") - 1);
        }
        if (data.Contains("Pic"))
        {
            PhotoUrl = data.Substring(data.IndexOf("Pic:") + 4, data.IndexOf(")") - 4 - data.IndexOf("Pic:"));
        }
        LoadData();
    }

    public void LoadData()
    {
        StartCoroutine("LoadDataIEnumerator");
    }

    public IEnumerator LoadDataIEnumerator()
    {
        float temp = 0;
        float timeLong = 0;
        while (true)
        {
            if (temp > 1)
            {
                UI.transform.rotation = UIStartRota;
                break;
            }
            else
            {
                temp += Time.deltaTime;
                UI.transform.Rotate(Vector3.forward, 360.0f / (1 / 0.02f));
                UI.transform.position = Vector3.Lerp(UIStartPosition, UIEndPosition, temp);
                UI.transform.localScale = Vector3.Lerp(UIStartScale, UIEndScale, temp);
            }
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1);

        Name.text = null;
        Score.text = null;
        Rebound.text = null;
        Assisting.text = null;
        Photo.sprite = null;
        Team.sprite = null;

        AsyncImageDownload.Instance.SetAsyncImage(PhotoUrl, Photo);
        AsyncImageDownload.Instance.SetAsyncImage(TeamUrl, Team);

        foreach (Transform trans in tra)
        {
            trans.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        Name.GetComponent<SubtitleTest>().SetData(NameText);
        yield return new WaitForSeconds(1);

        Score_Lable.GetComponent<SubtitleTest>().SetData("得分");
        yield return new WaitForSeconds(1);

        Score.GetComponent<SubtitleTest>().SetData(ScoreText);
        yield return new WaitForSeconds(1);

        Rebound_Lable.GetComponent<SubtitleTest>().SetData("篮板");
        yield return new WaitForSeconds(1);

        Rebound.GetComponent<SubtitleTest>().SetData(ReboundText);
        yield return new WaitForSeconds(1);

        Assisting_Lable.GetComponent<SubtitleTest>().SetData("助攻");
        yield return new WaitForSeconds(1);

        Assisting.GetComponent<SubtitleTest>().SetData(AssistingText);

        while (true)
        {
            if (timeLong > 10)
            {
                timeLong = 0;
                break;
            }
            else
            {
                timeLong += Time.deltaTime;
                yield return new WaitForSeconds(0.02f);
            }
        }
        foreach (Transform trans in tra)
        {
            trans.gameObject.SetActive(false);
        }

        UI.transform.localScale = UIStartScale;
        UI.transform.position = UIStartPosition;
        yield return 0;
    }
}
