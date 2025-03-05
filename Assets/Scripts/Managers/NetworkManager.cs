using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager>
{
    public IEnumerator Signup(SignupData signupData, Action successCallback, Action failureCallback)
    {
        string jsonString = JsonUtility.ToJson(signupData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "users/signup", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                failureCallback?.Invoke();
            }
            else
            {
                var result = www.downloadHandler.text;
                successCallback?.Invoke();
            }
        }
    }
    
    public IEnumerator Signin(SigninData signinData, Action successCallback, Action failureCallback)
    {
        string jsonString = JsonUtility.ToJson(signinData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "users/signin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                failureCallback?.Invoke();
            }
            else
            {
                var result = JsonUtility.FromJson<SigninResult>(www.downloadHandler.text);
                
                if (result.result == 2)
                {
                    successCallback?.Invoke();
                }
                else
                {
                    failureCallback?.Invoke();
                }
            }
        }
    }
}
