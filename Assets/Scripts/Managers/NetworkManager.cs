using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;

public class NetworkManager : Singleton<NetworkManager>
{
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
    
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

            Debug.Log($"Response Code: {www.responseCode}");
            Debug.Log($"Response Body: {www.downloadHandler.text}");
            
            // 모든 응답 헤더 로깅
            var allHeaders = www.GetResponseHeaders();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                failureCallback?.Invoke();
            }
            else
            {
                string cookieHeader = null;
                foreach (var header in allHeaders)
                {
                    if (header.Key.ToLower() == "set-cookie")
                    {
                        cookieHeader = header.Value;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    string[] cookieParts = cookieHeader.Split(';');
                    string sessionCookie = cookieParts[0]; // 첫 번째 부분이 일반적으로 세션 ID
                    
                    PlayerPrefs.SetString("sid", sessionCookie);
                    PlayerPrefs.Save(); // 명시적 저장
                }
                else
                {
                    Debug.LogWarning("No cookie found in response headers.");
                }
                
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

                
    public IEnumerator GetScore(Action<UserInfo> successCallback, Action failureCallback)
    {
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "users/score", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            
            string sid = PlayerPrefs.GetString("sid");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }
            
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                if (www.responseCode == 403)
                {
                    //TODO: 로그인 
                }
                
                failureCallback?.Invoke();
            }
            else
            {
                var result = www.downloadHandler.text;
                var userScore = JsonUtility.FromJson<UserInfo>(result);
                successCallback?.Invoke(userScore);
            }
        }
    }

    public IEnumerator AddScore(ScoreData score, Action<bool> callback = null)
    {
        string jsonString = JsonUtility.ToJson(score);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
        
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "users/addscore", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            // 세션 쿠키 추가
            string sid = PlayerPrefs.GetString("sid");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }
            
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Add score error: " + www.error);
                Debug.Log("Response code: " + www.responseCode);
                callback?.Invoke(false);
            }
            else
            {
                Debug.Log("Success add score");
                callback?.Invoke(true);
            }
        }
    }

    public IEnumerator GetLeaderboard(Action<List<UserInfo>> callback = null)
    {
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "users/leaderboard", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                yield break;
            }
            
            var result = www.downloadHandler.text;
            Debug.Log("Leaderboard response: " + result);
        
            try {
                // JSON 배열을 객체로 감싸기
                string wrappedJson = "{\"leaderboard\":" + result + "}";
                var wrapper = JsonUtility.FromJson<LeaderboardResult>(wrappedJson);
                callback?.Invoke(wrapper.leaderboard);
            }
            catch (Exception e) {
                Debug.LogError("Error parsing leaderboard: " + e.Message);
                callback?.Invoke(null);
            }
        }
    }
}
