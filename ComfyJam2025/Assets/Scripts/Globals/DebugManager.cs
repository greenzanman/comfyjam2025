using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;

    // Top left debug display
    private string debugMessage;
    private float debugTimer;
    private const float DEFAULT_DEBUG_DURATION = 2.5f;


	// Tracking which errors already occurred
	private HashSet<string> seenMistakes = new HashSet<string>();
	// 'Console Variables' TODO: Make an actual console support
	private Dictionary<string, int> ConsoleVars = new Dictionary<string, int>();


    void Awake()
    {
        if (instance == null)
            instance = this;

        //Logger.Log("DebugManager registered", LogLevel.info);
    }

    void Update()
    {
        debugTimer = math.max(debugTimer - Time.deltaTime, 0);
    }

    public static void DisplayDebug(string message, float debugDuration = DEFAULT_DEBUG_DURATION)
    {
        instance.debugTimer = debugDuration;
        instance.debugMessage = message;
    }

    void OnGUI()
    {
        if (debugTimer > 0)
        {
            GUI.Label(new Rect(20, 20, 500, 100), debugMessage);
        }
    }


    // Console variables
    public static void RegisterConsoleVar(string newVar, int startVal = 0)
    {
        if (!instance.ConsoleVars.ContainsKey(newVar))
        {
            instance.ConsoleVars.Add(newVar, startVal);
        }
    }

	public static void SetConsoleVar(string convar, int newVal)
	{
		if (!instance.ConsoleVars.ContainsKey(convar))
		{
			if (!instance.seenMistakes.Contains(convar))
			{
				instance.seenMistakes.Add(convar);
				Logger.Log("Convar \"" + convar + "\" is not registered", LogLevel.error);
			}
		}
		else
		{
			instance.ConsoleVars[convar] = newVal;
		}
	}

	public static int GetConsoleVar(string convar)
	{
        if (!instance.ConsoleVars.ContainsKey(convar))
        {
            if (!instance.seenMistakes.Contains(convar))
            {
                Logger.Log("Convar \"" + convar + "\" is not registered", LogLevel.error);
            }
            return 0;
        }
        else
        {
            return instance.ConsoleVars[convar];
        }
	}
}
