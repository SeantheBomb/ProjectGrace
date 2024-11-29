using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RedditFetcher : MonoBehaviour
{
    public string subreddit = "Catmemes";
    public RawImage postImage; // Assign in the Unity Editor
    public TMP_Text postTitle;     // Assign in the Unity Editor


    private const string ProxyBaseUrl = "https://cors-anywhere.herokuapp.com/";
    private const string RedditUrlTemplate =/* ProxyBaseUrl +*/ "https://www.reddit.com/r/{0}/top/.json?sort=top&t=day&limit=1";


    private void Start()
    {
        ShowTopPost();
    }

    public void ShowTopPost()
    {
        postImage.gameObject.SetActive(false);
        postTitle.gameObject.SetActive(false);
        StartCoroutine(FetchTopPost());
    }

    IEnumerator FetchTopPost()
    {
        string url = string.Format(RedditUrlTemplate, subreddit);
        string imageUrl = "";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching subreddit data: {request.error}");
                yield return new WaitForSeconds(3f);
                yield return FetchTopPost();
                yield break;
            }

            try
            {
                // Parse JSON response
                RedditResponse redditResponse = JsonUtility.FromJson<RedditResponse>(request.downloadHandler.text);
                if (redditResponse != null && redditResponse.data != null && redditResponse.data.children.Length > 0)
                {
                    var topPost = redditResponse.data.children[0].data;

                    string title = topPost.title;
                    imageUrl = topPost.url;

                    // Update UI
                    postTitle.text = title;
                    postTitle.gameObject.SetActive(true);

                    // Fetch and display the image
                }
                else
                {
                    Debug.LogError("Invalid Reddit response or no posts found.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing JSON: {ex.Message}");
                yield break;
            }
            yield return StartCoroutine(LoadImage(imageUrl));

        }
    }

    IEnumerator LoadImage(string imageUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(ProxyBaseUrl + imageUrl))
        {

            // Add required headers for the proxy
            request.SetRequestHeader("User-Agent", "UnityRedditClient/1.0");
            request.SetRequestHeader("origin", "https://yourgamehost.com"); // Replace with your hosting domain or any dummy value
            request.SetRequestHeader("x-requested-with", "XMLHttpRequest");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error loading image: {request.error}");
                yield break;
            }

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            postImage.texture = texture;
            postImage.SetNativeSize(); // Adjust size to fit the image dimensions
            postImage.gameObject.SetActive(true);
        }
    }
}

[System.Serializable]
public class RedditResponse
{
    public RedditData data;
}

[System.Serializable]
public class RedditData
{
    public RedditChild[] children;
}

[System.Serializable]
public class RedditChild
{
    public RedditPost data;
}

[System.Serializable]
public class RedditPost
{
    public string title;
    public string url;
}
