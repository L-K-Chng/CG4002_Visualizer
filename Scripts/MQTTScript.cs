using System;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using PimDeWitte.UnityMainThreadDispatcher;


public class MQTTScript : MonoBehaviour
{
    private MqttClient mqttClient;
    public bool isConnected = false;

    private string MQTT_BROKER = "broker.emqx.io";
    private int MQTT_PORT = 1883;
    private string MQTT_USERNAME = "";
    private string MQTT_PASSWORD = "";
    private string MQTT_TOPIC = "M2MQTT_Unity/test";

    // Bring in classes which we need to be used.
    public GameManager gameManager;
    
    // private string MQTT_TOPIC = "test";
    //mosquitto_sub -h p4361666.ala.asia-southeast1.emqxsl.com -p 8883 -t player1 -u user -P capstone 

    void Start()
    {
        mqttClient = new MqttClient(MQTT_BROKER, MQTT_PORT, false, null, null, MqttSslProtocols.None);
        string clientId = Guid.NewGuid().ToString();

        try
        {
            mqttClient.Connect(clientId, MQTT_USERNAME, MQTT_PASSWORD);
            isConnected = mqttClient.IsConnected;
            if (isConnected)
            {
                Debug.Log("MQTT Client connected successfully to broker: " + MQTT_BROKER + " on port: " + MQTT_PORT);

                // Register the message received event 
                mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

                // Subscribe to the desired topic with QoS level 2 
                mqttClient.Subscribe(new string[] { MQTT_TOPIC }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            else
            {
                Debug.LogError("MQTT connection failed!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("MQTT connection error: " + ex.Message);
        }
        // NotifyGameStart();
    }

    public void PublishMessage(string topic, string message)
    {
        if (isConnected)
        {
            mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Published message: " + message + " to topic: " + topic);
        }
        else
        {
            Debug.LogWarning("Cannot publish - MQTT is not connected.");
        }
    }

    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string receivedMessage = Encoding.UTF8.GetString(e.Message);
        Debug.Log("Message received from topic: " + e.Topic + " - " + receivedMessage);

        Dictionary<string, object> messageData = JsonConvert.DeserializeObject<Dictionary<string, object>>(receivedMessage);
        Debug.Log("Message data: " + messageData);

        UnityMainThreadDispatcher.Instance().Enqueue(() => {
        Debug.Log("Executing on main thread");

            try
            {
                // Perform action based on "action" 
                if (messageData.ContainsKey("action"))
                {
                    string action = messageData["action"].ToString();
                    switch (action)
                    {
                        case "basket":
                            gameManager.PlayerDoBasketball();
                            break;
                        case "volley":
                            gameManager.PlayerDoVolleyball();
                            break;
                        case "bowl":
                            gameManager.PlayerDoBowling();
                            break;
                        case "soccer":
                            gameManager.PlayerDoSoccer();
                            break;
                        case "bomb":
                            gameManager.PlayerDoRainBomb();
                            break;
                        case "gun":
                            gameManager.PlayerDoFireBullets();
                            break;
                        case "reload":
                            gameManager.PlayerDoReload();
                            break;
                        case "shield":
                            gameManager.PlayerDoShield();
                            break;
                        case "none":
                            break;
                        default:
                            Debug.Log("Unknown action received: " + action);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error processing message on main thread: " + ex.Message);
            }
        });
    }

    void OnDestroy()
    {
        if (mqttClient != null && mqttClient.IsConnected)
        {
            mqttClient.Disconnect();
        }
    }
    /*
if (mqttScript != null && mqttScript.isConnected) 
        { 
            mqttScript.PublishMessage(MQTT_TOPIC, "Game has started"); 
        }
    */
    // Call this method when the game starts or when you want to publish a message
    void NotifyGameStart()
    {
        if (isConnected)
        {
            PublishMessage(MQTT_TOPIC, "Game has started");
        }
        else
        {
            Debug.LogWarning("MQTT is not connected, cannot publish 'Game has started'.");
        }
    }
}


