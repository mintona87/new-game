using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;

public class LoginPanelController : MonoBehaviour
{
    public Button loginButton;
    public Button exitButton;
    public GameObject loginPanel;
    public TMP_InputField usernameInput; 
    public TMP_InputField passwordInput; 
    public Button loginSubmitButton;
    public Button createAccountButton;


    void Start()
    {
            Debug.Log("LoginPanelController started");
   //     loginButton.onClick.AddListener(OpenLoginPanel);
        exitButton.onClick.AddListener(Exit);
        loginSubmitButton.onClick.AddListener(Login);
        createAccountButton.onClick.AddListener(CreateAccount);
        loginPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button clicked");
        }
    }

    public void OpenLoginPanel()
    {
        Debug.Log("Opening Login Panel");
        loginPanel.SetActive(true);
        loginButton.gameObject.SetActive(false); // Hide the Login Button
        exitButton.gameObject.SetActive(false); // Hide the Exit Button
    }

    public void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        // Replace this with your actual login implementation
        Debug.Log($"Login attempt with username: {username} and password: {password}");
    }

    public void CreateAccount()
    {
        // Replace this with your actual create account implementation
        Debug.Log("Create Account button clicked");
    }

    public void Exit()
    {
        loginButton.gameObject.SetActive(false); // Hide the Login Button
        exitButton.gameObject.SetActive(false); // Hide the Exit Button
        Application.Quit();
    }
}
