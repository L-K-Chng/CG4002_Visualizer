using System;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using PimDeWitte.UnityMainThreadDispatcher;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Contracts;
using System.Collections;
using Unity.VisualScripting;


public class MQTTScript : MonoBehaviour
{
    private MqttClient mqttClient;
    public bool isConnected = false;

    // updating MQTT broker details
    private string MQTT_BROKER = "172.26.191.157";
    private int MQTT_PORT = 1883;
    private string MQTT_USERNAME = "";
    private string MQTT_PASSWORD = "";
    private string MQTT_TOPIC_CAN_SEE = "unity/oppseen";
    private string MQTT_TOPIC_ACTION = "payload/unity";
    private string MQTT_TOPIC_GAME_STATE = "gamestate/unity";
    private string MQTT_TOPIC_OPP_POSITION = "unity/opppos";

    // Bring in classes which we need to be used.
    public GameManager gameManager;
    public PlayerScript playerScript;
    public EnemyUIScript enemyUIScript;
    public ShieldScript shieldScript;
    // public RainEffectDamageScript rainEffectDamageScript;

    // Create a list of rain effect damage scripts to loop through the rain effects.
    public List<RainEffectDamageScript> rainEffectDamageScripts;
    //public RainEffectDamageScript rainEffectDamageScript;

    private int previousInRainCount = 0; // Tracks the previous total in rain count 
    public int currentInRainCount = 0;

    // newest implementation for syncing collision with game state change, as well as displaying the kill and death message.
    private string latestAction;
    int stored_player_id = LoginStateScript.stored_player_id;

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

                // NEW
                mqttClient.Subscribe(new string[] { MQTT_TOPIC_ACTION, MQTT_TOPIC_GAME_STATE, MQTT_TOPIC_OPP_POSITION },
                    new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                // 25 Oct update: edited new visibility and rain messages for Start() function.
                SendRainMessage(stored_player_id, 0);
                Debug.Log("Initial rain Message Sent!");
                SendVisibilityMessage(stored_player_id, false);
                Debug.Log("Initial visibility message sent!");
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
    }

    // Update method for actively checking and publishing in_rain upon change in this count
    void Update()
    {
        // RainEffectDamage Script updates this in rain count value. the moment the in rain count value changes, it updates and sends to mqtt. 
        if (currentInRainCount != previousInRainCount)
        {
            // sends the rain message once the number of rains changes.
            SendRainMessage(stored_player_id, currentInRainCount);

            // Update the previous count to avoid re-publishing.
            previousInRainCount = currentInRainCount;
        }
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

    // new mqtt publish received for action and game_state topics.
    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string receivedMessage = Encoding.UTF8.GetString(e.Message);
        Debug.Log("Message received from topic: " + e.Topic + " - " + receivedMessage);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Debug.Log("Executing on main thread");
            try
            {
                // Check which topic the message is from
                if (e.Topic == MQTT_TOPIC_GAME_STATE)
                {
                    // Receive game state update after a delay of 2 seconds.
                    // find a way to handle for player 2.
                    float actionDelayTime = latestAction != null ? CalculateDelayTime(latestAction) : 0f;
                    StartCoroutine(DelayGameStateUpdate(stored_player_id, actionDelayTime, receivedMessage));
                }
                else if (e.Topic == MQTT_TOPIC_ACTION)
                {
                    // Handle action messages
                    // HandleActionMessage(stored_player_id, receivedMessage);
                    HandleActionMessage(stored_player_id, receivedMessage);
                }

                // new implementation! 2 november 2024
                else if (e.Topic == MQTT_TOPIC_OPP_POSITION)
                {
                    HandleRainMessage(receivedMessage);
                }
                // else case for when message comes from unknown topic.
                else
                {
                    Debug.LogWarning("Message received from an unknown topic: " + e.Topic);
                }
            }
            catch (JsonReaderException jsonEx)
            {
                Debug.LogWarning("Received a non-JSON message: " + receivedMessage);
                Debug.LogWarning("JSON Error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error processing message on main thread: " + ex.Message);
            }
        });
    }

    // for handling in_rain message
    private void HandleRainMessage(string message)
    {
        try
        {
            Dictionary<string, object> rainData = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

            if (rainData.ContainsKey("player_id") && rainData.ContainsKey("in_rain"))
            {
                int player_id = Convert.ToInt32(rainData["player_id"]);
                int in_rain = Convert.ToInt32(rainData["in_rain"]);

                if (stored_player_id != player_id)
                {
                    if (in_rain > 0)
                    {
                        playerScript.DisplayInRainMessage(in_rain);
                    }
                    else
                    {
                        playerScript.DeactivateInRainMessage();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error processing rain message: " + ex.Message);
        }
    }

    // New HandleActionMessage function
    // NEW EDITS START HERE
    // Testing new handleactionmessage function.
    private void HandleActionMessage(int player_id_stored, string receivedMessage)
    {
        try
        {
            Dictionary<string, object> actionData = JsonConvert.DeserializeObject<Dictionary<string, object>>(receivedMessage);

            // Original
            if (actionData.ContainsKey("player_id") && actionData.ContainsKey("action") && actionData.ContainsKey("can_see") && actionData.ContainsKey("redo") && actionData.ContainsKey("game_state"))
            {
                string action = actionData["action"].ToString();
                int player_id = Convert.ToInt32(actionData["player_id"]);
                bool redo_flag = Convert.ToBoolean(actionData["redo"]);
                // test this.
                string gamestate = actionData["game_state"].ToString();
                Debug.Log("Game State is: " + gamestate);

                // Handle actions for both player 1 and 2 (Player's actions)
                if (player_id == player_id_stored)
                {
                    // new part here!
                    if (!redo_flag)
                    {
                        // to sync with duration of action
                        latestAction = action;
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
                                gameManager.PlayerDoRainBomb(stored_player_id);
                                break;
                            case "gun":
                                gameManager.PlayerDoFireBullets(stored_player_id);
                                break;
                            case "reload":
                                gameManager.PlayerDoReload(stored_player_id);
                                break;
                            case "shield":
                                gameManager.PlayerDoShield(stored_player_id);
                                break;
                            case "logout":
                                gameManager.PlayerDoLogOut();
                                break;
                            case "none":
                                break;
                            default:
                                Debug.Log("Unknown action received: " + action);
                                break;
                        }
                    }
                    else
                    {
                        // if redo is true, then display the message to redo the action
                        StartCoroutine(playerScript.DisplayMessage(playerScript.redoActionErrorMessage));
                        return;
                    }
                }

                // Case 1: If I can see the enemy and the enemy can see me, the enemy will launch a projectile towards me and this projectile explodes upon collision
                // Case 2: If I cannot see the enemy but the enemy can see me, I will not see the projectile, but will take damage after 1.5 seconds. Play the sound afterwards as well.
                // Case 3: If the enemy does an action but they cannot see me, I will not see any projectile, neither will I take any damage. 
                // Handle actions for player_id == 2 (Enemy's actions)
                if (player_id != player_id_stored)
                {
                    if (!redo_flag)
                    {
                        // the can_see value will be that of the enemy
                        bool enemy_can_see_value = Convert.ToBoolean(actionData["can_see"]);
                        bool player_can_see_value = enemyUIScript.GetEnemyVisibility();

                        if (enemy_can_see_value && player_can_see_value)
                        {
                            switch (action)
                            {
                                case "basket":
                                    gameManager.OnEnemyBasketballButtonPressed();
                                    break;
                                case "volley":
                                    gameManager.OnEnemyVolleyballButtonPressed();
                                    break;
                                case "soccer":
                                    gameManager.OnEnemySoccerButtonPressed();
                                    break;
                                case "bowl":
                                    gameManager.OnEnemyBowlingButtonPressed();
                                    break;
                                case "gun":
                                    // gameManager.OnEnemyFireButtonPressed();
                                    gameManager.OnEnemyFireButtonPressed(stored_player_id);
                                    break;
                                case "bomb":
                                    // gameManager.OnEnemyRainBombButtonPressed();
                                    gameManager.OnEnemyRainBombButtonPressed(stored_player_id);
                                    break;
                                case "shield":
                                case "reload":
                                case "logout":
                                case "none":
                                    break;
                                default:
                                    Debug.Log("Unknown action received: " + action);
                                    break;
                            }
                        }
                        else if (enemy_can_see_value && !player_can_see_value)
                        {
                            switch (action)
                            {
                                case "basket":
                                    IEnumerator hitByBasketballMessage = playerScript.DisplayMessage(playerScript.hitByBasketballText);
                                    StartCoroutine(HandleInvisibleAIAction(hitByBasketballMessage));
                                    break;
                                case "volley":
                                    IEnumerator hitByVolleyballMessage = playerScript.DisplayMessage(playerScript.hitByVolleyballText);
                                    StartCoroutine(HandleInvisibleAIAction(hitByVolleyballMessage));
                                    break;
                                case "soccer":
                                    IEnumerator hitBySoccerBallMessage = playerScript.DisplayMessage(playerScript.hitBySoccerBallText);
                                    StartCoroutine(HandleInvisibleAIAction(hitBySoccerBallMessage));
                                    break;
                                case "bowl":
                                    IEnumerator hitByBowlingBallMessage = playerScript.DisplayMessage(playerScript.hitByBowlingBallText);
                                    StartCoroutine(HandleInvisibleAIAction(hitByBowlingBallMessage));
                                    break;
                                // we need to check the ammunition before deciding whether the player actually gets hit.
                                case "gun":
                                    IEnumerator hitByBulletMessage = playerScript.DisplayMessage(playerScript.hitByBulletText);
                                    StartCoroutine(HandleInvisibleGunAction(hitByBulletMessage));
                                    break;
                                case "bomb":
                                    IEnumerator hitByRainBombMessage = playerScript.DisplayMessage(playerScript.hitByRainBombText);
                                    StartCoroutine(HandleInvisibleBombAction(hitByRainBombMessage));
                                    break;
                                case "shield":
                                case "reload":
                                case "logout":
                                case "none":
                                    break;
                                default:
                                    Debug.Log("Unknown action received: " + action);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                // IMPT
                // Game State update by the game engine can be done here.
                // put in the game state update function here.
                HandleGameEngineGameStateUpdate(action, stored_player_id, gamestate);
            }
            else
            {
                Debug.LogError("Action message missing 'action', 'player_id', or 'can_see' fields.");
            }
        }
        catch (JsonReaderException jsonEx)
        {
            Debug.LogWarning("Received a non-JSON message: " + receivedMessage);
            Debug.LogWarning("JSON Error: " + jsonEx.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error processing action message: " + ex.Message);
        }
    }

    public void HandleGameEngineGameStateUpdate(string action, int player_id_stored, string receivedMessage)
    {
        Debug.Log("Carrying out HandleGameEngineGameStateUpdate function!");
        float delay_time = CalculateGameEngineDelayTime(action);
        StartCoroutine(DelayGameEngineGameStateUpdate(delay_time, player_id_stored, receivedMessage));
    }

    private float CalculateGameEngineDelayTime(string action)
    {
        switch (action)
        {
            case "basket":
            case "volley":
            case "soccer":
            case "bowl":
            case "gun":
            case "bomb":
                return 1.3f;
            case "logout":
            case "none":
            case "shield":
                return 0f;
            case "reload":
                return 2.7f;
            default:
                Debug.Log("Unknown action received: " + action);
                return 0f;
        }
    }

    private IEnumerator DelayGameEngineGameStateUpdate(float time, int player_id_stored, string receivedMessage)
    {
        yield return new WaitForSeconds(time);
        UpdateGameStateFromGameEngine(player_id_stored, receivedMessage);
    }

    public void UpdateGameStateFromGameEngine(int player_id_stored, string receivedMessage)
    {
        // player one score = player two deaths
        // player two score = player one deaths
        int existingPlayerTwoDeaths = playerScript.GetPlayerScore();
        int existingPlayerOneDeaths = enemyUIScript.GetEnemyScore();
        Debug.Log("Managed to enter the UpdateGameStateFromGameEngine function!");

        try
        {
            // Deserialize the JSON string directly into a Dictionary representing p1 and p2
            Dictionary<string, object> gameStateData = JsonConvert.DeserializeObject<Dictionary<string, object>>(receivedMessage);

            if (gameStateData.ContainsKey("p1") && gameStateData.ContainsKey("p2"))
            {
                if (gameStateData["p1"] is JObject p1Object && gameStateData["p2"] is JObject p2Object)
                {
                    var p1Data = p1Object.ToObject<Dictionary<string, object>>();
                    var p2Data = p2Object.ToObject<Dictionary<string, object>>();

                    // Extract data for p1
                    int playerOneHp = Convert.ToInt32(p1Data["hp"]);
                    int playerOneBullets = Convert.ToInt32(p1Data["bullets"]);
                    int playerOneRainBombCount = Convert.ToInt32(p1Data["bombs"]);
                    int playerOneShieldHp = Convert.ToInt32(p1Data["shield_hp"]);
                    int playerOneDeaths = Convert.ToInt32(p1Data["deaths"]);
                    int playerOneShieldCount = Convert.ToInt32(p1Data["shields"]);

                    // Update player 1 UI
                    playerScript.SetHp(playerOneHp);
                    playerScript.UpdateBulletCount(playerOneBullets);
                    playerScript.UpdateRainBombCount(playerOneRainBombCount);
                    playerScript.SetShieldHp(playerOneShieldHp);
                    enemyUIScript.UpdateEnemyScore(playerOneDeaths);
                    playerScript.SetPlayerShields(playerOneShieldCount);

                    // Extract data for p2
                    int playerTwoHp = Convert.ToInt32(p2Data["hp"]);
                    int playerTwoBullets = Convert.ToInt32(p2Data["bullets"]);
                    int playerTwoRainBombCount = Convert.ToInt32(p2Data["bombs"]);
                    int playerTwoShieldHp = Convert.ToInt32(p2Data["shield_hp"]);
                    int playerTwoDeaths = Convert.ToInt32(p2Data["deaths"]);
                    int playerTwoShieldCount = Convert.ToInt32(p2Data["shields"]);

                    // Update player 2 UI
                    enemyUIScript.SetEnemyHp(playerTwoHp);
                    enemyUIScript.UpdateBulletCount(playerTwoBullets);
                    enemyUIScript.UpdateRainBombCount(playerTwoRainBombCount);
                    enemyUIScript.SetEnemyShieldHp(playerTwoShieldHp);
                    playerScript.UpdatePlayerScore(playerTwoDeaths);
                    enemyUIScript.SetEnemyShields(playerTwoShieldCount);

                    // Shield and kill/death handling
                    gameManager.CheckShieldActive(player_id_stored);

                    if (player_id_stored == 1)
                    {
                        if (playerTwoDeaths > existingPlayerTwoDeaths)
                        {
                            StartCoroutine(playerScript.DisplayMessage(playerScript.killText));
                        }
                        else if (playerOneDeaths > existingPlayerOneDeaths)
                        {
                            StartCoroutine(playerScript.DisplayMessage(playerScript.deathText));
                        }
                    }
                    else
                    {
                        if (playerOneDeaths > existingPlayerOneDeaths)
                        {
                            StartCoroutine(playerScript.DisplayMessage(playerScript.killText));
                        }
                        else if (playerTwoDeaths > existingPlayerTwoDeaths)
                        {
                            StartCoroutine(playerScript.DisplayMessage(playerScript.deathText));
                        }
                    }
                }
                else
                {
                    Debug.LogError("p1 or p2 data is not in the expected format.");
                }
            }
        }
        catch (JsonReaderException jsonEx)
        {
            Debug.LogWarning("JSON Error: " + jsonEx.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error updating game state after delay: " + ex.Message);
        }
    }
    // new handleaction function ends here
    //

    // Functions to handle invisible actions. There is a delay of 1.5s as the projectile will hit the target in 1.5s each time.
    private IEnumerator HandleInvisibleAIAction(IEnumerator message)
    {
        yield return new WaitForSeconds(1.5f);
        if (shieldScript.IsPlayerShieldActive())
        {
            gameManager.PlayerReceiveDamageWithShield(message);
        }
        else
        {
            gameManager.PlayerReceiveDamage(message);
        }
    }

    private IEnumerator HandleInvisibleBombAction(IEnumerator message)
    {
        yield return new WaitForSeconds(1.5f);
        if (stored_player_id == 1 && enemyUIScript.GetEnemyRainBombCount() != 0)
        {
            if (shieldScript.IsPlayerShieldActive())
            {
                gameManager.PlayerReceiveDamageWithShield(message);
            }
            else
            {
                gameManager.PlayerReceiveDamage(message);
            }
        }
        if (stored_player_id != 1 && playerScript.GetPlayerRainBombCount() != 0)
        {
            if (shieldScript.IsPlayerShieldActive())
            {
                gameManager.PlayerReceiveDamageWithShield(message);
            }
            else
            {
                gameManager.PlayerReceiveDamage(message);
            }
        }
    }

    private IEnumerator HandleInvisibleGunAction(IEnumerator message)
    {
        yield return new WaitForSeconds(1.5f);
        if (stored_player_id == 1 && enemyUIScript.GetEnemyBulletCount() != 0)
        {
            if (shieldScript.IsPlayerShieldActive())
            {
                gameManager.PlayerReceiveDamageWithShield(message);
            }
            else
            {
                gameManager.PlayerReceiveDamage(message);
            }
        }
        // player id 2, player 1 bullet count not 0. then will have the bullet fired at him
        if (stored_player_id != 1 && playerScript.GetPlayerBulletCount() != 0)
        {
            if (shieldScript.IsPlayerShieldActive())
            {
                gameManager.PlayerReceiveDamageWithShield(message);
            }
            else
            {
                gameManager.PlayerReceiveDamage(message);
            }
        }
    }

    // calculates the delay time before the game state is updated.
    private float CalculateDelayTime(string action)
    {
        switch (action)
        {
            case "basket":
            case "volley":
            case "soccer":
            case "bowl":
            case "gun":
            case "bomb":
                return 2f;
            case "logout":
            case "none":
            case "shield":
                return 0f;
            case "reload":
                return 3f;
            default:
                Debug.Log("Unknown action received: " + action);
                return 0f;
        }
    }

    // delaying the game state update to sync with the attack actions.
    private IEnumerator DelayGameStateUpdate(int player_id_stored, float delay_time, string receivedMessage)
    {
        // player one score = player two deaths
        // player two score = player one deaths
        int existingPlayerTwoDeaths = playerScript.GetPlayerScore();
        int existingPlayerOneDeaths = enemyUIScript.GetEnemyScore();
        yield return new WaitForSeconds(delay_time); // Delay for 1.5 seconds

        try
        {
            Dictionary<string, object> receivedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(receivedMessage);

            if (receivedData.ContainsKey("game_state") && receivedData["game_state"] is JObject gameState)
            {
                Dictionary<string, object> gameStateData = gameState.ToObject<Dictionary<string, object>>();

                if (gameStateData.ContainsKey("p1") && gameStateData.ContainsKey("p2"))
                {
                    if (gameStateData["p1"] is JObject p1Object && gameStateData["p2"] is JObject p2Object)
                    {
                        var p1Data = p1Object.ToObject<Dictionary<string, object>>();
                        var p2Data = p2Object.ToObject<Dictionary<string, object>>();

                        // TESTING HERE
                        // EDITED ON 25 OCT
                        int playerOneHp = Convert.ToInt32(p1Data["hp"]);
                        int playerOneBullets = Convert.ToInt32(p1Data["bullets"]);
                        int playerOneRainBombCount = Convert.ToInt32(p1Data["bombs"]);
                        int playerOneShieldHp = Convert.ToInt32(p1Data["shield_hp"]);
                        int playerOneDeaths = Convert.ToInt32(p1Data["deaths"]);
                        int playerOneShieldCount = Convert.ToInt32(p1Data["shields"]);

                        // Update player 1 UI
                        playerScript.SetHp(playerOneHp);
                        playerScript.UpdateBulletCount(playerOneBullets);
                        playerScript.UpdateRainBombCount(playerOneRainBombCount);
                        playerScript.SetShieldHp(playerOneShieldHp);
                        enemyUIScript.UpdateEnemyScore(playerOneDeaths);
                        playerScript.SetPlayerShields(playerOneShieldCount);

                        int playerTwoHp = Convert.ToInt32(p2Data["hp"]);
                        int playerTwoBullets = Convert.ToInt32(p2Data["bullets"]);
                        int playerTwoRainBombCount = Convert.ToInt32(p2Data["bombs"]);
                        int playerTwoShieldHp = Convert.ToInt32(p2Data["shield_hp"]);
                        int playerTwoDeaths = Convert.ToInt32(p2Data["deaths"]);
                        int playerTwoShieldCount = Convert.ToInt32(p2Data["shields"]);

                        enemyUIScript.SetEnemyHp(playerTwoHp);
                        enemyUIScript.UpdateBulletCount(playerTwoBullets);
                        enemyUIScript.UpdateRainBombCount(playerTwoRainBombCount);
                        enemyUIScript.SetEnemyShieldHp(playerTwoShieldHp);
                        playerScript.UpdatePlayerScore(playerTwoDeaths);
                        enemyUIScript.SetEnemyShields(playerTwoShieldCount);

                        // IMPT: this shield function will do a check of the shield hp and
                        // activate the shield if the corresponding shield hp is not at 0
                        // similarly, if the shield hp drops to 0 then the shield will be removed.
                        gameManager.CheckShieldActive(player_id_stored);

                        // Does a check whether or not they killed the enemy or died.
                        // there is an existing kill and death counter. if the player dies, the message is flashed and the death count is updated
                        // same happens for the kill count.
                        if (player_id_stored == 1)
                        {
                            if (playerTwoDeaths > existingPlayerTwoDeaths)
                            {
                                StartCoroutine(playerScript.DisplayMessage(playerScript.killText));
                            }
                            else if (playerOneDeaths > existingPlayerOneDeaths)
                            {
                                StartCoroutine(playerScript.DisplayMessage(playerScript.deathText));
                            }
                        }
                        else
                        {
                            if (playerOneDeaths > existingPlayerOneDeaths)
                            {
                                StartCoroutine(playerScript.DisplayMessage(playerScript.killText));
                            }
                            else if (playerTwoDeaths > existingPlayerTwoDeaths)
                            {
                                StartCoroutine(playerScript.DisplayMessage(playerScript.deathText));
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("p1 or p2 data is not in the expected format.");
                    }
                }
            }
        }
        catch (JsonReaderException jsonEx)
        {
            Debug.LogWarning("JSON Error: " + jsonEx.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error updating game state after delay: " + ex.Message);
        }
    }

    void OnDestroy()
    {
        if (mqttClient != null && mqttClient.IsConnected)
        {
            mqttClient.Disconnect();
        }
    }

    // 25 oct changes 
    // adjust this to fit according to the stored_player_state
    // initial rain message sent for the bomb.
    void SendRainMessage(int player_id_stored, int in_rain_value)
    {
        var message = new
        {
            player_id = player_id_stored,
            in_rain = in_rain_value
        };
        string jsonMessage = JsonConvert.SerializeObject(message);
        PublishMessage(MQTT_TOPIC_OPP_POSITION, jsonMessage);
    }

    // to be used
    public void SendVisibilityMessage(int player_id_stored, bool visibility_value)
    {
        var message = new
        {
            player_id = player_id_stored,
            can_see = visibility_value
        };

        string jsonMessage = JsonConvert.SerializeObject(message);
        PublishMessage(MQTT_TOPIC_CAN_SEE, jsonMessage);
    }
}