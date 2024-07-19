using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using echo17.EndlessBook;
using echo17.EndlessBook.Demo02;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour {
    public static event Action<Story> OnCreateStory;
	
    void Awake () {
		// Remove the default message
		CreateNewPage();
		StartStory();
	}

	// Creates a new Story object with the compiled story which we can then play!
	void StartStory () {
		story = new Story (inkJSONAsset.text);
        if(OnCreateStory != null) OnCreateStory(story);
        AudioManager.Instance.PlaySoundInList(AudioClipName.WriteStorySound);
        RefreshView();
	}
	
	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView () {
		// Remove all the UI on screen
		//RemoveChildren ();
		
		// Read all the content until we can't continue any more
		while (story.canContinue) {
			// Continue gets the next line of the story
			string text = story.Continue ();
			// This removes any white space from the text.
			text = text.Trim();
        	List<string> tags = story.currentTags;
			foreach (var tag in tags)
        	{
				if(tag.StartsWith("image:"))
				{
					string imageName = tag.Substring(6);
					imageName = imageName.Trim();
                    Debug.Log(tag);
                    DisplayImage(imageName);
				}
				if (tag.StartsWith("time"))
                {
                    Debug.Log(tag);
                    ParseTimeTag(tag);
                }
        	}
			// Display the text on screen!
			CreateContentView(text);
		}

		// Display all the choices, if there are any!
		if(story.currentChoices.Count > 0) {
			for (int i = 0; i < story.currentChoices.Count; i++) {
				Choice choice = story.currentChoices [i];
				Button button = CreateChoiceView (choice.text.Trim ());
				// Tell the button what to do when we press it
				button.onClick.AddListener (delegate {
					OnClickChoiceButton (choice);
					button.enabled = false;
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else {
			Button choice = CreateChoiceView("End of story.\nRestart?");
			choice.onClick.AddListener(delegate {
                StartStory();
			});
		}
	}

    void ParseTimeTag(string tag)
    {
        // 提取时间字符串
        string timeString = tag.Substring(5); // 去掉"time:"前缀

        // 使用正则表达式解析时间字符串
        Regex regex = new Regex(@"(\d+)d(\d+)h(\d+)m(\d+)s");
        Match match = regex.Match(timeString);

        if (match.Success)
        {
            int days = int.Parse(match.Groups[1].Value);
            int hours = int.Parse(match.Groups[2].Value);
            int minutes = int.Parse(match.Groups[3].Value);
            int seconds = int.Parse(match.Groups[4].Value);

            // 处理解析后的时间
			TimeMove(days, hours, minutes, seconds);
			TimeReturnToInk();

            // 在这里你可以更新你的游戏时间变量或执行其他操作
        }
        else
        {
            Debug.LogWarning("Time tag format is incorrect");
        }
    }
	void TimeMove(int days, int hous,int min, int sec) { 
		EventHandler.time += new GameTime(days, hous, min, sec);
    }

    void DisplayImage(string imageName)
    {
        Sprite sprite = Resources.Load<Sprite>("Images/" + imageName); // 假设图片存储在Resources/Images文件夹
        if (sprite != null)
        {
            Image imageInstance = Instantiate(imagePrefab, canvasList[canvasNum].transform);
            imageInstance.sprite = sprite;
			imageInstance.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
			currentHeight += imageInstance.rectTransform.sizeDelta.y/2 + 25;//25是Padding的值
            imageInstance.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Image not found: " + imageName);
        }
    }

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
		Debug.Log("OnClickChoiceButton");
		AudioManager.Instance.PlaySoundInList(AudioClipName.WriteStorySound);
		RemoveAllButton();
        RefreshView();
	}

	void TimeReturnToInk() {

		Debug.Log("Ink day should be " + EventHandler.time.Day);
        SetInkVariable("day", EventHandler.time.Day);
        SetInkVariable("hour", EventHandler.time.Hour);
        SetInkVariable("minutes", EventHandler.time.Minute);
        SetInkVariable("day", (int)EventHandler.time.Second);
    }
    void SetInkVariable(string variableName, int value)
    {
        if (story.variablesState != null)
        {
            story.variablesState[variableName] = value;
        }
    }

    // Creates a textbox showing the the line of text
    void CreateContentView (string text) {
		Text storyText = Instantiate (textPrefab) as Text;
		canvasRectTransform = canvasList[canvasNum].GetComponent<RectTransform>();
		storyText.text = text;
		storyText.transform.SetParent (canvasList[canvasNum].transform, false);
		storyText.alignment = TextAnchor.MiddleLeft;
        LayoutRebuilder.ForceRebuildLayoutImmediate(storyText.rectTransform);
		currentHeight += storyText.rectTransform.sizeDelta.y/2;
		//Debug.Log("currentHeight is " + currentHeight);
		if(currentHeight > canvasRectTransform.rect.height-150) {
			canvasNum++;
			CreateNewPage();
		}
	}
	void CreateNewPage(){
			GameObject newPage= Instantiate(pagePrefab);
			Camera RenderCamera = newPage.GetComponentInChildren<Camera>();
			RenderTexture renderTexture = new RenderTexture(2048, 2048, 24);
        	renderTexture.antiAliasing = 8; // 设置抗锯齿
			RenderCamera.targetTexture = renderTexture;
			RenderCamera.targetTexture.filterMode = FilterMode.Point;
			RenderCamera.targetTexture.name = "PageView_0" + (canvasNum+2);
			Material PageMaterial = new Material(BaseMaterial);
			PageMaterial.mainTexture = RenderCamera.targetTexture;
			newPage.transform.SetParent(pageParent.transform,true);
			canvasList.Add(newPage.GetComponentInChildren<Canvas>());
			pageController.pageViews.Add(newPage.GetComponent<PageView>());
			Book.CreatePage();
			Book.AddPageData(PageMaterial);
			if(canvasNum+2<10){
				newPage.gameObject.name = "PageView_0" + (canvasNum+2);
			}else{
				newPage.gameObject.name = "PageView_" + (canvasNum+2);
			}
			RemoveChildren(canvasNum);
			newPage.transform.localPosition = new Vector3(-150,canvasNum * -40,0);
			currentHeight = 0;
	}

	// Creates a button showing the choice text
	Button CreateChoiceView (string text) {
		// Creates the button from a prefab
		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (canvasList[canvasNum].transform, false);
		
		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text> ();
		choiceText.text = text;

		// Add a BoxCollider2D or BoxCollider to the button and set it as a trigger
		BoxCollider collider = choice.gameObject.AddComponent<BoxCollider>();
		collider.size = new Vector3 (choice.gameObject.GetComponentInChildren<RectTransform>().sizeDelta.x, 
		choice.gameObject.GetComponentInChildren<RectTransform>().sizeDelta.y ,
		0.01f);
		collider.isTrigger = true;

		// Make the button expand to fit the text
		HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
		layoutGroup.childForceExpandHeight = false;

		return choice;
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren (int PageNum) {
		if(canvasList.Count>0){
			int childCount = canvasList[PageNum].transform.childCount;
			for (int i = childCount - 1; i >= 0; --i) {
				Destroy (canvasList[PageNum].transform.GetChild (i).gameObject);
			}
		}
	}
	void RemoveAllButton() {
		foreach (Canvas canvas in canvasList) {
            Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
			foreach (Button button in buttons) {
				Destroy(button.gameObject);
			}
        }
	}

	[SerializeField]
	private TextAsset inkJSONAsset = null;
	public Story story;

	[SerializeField]
	private List<Canvas> canvasList = new List<Canvas>();
	private RectTransform canvasRectTransform = null;
	private float currentHeight = 0;
	private int canvasNum = 0;
	private float canvasGroupHeight = 0;

	// UI Prefabs
	[SerializeField]
	private EndlessBook Book = null;
	[SerializeField]
	private Text textPrefab = null;
	[SerializeField]
	private Image imagePrefab = null;
	[SerializeField]
	private GameObject pagePrefab = null;
	[SerializeField]
	private Material BaseMaterial = null;
	[SerializeField]
	private GameObject pageParent = null;
	[SerializeField]
	private Demo02 pageController = null;
	[SerializeField]
	private Button buttonPrefab = null;
}
