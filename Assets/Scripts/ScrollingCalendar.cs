using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ScrollingCalendar : MonoBehaviour
{
	private void InitializeYears()
	{
		int[] array = new int[int.Parse(DateTime.Now.ToString("yyyy")) + 1 - 1900];
		this.yearsButtons = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 1900 + i;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.yearsButtonPrefab, new Vector3(0f, (float)(i * 80), 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
			gameObject.transform.SetParent(this.yearsScrollingPanel, false);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.GetComponentInChildren<Text>().text = string.Concat(array[i]);
			gameObject.name = "Year_" + array[i];
			gameObject.AddComponent<CanvasGroup>();
			this.yearsButtons[i] = gameObject;
		}
	}

	private void InitializeMonths()
	{
		int[] array = new int[12];
		this.monthsButtons = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = "";
			array[i] = i;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.monthsButtonPrefab, new Vector3(0f, (float)(i * 80), 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
			gameObject.transform.SetParent(this.monthsScrollingPanel, false);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			switch (i)
			{
			case 0:
				text = "Jan";
				break;
			case 1:
				text = "Feb";
				break;
			case 2:
				text = "Mar";
				break;
			case 3:
				text = "Apr";
				break;
			case 4:
				text = "May";
				break;
			case 5:
				text = "Jun";
				break;
			case 6:
				text = "Jul";
				break;
			case 7:
				text = "Aug";
				break;
			case 8:
				text = "Sep";
				break;
			case 9:
				text = "Oct";
				break;
			case 10:
				text = "Nov";
				break;
			case 11:
				text = "Dec";
				break;
			}
			gameObject.GetComponentInChildren<Text>().text = text;
			gameObject.name = "Month_" + array[i];
			gameObject.AddComponent<CanvasGroup>();
			this.monthsButtons[i] = gameObject;
		}
	}

	private void InitializeDays()
	{
		int[] array = new int[31];
		this.daysButtons = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = i + 1;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.daysButtonPrefab, new Vector3(0f, (float)(i * 80), 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
			gameObject.transform.SetParent(this.daysScrollingPanel, false);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.GetComponentInChildren<Text>().text = string.Concat(array[i]);
			gameObject.name = "Day_" + array[i];
			gameObject.AddComponent<CanvasGroup>();
			this.daysButtons[i] = gameObject;
		}
	}

	public void Awake()
	{
		this.InitializeYears();
		this.InitializeMonths();
		this.InitializeDays();
		this.monthsVerticalScroller = new UIVerticalScroller(this.monthsScrollingPanel, this.monthsButtons, this.monthCenter);
		this.yearsVerticalScroller = new UIVerticalScroller(this.yearsScrollingPanel, this.yearsButtons, this.yearsCenter);
		this.daysVerticalScroller = new UIVerticalScroller(this.daysScrollingPanel, this.daysButtons, this.daysCenter);
		this.monthsVerticalScroller.Start();
		this.yearsVerticalScroller.Start();
		this.daysVerticalScroller.Start();
	}

	public void SetDate()
	{
		this.daysSet = int.Parse(this.inputFieldDays.text) - 1;
		this.monthsSet = int.Parse(this.inputFieldMonths.text) - 1;
		this.yearsSet = int.Parse(this.inputFieldYears.text) - 1900;
		this.daysVerticalScroller.SnapToElement(this.daysSet);
		this.monthsVerticalScroller.SnapToElement(this.monthsSet);
		this.yearsVerticalScroller.SnapToElement(this.yearsSet);
	}

	private void Update()
	{
		this.monthsVerticalScroller.Update();
		this.yearsVerticalScroller.Update();
		this.daysVerticalScroller.Update();
		string text = this.daysVerticalScroller.GetResults();
		string results = this.monthsVerticalScroller.GetResults();
		string results2 = this.yearsVerticalScroller.GetResults();
		if (text.EndsWith("1") && text != "11")
		{
			text += "st";
		}
		else if (text.EndsWith("2") && text != "12")
		{
			text += "nd";
		}
		else if (text.EndsWith("3") && text != "13")
		{
			text += "rd";
		}
		else
		{
			text += "th";
		}
		this.dateText.text = string.Concat(new string[]
		{
			results,
			" ",
			text,
			" ",
			results2
		});
	}

	public void DaysScrollUp()
	{
		this.daysVerticalScroller.ScrollUp();
	}

	public void DaysScrollDown()
	{
		this.daysVerticalScroller.ScrollDown();
	}

	public void MonthsScrollUp()
	{
		this.monthsVerticalScroller.ScrollUp();
	}

	public void MonthsScrollDown()
	{
		this.monthsVerticalScroller.ScrollDown();
	}

	public void YearsScrollUp()
	{
		this.yearsVerticalScroller.ScrollUp();
	}

	public void YearsScrollDown()
	{
		this.yearsVerticalScroller.ScrollDown();
	}

	public RectTransform monthsScrollingPanel;

	public RectTransform yearsScrollingPanel;

	public RectTransform daysScrollingPanel;

	public GameObject yearsButtonPrefab;

	public GameObject monthsButtonPrefab;

	public GameObject daysButtonPrefab;

	private GameObject[] monthsButtons;

	private GameObject[] yearsButtons;

	private GameObject[] daysButtons;

	public RectTransform monthCenter;

	public RectTransform yearsCenter;

	public RectTransform daysCenter;

	private UIVerticalScroller yearsVerticalScroller;

	private UIVerticalScroller monthsVerticalScroller;

	private UIVerticalScroller daysVerticalScroller;

	public InputField inputFieldDays;

	public InputField inputFieldMonths;

	public InputField inputFieldYears;

	public Text dateText;

	private int daysSet;

	private int monthsSet;

	private int yearsSet;
}
