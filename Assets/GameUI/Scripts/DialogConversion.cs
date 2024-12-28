using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
public class DialogData
{
    public string name;
    public string content;
    public Dictionary<string, Action> choices;
}

public class DialogConversion : MonoBehaviour
{
    public enum E_DialogState
    {
        None,
        Running,
        End,
    }

    [SerializeField] private TextAsset m_textAsset;
    private LnkStory m_story;

    public LnkStory Story
    {
        get
        {
            if (m_story == null)
            {
                if (m_textAsset == null)
                {
                    return null;
                }

                m_story = new LnkStory(m_textAsset);
            }

            return m_story;
        }
    }

    public object this[string argName]
    {
        get => Story[argName];
        set => Story[argName] = value;
    }

    public List<string> CurTags
        => Story.CurrentTags;

    public void SetTextAsset(TextAsset settingTextAsset)
        => this.m_textAsset = settingTextAsset;

    public E_DialogState CurDialogState { get; set; }

    public void Init()
    {
        CurDialogState = E_DialogState.None;
    }
    public  DialogData ContinueStory()
    {
        CurDialogState = E_DialogState.Running;
        while (Story.CanContinue)
        {
            string content = Story.Continue();
            content = content.Trim();
            if (!string.IsNullOrEmpty(content))
            {
                DialogData dialogData = new DialogData();
                string[] result = ResolveStoryContent(content);
                dialogData.name = result[0];
                dialogData.content = result[1];
                if (HasChoices())
                {
                    dialogData.choices = new Dictionary<string, Action>();
                    foreach (Choice choice in Story.CurrentChoices)
                    {
                        dialogData.choices.Add(choice.text.Trim(), () => OnMakeChoice(choice));
                    }
                }
                return dialogData;
            }
            return ContinueStory();
        }

        return null;
    }

    /// <summary>
    /// 解析故事情节
    /// </summary>
    /// <param name="_text"></param>
    private string[] ResolveStoryContent(string _text)
    {
        string[] strArray = _text.Split(':', '：');
        if (strArray.Length == 1)
        {
            return new[] { "", _text };
        }
        return strArray;
    }

    /// <summary>
    /// 是否有选项
    /// </summary>
    /// <returns></returns>
    private bool HasChoices()
        => Story.CurrentChoices.Count > 0;

    public void ResetStory()
    {
        CurDialogState = E_DialogState.End;
        Story.ResetState();
    }
    public void ForceEnd()
    {
        CurDialogState = E_DialogState.End;
        Story.ForceEnd();
    }
    //当做出选择之后
    private void OnMakeChoice(Choice choice)
    {
        Story.ChooseChoiceIndex(choice.index);
    }
    /// <summary>
    /// 监听变量变化
    /// </summary>
    /// <param name="_varName"></param>
    /// <param name="_callback"></param>
    public void ObserveVariable(string _varName,Action<string,object> _callback)
    {
        Story.ObserveVariable(_varName, _callback);
    }
    /// <summary>
    /// 绑定外部函数
    /// </summary>
    /// <param name="_funcName"></param>
    /// <param name="_callback"></param>
    public void BindExternalFunction<T>(string _funcName, Action<T> _callback)
    {
        Story.BindExternalFunction(_funcName, _callback);
    }
}