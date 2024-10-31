using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicText : MonoBehaviour
{

    private DynamicTextData data;
    private TextMeshProUGUI textObject;

    private bool initialised = false; // set to true after Initialise() is run
    private bool[] entered; // array to track progress of all entries
    private bool completeEntered = true; // above array is checked through every frame and if any of its values are false, this is set to false
    private bool exit = false; // used to track whether Exit() should be actively run
    private bool despawnStarted = false; // used to run the StartDespawn() coroutine once after initialisation


    // values used to change colour, size, position and exiting
    private float tColour = 0f;
    private float tSize = 0f;
    private float tPosition = 0f;
    private float tExit = 0f;

    // value used to calculate while lerping
    private Vector3 startPosition;
    private Vector3 startScale, startScaleZero;
    private Color startColour, startColourNoOpacity;
    // values used for bounce entries
    private float direction;

    // index values used in calculating colour changes and size changes
    private int totalColourIndex = 0;
    private int colourIndex = 0;
    private int nextColourIndex = 1;

    private int totalSizeIndex = 0;
    private int sizeIndex = 0;
    private int nextSizeIndex = 1;

    void Update()
    {

        // Update() functions only run if Initialise() has been run
        if (initialised)
        {
            // check through the entered array. if any false values are found, set this to false
            completeEntered = true;
            for (int i = 0; i < entered.Length; i++)
            {
                if (!entered[i]) completeEntered = false;
            }

            // if no false values are found, entry must be complete so main functionality can be run
            if (completeEntered)
            {
                // if despawn timer has not been started, start it and some other once-off functions
                if (!despawnStarted)
                {

                    StartCoroutine(StartDespawn()); // start despawn timer

                    // if immediate colour alternation mode instead of gradient, this is calculated by running a single coroutine rather than actively calculating
                    if (data.colourAlternationMode == AlternationMode.Immediate)
                    {
                        StartCoroutine(ColourSwitch());
                    }

                    // same for size alternation
                    if (data.sizeAlternationMode == AlternationMode.Immediate)
                    {
                        StartCoroutine(SizeSwitch());
                    }
                }

                // if these modes are gradients, however, they are actively calculated
                if (data.colourAlternationMode == AlternationMode.Gradient && totalColourIndex < data.numberOfColourAlternations)
                {
                    ColourGradient();
                }

                if (data.sizeAlternationMode == AlternationMode.Gradient && totalSizeIndex < data.numberOfSizeAlternations)
                {
                    SizeGradient();
                }

                // calculate bounce position after having entered
                for (int i = 0; i < data.enterType.Length; i++)
                {
                    if(data.enterType[i] == EnterType.Bounce)
                    {
                        tPosition += Time.deltaTime / data.enterDuration;
                        Vector3 targetPosition = startPosition - new Vector3(direction, data.maxHeight, direction);
                        transform.position = Vector3.Slerp(startPosition, targetPosition, tPosition);
                    }
                }

                // run Exit() while exiting
                if (exit)
                {
                    Exit();
                }
            }
            // run Enter() while entering
            else
            {
                Enter();
            }
        }

    }

    // function required to initialise the object, taking the desired text and date object as parameters
    public void Initialise(string _text, DynamicTextData _data)
    {
        // set the data object in this script to that which was passed
        data = _data;
        // change the placeholder text
        textObject = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textObject.text = _text;

        // set the font
        if (data.font != null) textObject.font = data.font;

        // set bold, italics, underline, strikethrough

        if (data.bold) textObject.fontStyle = FontStyles.Bold;
        if (data.italic) textObject.fontStyle = FontStyles.Italic;
        if (data.underline) textObject.fontStyle = FontStyles.Underline;
        if (data.strikethrough) textObject.fontStyle = FontStyles.Strikethrough;

        if(data.colours.Length > 0) textObject.color = data.colours[0];
        if(data.sizes.Length > 0) textObject.transform.localScale = data.sizes[0] * Vector3.one;

        // assign start colour, scale and position
        startColour = textObject.color;
        startScale = textObject.transform.localScale;
        startPosition = transform.position;

        // choose a random direction, only used for Bounce entries
        direction = (Random.value - 0.5f) * data.maxDrift * 0.5f;

        // generate entered array by running through the total number of enter types in the data
        List<bool> enteredList = new List<bool>();
        for (int i = 0; i < data.enterType.Length; i++)
        {
            enteredList.Add(false);
        }
        entered = enteredList.ToArray();

        // run through each enter type and run the necessary one off functionality for each
        for (int i = 0; i < data.enterType.Length; i++)
        {
            if(data.enterType[i] == EnterType.Simple)
            {
                entered[i] = true;
            }
            if (data.enterType[i] == EnterType.Fade)
            {
                // set opacity to 0
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0);
                startColourNoOpacity = textObject.color;
            }
            if (data.enterType[i] == EnterType.Pop)
            {
                // set scale to 0
                textObject.transform.localScale = new Vector3(0f, 0f, 0f);
                startScaleZero = textObject.transform.localScale;
            }
        }

        // mark initialised as true after everything is complete
        initialised = true;
    }

    IEnumerator ColourSwitch()
    {
        // iterate through the defined number of colour alternations and cycle through each colour
        // until all colour alternations have been complete
        for (int i = 0; i < data.numberOfColourAlternations; i++)
        {
            if (colourIndex >= data.colours.Length) colourIndex = 0;
            if (nextColourIndex >= data.colours.Length) nextColourIndex = 0;
            yield return new WaitForSeconds(data.colourAlternationDuration);
            textObject.color = data.colours[nextColourIndex];
            totalColourIndex += 1;
            colourIndex += 1;
            nextColourIndex += 1;
        }
    }

    void ColourGradient()
    {

        // lerp from one colour to the next until the target colour has been reached
        tColour += Time.deltaTime / data.colourAlternationDuration;
        textObject.color = Color.Lerp(data.colours[colourIndex], data.colours[nextColourIndex], tColour);

        // when target colour has been reached, increase all indices and reset tColour to 0
        if (tColour >= 1)
        {
            totalColourIndex += 1;
            colourIndex += 1;
            nextColourIndex += 1;
            if (colourIndex >= data.colours.Length) colourIndex = 0;
            if (nextColourIndex >= data.colours.Length) nextColourIndex = 0;
            tColour = 0;
        }
    }

    IEnumerator SizeSwitch()
    {

        // iterate through the number of size alternations and cycle through each size
        for (int i = 0; i < data.numberOfSizeAlternations; i++)
        {
            if (sizeIndex >= data.sizes.Length) sizeIndex = 0;
            if (nextSizeIndex >= data.sizes.Length) nextSizeIndex = 0;
            yield return new WaitForSeconds(data.sizeAlternationDuration);
            Vector3 newScale = new Vector3(data.sizes[nextSizeIndex],
                                           data.sizes[nextSizeIndex],
                                           data.sizes[nextSizeIndex]);
            textObject.transform.localScale = newScale;
            totalSizeIndex += 1;
            sizeIndex += 1;
            nextSizeIndex += 1;
        }

    }

    void SizeGradient()
    {

        // lerp from one size to the next until target size has been reached
        tSize += Time.deltaTime / data.sizeAlternationDuration;
        textObject.transform.localScale = Vector3.Lerp(data.sizes[sizeIndex] * startScale, data.sizes[nextSizeIndex] * startScale, tSize);

        // then increase indices and reset tSize
        if(tSize >= 1)
        {
            totalSizeIndex += 1;
            sizeIndex += 1;
            nextSizeIndex += 1;
            if (sizeIndex >= data.sizes.Length) sizeIndex = 0;
            if (nextSizeIndex >= data.sizes.Length) nextSizeIndex = 0;
            tSize = 0;
        }

    }

    void Enter()
    {
        // run through all enter types
        for (int i = 0; i < data.enterType.Length; i++)
        {
            // if the entry for this enter type has not been completed
            if (!entered[i])
            {
                if (data.enterType[i] == EnterType.Fade)
                {
                    // lerp to start colour from start colour with 0 alpha
                    tColour += Time.deltaTime / data.enterDuration;
                    textObject.color = Color.Lerp(startColourNoOpacity, startColour, tColour);
                    // if tColour is 1, colour must be at desired colour so mark entered as true for this entry
                    if (tColour >= 1f)
                    {
                        tColour = 0f;
                        entered[i] = true;
                    }
                }
                if (data.enterType[i] == EnterType.Pop)
                {
                    // lerp to target scale from 0, target scale being the start scale multiplied by the pop modifier
                    tSize += Time.deltaTime / data.enterDuration;
                    Vector3 targetScale = startScale * data.popModifier;
                    textObject.transform.localScale = Vector3.Lerp(startScaleZero, targetScale, tSize);
                    // if tSize is 1, target scale must have been reached so mark entered as true for this entry
                    if (tSize >= 1f)
                    {
                        tSize = 0f;
                        textObject.transform.localScale = startScale;
                        entered[i] = true;
                    }
                }
                if (data.enterType[i] == EnterType.Shift)
                {
                    // lerp from start position to target position
                    tPosition += Time.deltaTime / data.enterDuration;
                    Vector3 targetPosition = startPosition + new Vector3(0f, data.maxHeight, 0f);
                    transform.position = Vector3.Lerp(startPosition, targetPosition, tPosition);
                    // if tPosition is 1, desired position must have been reached so mark entered as true for this entry
                    if (tPosition >= 1f)
                    {
                        tPosition = 0f;
                        transform.position = targetPosition;
                        entered[i] = true;
                    }
                }
                // works in much the same way as Shift, only changing the x and z coordinates as well
                if (data.enterType[i] == EnterType.Bounce)
                {
                    tPosition += Time.deltaTime / data.enterDuration;
                    Vector3 targetPosition = startPosition + new Vector3(direction, data.maxHeight, direction);
                    transform.position = Vector3.Slerp(startPosition, targetPosition, tPosition);
                    if (tPosition >= 1f)
                    {
                        tPosition = 0f;
                        startPosition = transform.position; // set new start position to here
                        direction = -direction; // invert direction for on the way down
                        entered[i] = true;
                    }
                }
            }
        }
    }

    void Exit()
    {
        if (data.exitType == ExitType.Fade)
        {
            // reverse of fade enter functionality
            tColour += Time.deltaTime / data.exitDuration;
            textObject.color = Color.Lerp(startColour, startColourNoOpacity, tColour);
            if (tColour >= 1) Destroy(gameObject);
        }
        if(data.exitType == ExitType.Pop)
        {
            // reverse of pop enter functionality
            tSize += Time.deltaTime / data.exitDuration;
            Vector3 targetScale = startScale * data.popModifier;
            textObject.transform.localScale = Vector3.Lerp(targetScale, startScaleZero, tSize);
            if (tSize >= 1f) Destroy(gameObject);
        }
    }

    IEnumerator BlinkExit()
    {
        // set tExit to three times the exit duration
        tExit = data.exitDuration * 3;
        // get the current colour of the object
        Color currentColour = textObject.color;
        // set its new colour to the same but with 0 for the alpha value
        Color newColor = new Color(currentColour.r, currentColour.g, currentColour.b, 0f);
        // set exit to true
        exit = true;
        // then while exit is true
        while (exit)
        {
            // set the text colour to the invisible version
            textObject.color = newColor;
            // wait for an interval
            yield return new WaitForSeconds((data.exitDuration / tExit) * data.exitDuration);
            // make the text visible again
            textObject.color = currentColour;
            // halve the interval
            tExit *= 2;
            // wait again
            yield return new WaitForSeconds((data.exitDuration / tExit) * data.exitDuration);
            // until eventually the text is destroyed in BlinkExitDestruction()
        }
    }

    IEnumerator BlinkExitDestruction()
    {
        yield return new WaitForSeconds(data.exitDuration);
        Destroy(gameObject);
    }

    IEnumerator StartDespawn()
    {
        // mark despawnStarted as true so this only runs once
        despawnStarted = true;
        // wait for the lifetime of this text
        yield return new WaitForSeconds(data.lifetime);
        if(data.exitType == ExitType.Simple)
        {
            // destroy this text, no frills attached
            Destroy(gameObject);
        }
        if(data.exitType == ExitType.Fade || data.exitType == ExitType.Pop)
        {
            // reset tColour and tSize and mark exit as true, functionality handled by Exit() now
            tColour = 0f;
            tSize = 0f;
            exit = true;
        }
        if (data.exitType == ExitType.Blink)
        {
            // start above coroutines
            StartCoroutine(BlinkExit());
            StartCoroutine(BlinkExitDestruction());
        }
    }

}
