using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class TokenResponse
{
    public string accessToken;
    public string refreshToken;
}
public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text errorText;
    public TMP_Dropdown environmentDropdown;

    private ApiConfig config;
    private string selectedEnvironment;

    void Start()
    {
        config = Resources.Load<ApiConfig>("Configs/ApiConfig");

        List<string> options = config.GetEnvironmentNames();
        environmentDropdown.ClearOptions();
        environmentDropdown.AddOptions(options);

        // Set the default selection
        int defaultIndex = options.IndexOf(config.defaultEnvironment);
        if (defaultIndex >= 0)
            environmentDropdown.value = defaultIndex;

        selectedEnvironment = options[environmentDropdown.value];

        // Listen to selection changes
        environmentDropdown.onValueChanged.AddListener(delegate { OnEnvironmentChanged(); });
    }

    void OnEnvironmentChanged()
    {
        selectedEnvironment = config.GetEnvironmentNames()[environmentDropdown.value];
        Debug.Log("Selected environment: " + selectedEnvironment);
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string accessToken;
        public string refreshToken;
    }

    public void OnLoginButtonClicked()
    {
        var loginRequest = new LoginRequest
        {
            username = usernameField.text,
            password = passwordField.text
        };

        string json = JsonUtility.ToJson(loginRequest);

        StartCoroutine(ApiClient.PostJson(
            "account/login",
            json,
            onSuccess: response =>
            {
                Debug.Log("Login successful: " + response);
                // TODO: Parse token and proceed
            },
            onError: error =>
            {
                Debug.LogError("Login failed: " + error);
                errorText.text = "Invalid username or password.";
            },
            selectedEnvironment
        ));
    }

}
public class TabNavigationFix : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
            if (current != null)
                EventSystem.current.SetSelectedGameObject(current.FindSelectableOnRight()?.gameObject);
        }
    }
}
