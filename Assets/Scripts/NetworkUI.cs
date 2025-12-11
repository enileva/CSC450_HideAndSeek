using UnityEngine;
using TMPro;
using PurrNet;
using PurrNet.Transports;

public class NetworkUI : MonoBehaviour
{
    public TMP_InputField ipInput;
    public GameObject uiRoot; // canvas root
    public GameObject hudCanvas;

    void Awake()
    {
        if (!uiRoot)
            uiRoot = gameObject;

        if (hudCanvas)
            hudCanvas.SetActive(false); // off at startup
    }

    public void OnHostPressed()
    {
        Debug.Log("Host pressed");
        NetworkManager.main.StartServer();
        NetworkManager.main.StartClient();

        Debug.Log("Server started");
        if (uiRoot)
            uiRoot.SetActive(false);
        if (hudCanvas) hudCanvas.SetActive(true);
    }

    public void OnConnectPressed()
    {
        Debug.Log("Connect pressed");
        string ip = ipInput.text.Trim();
        if (string.IsNullOrEmpty(ip))
            ip = "127.0.0.1";

        Debug.Log("Using IP " + ip);

        // Set the client address on the UDP transport
        UDPTransport transport = (UDPTransport)NetworkManager.main.transport;
        transport.address = ip;

        NetworkManager.main.StartClient();
        if (uiRoot) uiRoot.SetActive(false);
        if (hudCanvas) hudCanvas.SetActive(true);
    }
}
