using System;
using System.Collections.Generic;
using Ink;
using Ink.Runtime;
using UnityEngine;
public class LnkStory
{
    private readonly Story m_story;

    #region 封装的字段或属性

    /// <summary>
    /// 是否可以继续
    /// </summary>
    public bool CanContinue => m_story.canContinue;

    /// <summary>
    /// 当前位置对应的标签列表
    /// </summary>
    public List<string> CurrentTags => m_story.currentTags;

    /// <summary>
    /// 全局标签
    /// </summary>
    public List<string> GlobalTags => m_story.globalTags;

    /// <summary>
    /// 当前对应的选择列表信息
    /// </summary>
    public List<Choice> CurrentChoices => m_story.currentChoices;

    /// <summary>
    /// 获取或者设置变量的媒介
    /// </summary>
    private VariablesState VariablesState => m_story.variablesState;

   

    /// <summary>
    /// 获取设置墨水变量的索引器
    /// </summary>
    /// <param name="argName"></param>
    /// <returns></returns>
    public object this[string argName]
    {
        get => VariablesState[argName];
        set => VariablesState[argName] = value;
    }

    #endregion

    #region 构造函数

    /// <summary>
    /// 建议使用通过转化为Json的ink文本初始化
    /// </summary>
    /// <param name="textAsset"></param>
    public LnkStory(TextAsset textAsset)
    {
        m_story = new Story(textAsset.text);
        SetErrorInfo();
    }

    public LnkStory(string text)
    {
        m_story = new Story(text);
        SetErrorInfo();
    }

    /// <summary>
    /// 设置错误信息
    /// </summary>
    private void SetErrorInfo()
    {
        if (m_story == null) return;
        m_story.onError += (msg, type) =>
        {
            if (type == ErrorType.Warning)
                Debug.LogWarning(msg);
            else
                Debug.LogError(msg);
        };
    }

    #endregion

    #region 封装的常用方法

    /// <summary>
    /// 强迫故事结束
    /// </summary>
    public void ForceEnd()
    {
        m_story.state.ForceEnd();
    }

    /// <summary>
    /// 继续故事
    /// </summary>
    public string Continue()
        => m_story.Continue();

    /// <summary>
    /// 设置故事对应的选择索引
    /// </summary>
    /// <param name="choiceIndex">Choice对象的索引</param>
    public void ChooseChoiceIndex(int choiceIndex)
        => m_story.ChooseChoiceIndex(choiceIndex);

    /// <summary>
    /// 得到当前节点的标签列表
    /// </summary>
    /// <param name="knotPath">标签路径</param>
    /// <returns></returns>
    public List<string> TagsForContentAtPath(string knotPath)
        => m_story.TagsForContentAtPath(knotPath);

    /// <summary>
    /// 将当前故事转化为Json信息
    /// </summary>
    /// <returns></returns>
    public string ToJson()
        => m_story.state.ToJson();

    /// <summary>
    /// 从Json字符串中读取当前故事
    /// </summary>
    /// <param name="jsonData"></param>
    public void LoadJson(string jsonData)
        => m_story.state.LoadJson(jsonData);

    /// <summary>
    /// 跳转到特定节点或者节点内子节点
    /// </summary>
    /// <param name="knotPath"></param>
    public void ChoosePathString(string knotPath)
        => m_story.ChoosePathString(knotPath);

    /// <summary>
    /// 得到节点或者子节点的访问次数
    /// </summary>
    /// <param name="knotPath"></param>
    /// <returns></returns>
    public int VisitCountAtPathString(string knotPath)
        => m_story.state.VisitCountAtPathString(knotPath);

    /// <summary>
    /// 监听某个变量的变化
    /// </summary>
    /// <param name="variableName">变量名</param>
    /// <param name="observer">监听函数</param>
    public void ObserveVariable(string variableName, Action<string, object> observer)
        => m_story.ObserveVariable(variableName, observer.Invoke);

    /// <summary>
    /// 监听多个变量的比那话
    /// </summary>
    /// <param name="variableNames">变量名列表</param>
    /// <param name="observer">监听函数</param>
    public void ObserveVariables(IList<string> variableNames, Action<string, object> observer)
        => m_story.ObserveVariables(variableNames, observer.Invoke);

    /// <summary>
    /// 绑定外部函数
    /// </summary>
    /// <param name="funcName">ink中对应的函数名</param>
    /// <param name="func">绑定的委托</param>
    /// <param name="lookaheadSafe">是否提前调用</param>
    public void BindExternalFunction(string funcName, Func<object> func, bool lookaheadSafe = false)
        => m_story.BindExternalFunction(funcName, func, lookaheadSafe);
    public void BindExternalFunction<T>(string funcName, Action<T> func, bool lookaheadSafe = false)
        => m_story.BindExternalFunction(funcName, func, lookaheadSafe);

    /// <summary>
    ///绑定额外的Debug函数功能
    /// </summary>
    /// <param name="errorHandler"></param>
    public void AddDebugMsgFunc(Action<string, ErrorType> errorHandler) => m_story.onError += (msg, type)
        => errorHandler?.Invoke(msg, type);

    public void ResetState() => m_story.ResetState();
    

    #endregion

 
}