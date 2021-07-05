using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Analytics;
using System.Linq;
using System;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private DependencyStatus dependencyStatus;
    public bool firebaseInitialized;

    [SerializeField] private List<Action> _sendQueue = new List<Action>();

    void Awake()
    {
        if (Instance == null) Instance = this;

        InitFirebaseAnal();
    }

    private IEnumerator SendQueue()
    {
        yield return new WaitUntil(() => _sendQueue.Count() != 0);
        yield return new WaitUntil(() => firebaseInitialized == true);

        _sendQueue[0]?.Invoke();
        _sendQueue.RemoveAt(0);

        StartCoroutine(SendQueue());
    }

    private void InitFirebaseAnal()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        //Debug.Log("Set user properties.");
        //// Set the user's sign up method.
        //FirebaseAnalytics.SetUserProperty(
        //  FirebaseAnalytics.UserPropertySignUpMethod,
        //  "Google");
        //// Set the user ID.
        //FirebaseAnalytics.SetUserId("uber_user_510");
        //// Set default session duration values.
        //FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        firebaseInitialized = true;
    }

    private void SendFirebaseLogEvent(string eventStatus, Dictionary<string, object> dictionary)
    {
        if (firebaseInitialized == true)
        {
            if (dictionary != null)
            {
                //List<KeyValuePair<string, string>>

                Parameter[] parameter = dictionary.Select(e => new Parameter(e.Key, e.Value.ToString())).ToArray();

                FirebaseAnalytics
                .LogEvent(eventStatus, parameter);


                Debug.Log("Firebase " + eventStatus);
            }
            else
            {
                FirebaseAnalytics
                    .LogEvent(eventStatus);


                Debug.Log("Firebase " + eventStatus);
            }
        }
    }

    private void SendFacebookLogEvent(string eventStatus, Dictionary<string, object> dictionary)
    {
        //if (dictionary != null)
        //{
        //    FB.LogAppEvent(eventStatus, null, dictionary);

        //    Debug.Log("FB " + eventStatus);
        //}
        //else
        //{
        //    FB.LogAppEvent(eventStatus);

        //    Debug.Log("FB " + eventStatus);
        //}
    }

    private void SendBrunchLogEvent(string eventStatus, Dictionary<string, object> dictionary)
    {
        //if (dictionary != null)
        //{
        //    //BranchEvent e = new BranchEvent(eventStatus);

        //    var t = new BranchEvent(eventStatus);
        //    foreach(string name in dictionary.Keys)
        //    { 
        //        t.AddCustomData(name, dictionary[name].ToString());
        //    }

        //    Branch.sendEvent(t);

        //    Debug.Log("Branch " + eventStatus);
        //}
        //else
        //{
        //    var t = new BranchEvent(eventStatus);

        //    Branch.sendEvent(t);

        //    Debug.Log("Branch " + eventStatus);
        //}
    }

    public void SendLogEventToAll(string eventName, Dictionary<string, object> parameters)
    {
        SendFirebaseLogEvent(eventName, parameters);
        SendFacebookLogEvent(eventName, parameters);
        SendBrunchLogEvent(eventName, parameters);
    }

    public void SendProductField(float costProduct)
    {
        SendFirebaseLogEvent("app_window_shop_window_action", new Dictionary<string, object> {
            {"action", $"cost_product_{costProduct}"} });

        SendFacebookLogEvent("app_window_shop_window_action", new Dictionary<string, object> {
            {"action", $"cost_product_{costProduct}"} });

        SendBrunchLogEvent("app_window_shop_window_action", new Dictionary<string, object> {
            {"action", $"cost_product_{costProduct}"} });
    }

    public void SendThemesThingItem(string nameItem)
    {
        SendFirebaseLogEvent("app_window_shop_theme_action", new Dictionary<string, object> {
            {"action", $"theme_{nameItem}"} });

        SendFacebookLogEvent("app_window_shop_theme_action", new Dictionary<string, object> {
            {"action", $"theme_{nameItem}"} });

        SendBrunchLogEvent("app_window_shop_theme_action", new Dictionary<string, object> {
            {"action", $"theme_{nameItem}"} });
    }

    public void SendOpenShopThemesWindow()
    {
        SendFirebaseLogEvent("app_window_shop_themes_open", null);

        SendFacebookLogEvent("app_window_shop_themes_open", null);

        SendBrunchLogEvent("app_window_shop_themes_open", null);
    }

    public void SendCloseShopThemesWindow()
    {
        SendFirebaseLogEvent("app_window_shop_themes_close", null);

        SendFacebookLogEvent("app_window_shop_themes_close", null);

        SendBrunchLogEvent("app_window_shop_themes_close", null);
    }

    public void SendOpenShopWindow()
    {
        SendFirebaseLogEvent("app_window_shop_window_open", null);

        SendFacebookLogEvent("app_window_shop_window_open", null);

        SendBrunchLogEvent("app_window_shop_window_open", null);

    }

    public void SendCloseShopWindow()
    {
        SendFirebaseLogEvent("app_window_shop_window_close", null);

        SendFacebookLogEvent("app_window_shop_window_close", null);

        SendBrunchLogEvent("app_window_shop_window_close", null);
    }

    public void SendCloseButtonShopWindow()
    {
        SendFirebaseLogEvent("app_window_shop_window_action", new Dictionary<string, object> {
            {"close", $"button_close"} });

        SendFacebookLogEvent("app_window_shop_window_action", new Dictionary<string, object> {
            {"close", $"button_close"} });

        SendBrunchLogEvent("app_window_shop_window_action", new Dictionary<string, object> {
            {"close", $"button_close"} });
    }

    public void SendCloseButtonShopThemesWindow()
    {
        SendFirebaseLogEvent("app_window_shop_themes_action", new Dictionary<string, object> {
            {"close", $"button_close"} });

        SendFacebookLogEvent("app_window_shop_themes_action", new Dictionary<string, object> {
            {"close", $"button_close"} });

        SendBrunchLogEvent("app_window_shop_themes_action", new Dictionary<string, object> {
            {"close", $"button_close"} });
    }

    public void SendAppOpen()
    {
        _sendQueue.Add(() =>
        {
            SendFirebaseLogEvent("app_window_open", null);

            SendFacebookLogEvent("app_window_open", null);

            SendBrunchLogEvent("app_window_open", null);
        });
    }

    public void SendRestartGame()
    {
        _sendQueue.Add(() =>
        {
            SendFirebaseLogEvent("app_window_action", new Dictionary<string, object> {
            {"action", $"restart_game"} });

            SendFacebookLogEvent("app_window_action", new Dictionary<string, object> {
            {"action", $"restart_game"} });

            SendBrunchLogEvent("app_window_action", new Dictionary<string, object> {
            {"action", $"restart_game"} });
        });
    }
}
