using UnityEngine;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    [Range(0, 1)]
    public float cubemapTransition = 0f; // Initial cubemap transition value
    public float transitionSpeed = 0.1f; // Speed of transition per interval

    private float elapsedTime = 0f;
    public float interval = 5f; // 60 seconds = 1 minute

    public Light directionalLight; // Assign your directional light in the Inspector
    public TextMeshProUGUI timeDisplay; // Assign a UI Text element for time display

    private int currentHour = 6; // Start at 6 AM (06:00)

    void Update()
    {
        // Accumulate time
        elapsedTime += Time.deltaTime;

        // Check if interval has passed
        if (elapsedTime >= interval)
        {
            // Update the hour by one every interval
            currentHour = (currentHour + 1) % 24;

            // Calculate cubemap transition based on the hour
            CalculateCubemapTransition();

            // Reset the elapsed time
            elapsedTime = 0f;
        }

        // Smoothly interpolate the cubemap transition
        RenderSettings.skybox.SetFloat("_CubemapTransition", cubemapTransition);

        // Adjust lighting based on cubemapTransition value
        AdjustLighting();

        // Update time display based on the current hour
        UpdateTimeDisplay();
    }

    void CalculateCubemapTransition()
    {
        if (currentHour >= 18 && currentHour < 21) // 6 PM - 9 PM: Evening, getting darker
        {
            cubemapTransition = Mathf.Lerp(0f, 1f, (currentHour - 18f) / 3f);
        }
        else if (currentHour >= 21 || currentHour < 3) // 9 PM - 3 AM: Night, fully dark
        {
            cubemapTransition = 1f;
        }
        else if (currentHour >= 3 && currentHour < 6) // 3 AM - 6 AM: Morning, getting lighter
        {
            cubemapTransition = Mathf.Lerp(1f, 0f, (currentHour - 3f) / 3f);
        }
        else // 6 AM - 6 PM: Daytime, fully light
        {
            cubemapTransition = 0f;
        }
    }

    void AdjustLighting()
    {
        if (directionalLight != null)
        {
            // Change light intensity from day to night
            directionalLight.intensity = Mathf.Lerp(1f, 0.1f, cubemapTransition);

            // Change light color from warm (day) to cool (night)
            directionalLight.color = Color.Lerp(Color.white, Color.blue, cubemapTransition);
        }
    }

    void UpdateTimeDisplay()
    {
        // Determine AM or PM
        string period = currentHour >= 12 && currentHour < 24 ? "PM" : "AM";

        // Adjust hours for 12-hour format
        int displayHour = currentHour > 12 ? currentHour - 12 : currentHour;
        displayHour = displayHour == 0 ? 12 : displayHour;

        // Format time as "HH:00 AM/PM"
        string timeString = string.Format("{0:00}:00 {1}", displayHour, period);

        // Display time on UI Text element
        if (timeDisplay != null)
        {
            timeDisplay.text = timeString;
        }
    }
}
